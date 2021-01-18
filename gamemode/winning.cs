//you want your points to be 0
//piloting plane reduces your team's points
//piloting plane increases other team's points
//no one piloting reduces altitude and all team's points
function MinigameSO::setupWinning(%mini){
	for(%i = 0; %i < %mini.currentTeamCount; %i++){
		%mini.currentTeam[%i].teamSetup();
	}
}
function MinigameSO::planeTick(%mini){
	%map = %mini.currentMap;
	//check if everyone who is piloting is on the same team and who is
	%pilots = %mini.pilotGroup;
	%pilotCount = %pilots.getCount();
	//incase there are no pilots
	%noInterference = false;
	if(%pilotCount != 0){
		%noInterference = true;
		%currentPilot = %pilots.getObject(0);
		for(%i = 1; %i < %pilotCount; %i++){
			%pilot = %pilots.getObject(%i);
			%pilot.team.onPilot(%pilot);
			if(!%currentPilot.team.winsWith(%pilot.team)){
				%noInterference = false;
			}
		}
		//if everyone is on the same team then they are piloting
	}
	if(%noInterference){
		if(%mini.currentAltitude <= $SMMinigame::AltitudeStart)
			%mini.currentAltitude++;
		%mini.currentPilotingTeam = %currentPilot.team;
	} else{
		//no one is piloting lol
		%mini.currentAltitude--;
		%mini.currentPilotingTeam = 0;
		//special effects and stuff
		for(%i = 0; %i < %map.cabinBeeperCount; %i++){
			%map.cabinBeeper[%i].playSound("Beep_EKG_Sound");
		}	
		
	}
	for(%i = 0; %i < %mini.currentTeamCount; %i++){
		%team = %mini.currentTeam[%i];
		%team.pointCheck();
	}
	//always want this to update
	setAltimeter(mfloor(10500 * %mini.currentAltitude / $SMMinigame::AltitudeStart));
	//calls game over check to see if the game has ended yet
	%gameOver = %mini.gameOverCheck(%currentPilot.team);
	//if no winner continue
	if(!%gameOver){
	$smGameSchedule = %mini.schedule(1000,"planeTick");
	return;
	}
	//if there is a winner finish the game up and generate a win message to log
}
//checks if anyone has won yet
function MinigameSO::gameOverCheck(%mini){
	//if we don't want it to end for debugging
	if(!$SMMinigame::canEnd){
		return false;
	}
	//if someone won finish the game
	for(%i = 0; %i < %mini.currentTeamCount; %i++){
		%team = %mini.currentTeam[%i];
		//only some teams can win if the round ends
		if(%team.endOfRoundWinCheck)
			continue;
		if(%team.winCheck()){
			%mini.winGame();
			return true;
		}
	}
	//checks if the plane crashed yet and no one won
	if(%mini.currentAltitude <= 0){
		%mini.planeCrash();
		return true;
	}
	return false;
}
//plane crashes kinda self explanatory
function MinigameSO::planeCrash(%mini){
	%mini.logSomething("\c7The plane crashed.");
	%mini.finishgame();
}
//generate win message and logs it and hand out points to the winning team
function MinigameSO::winGame(%mini){
	for(%i = 0; %i < %mini.currentTeamCount; %i++){
		%team = %mini.currentTeam[%i];
		//only some teams can win if the round ends
		if(%team.winCheck())
			%mini.logSomething(%team.color @ %team.name SPC "\c6" @ %team.winMessage);
		//hand out points to all players
		for(%j = 0; %j < clientGroup.getCount(); %j++){
			%client = clientGroup.getObject(%j);
			%team.scoring(%client);
		}
	}
	%mini.finishgame();
	//message all players who are body guards a message
}
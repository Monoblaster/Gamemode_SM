$SMMinigame::canEnd = true;
$SMMinigame::AltitudeStart = 20;
$SMMinigame::minPlayers = 3;
$SMMinigame::cabinDoorHealthStart = 300;
$SMMinigame::ElectricHealthStart = 500;
$SMMinigame::BrickGroup = MainBrickGroup.getObject(0);
$SMMinigame::MapStart = "_SMMap_";
//bricks to be looked for
$SMMinigame::SpecialBrickName[0] = "Spawn";
$SMMinigame::SpecialBrickName[1] = "CabinSpawn";
$SMMinigame::SpecialBrickName[2] = "CabinBeeper";
$SMMinigame::SpecialBrickName[3] = "PilotSeat";
$SMMinigame::SpecialBrickName[4] = "JoinArea";
$SMMinigame::SpecialBrickName[5] = "CabinDoor";
$SMMinigame::SpecialBrickName[6] = "Light";
$SMMinigame::SpecialBrickName[7] = "Elect";
$SMMinigame::SpecialBrickName[8] = "Crate";
$SMMinigame::SpecialBrickName[9] = "Drawer";
$SMMinigame::SpecialBrickName[10] = "CabinBlock";
$SMMinigame::SpecialBrickName[11] = "Altimeter0";
$SMMinigame::SpecialBrickName[12] = "Altimeter1";
$SMMinigame::SpecialBrickName[13] = "Altimeter2";
$SMMinigame::SpecialBrickName[14] = "Altimeter3";
$SMMinigame::SpecialBrickName[15] = "Altimeter4";
$SMMinigame::SpecialBrickName[16] = "TutorialSpawn";
$SMMinigame::Debug = false;
function server_Init(){
if(isObject(SMMinigame)){
	SMMinigame.endGame();
	SMMinigame.delete();
}
%mini = new ScriptObject(SMMinigame){
	class = MiniGameSO;
	
	//miniPrefs
	owner = -1;
	title = "Sky Marshals";
	colorIdx = 4;
	
	numMembers = 0;
	
	InviteOnly = false;
	
	UseAllPlayersBricks = true;
	PlayerUseOwnBricks = false;
	UseSpawnBricks = true;
	
	Points_BreakBrick = 0;
	Points_Die = 0;
	Points_KillBot = 0;
	Points_KillPlayer = 0;
	Points_KillSelf = 0;
	Points_PlantBrick = 0;
	
	RespawnTime = 1;
	VehicleRespawnTime = 1;
	BrickRespawnTime = 1;
	BotRespawnTime = 5000;
	
	FallingDamage = false;
	BrickDamage = false;
	VehicleDamage = false;
	WeaponDamage = true;
	SelfDamage = true;
	BotDamage = false;
	
	EnableBuilding = false;
	EnablePainting = false;
	EnableWand = false;
	
	
	PlayerDataBlock = "PlayerNoJet";
	
	TimeLimit = 0;
};
MinigameGroup.add(%mini);
$MiniGameColorTaken[%mini.colorIdx] = 1;
$DefaultMinigame = %mini;
commandToAll('AddMinigameLine', %mini.getLine(), %mini, %mini.colorIdx);
%teams = new scriptGroup(){};
%maps = new scriptGroup(){};
%joinGroup = new simSet(){};
%gameGroup = new simSet(){};
%pilotGroup = new simSet(){};
%mini.teams = %teams;
%mini.maps = %maps;
%mini.joinGroup = %joinGroup;
%mini.gameGroup = %gameGroup;
%mini.pilotGroup = %pilotGroup;
//teams and roles
%role = %mini.newTeam("Sky Marshals","\c4",
"weightValue" SPC 1
TAB "winsWith" SPC "Sky_Marshals"
TAB "winMessage" SPC "have landed the plane safely!"
TAB "endOfRoundWinCheck" SPC false
);
%role.newRole("Sky Marshal","\c4",
"fakePass" SPC false
TAB "badge" SPC true
TAB "knownBy" SPC "Sky_Marshal"
TAB "knownByName" SPC "Sky Marshal"
TAB "canFly" SPC true
TAB "spawnName" SPC "spawn" 
TAB "goalMessage" SPC "protect the passengers and prevent the plane from being hijacked"
TAB "pointValue" SPC 3
TAB "max" SPC -1
TAB "min" SPC 1
TAB "inv0" SPC "fistItem"
TAB "inv1" SPC "badge"
TAB "inv2" SPC "gun"
TAB "inv3" SPC "zipTie"
TAB "inv4" SPC "zipTie"
TAB "inv5" SPC "zipTie"
TAB "inv6" SPC "clipper"
);
%role.newRole("Pilot","\c1",
"fakePass" SPC false 
TAB "badge" SPC false
TAB "knownBy" SPC "Pilot"
TAB "knownByName" SPC "Pilot"
TAB "canFly" SPC true 
TAB "spawnName" SPC "cabinspawn" 
TAB "goalMessage" SPC "keep the plane on course and from being hijacked"
TAB "pointValue" SPC 6
TAB "max" SPC 2
TAB "min" SPC 1
TAB "inv0" SPC "fistItem"
TAB "inv1" SPC "gun"
);
%role.newRole("Passenger","\c6",
"fakePass" SPC false
TAB "badge" SPC false
TAB "knownBy"
TAB "knownByName" SPC "Passenger"
TAB "canFly" SPC false 
TAB "spawnName" SPC "spawn" 
TAB "goalMessage" SPC "survive the round without the plane being hijacked"
TAB "pointValue" SPC 1
TAB "max" SPC -1
TAB "min" SPC -1
TAB "inv0" SPC "fistItem"
TAB "inv1" SPC "passport"
);

%role = %mini.newTeam("Space Pirates","\c0",
"weightValue" SPC 0.50
TAB "winsWith" SPC "Space_Pirates"
TAB "startingPoints" SPC "10"
TAB "winMessage" SPC "have hijacked the plane!"
TAB "endOfRoundWinCheck" SPC false
);
%role.newRole("Space Pirate","\c0",
"fakePass" SPC true
TAB "badge" SPC false
TAB "knownBy" SPC "Space_Pirate"
TAB "knownByName" SPC "Space Pirate"
TAB "canFly" SPC true
TAB "spawnName" SPC "spawn" 
TAB "goalMessage" SPC "keep as many passengers and pilots alive as you can while hijacking the plane"
TAB "pointValue" SPC 5
TAB "max" SPC -1
TAB "min" SPC 1
TAB "inv0" SPC "fistItem"
TAB "inv1" SPC "passport"
TAB "inv2" SPC "gun"
TAB "inv3" SPC "zipTie"
TAB "inv4" SPC "zipTie"
TAB "inv5" SPC "zipTie"
TAB "inv6" SPC "clipper"
);
%role = %mini.newTeam("Terrorists","\c3",
"weightValue" SPC 0.20
TAB "winsWith" SPC "Terrorists"
TAB "winMessage" SPC "have crashed the plane!"
TAB "endOfRoundWinCheck" SPC false
);
%role.newRole("Terrorist","\c3",
"fakePass" SPC false
TAB "badge" SPC false
TAB "knownBy" SPC "Terrorist"
TAB "knownByName" SPC "Terrorist"
TAB "canFly" SPC false
TAB "spawnName" SPC "spawn" 
TAB "goalMessage" SPC "make the plane crash."
TAB "pointValue" SPC 5
TAB "max" SPC -1
TAB "min" SPC -1
TAB "inv0" SPC "fistItem"
TAB "inv1" SPC "passport"
TAB "inv2" SPC "gun"
TAB "inv6" SPC "clipper"
);
%role = %mini.newTeam("BodyGuards","\c3",
"weightValue" SPC 0.10
TAB "winsWith" SPC "Sky_Marshals" SPC "Space_Pirates"
TAB "winMessage" SPC "have protected their client!"
TAB "endOfRoundWinCheck" SPC true
);
%role.newRole("Bodyguard","\c3",
"fakePass" SPC false
TAB "badge" SPC false
TAB "knownBy" SPC "All"
TAB "knownByName" SPC "Bodyguard"
TAB "canFly" SPC false
TAB "spawnName" SPC "spawn" 
TAB "goalMessage" SPC "make the plane crash."
TAB "pointValue" SPC 5
TAB "max" SPC -1
TAB "min" SPC -1
TAB "inv0" SPC "fistItem"
TAB "inv2" SPC "gun"
);
serverGetMaps();
%mini.finishGame();
}
//Sky Marshals
//wins when their points are 0 or if they're the only remaining team
function Sky_Marshals::winCheck(%team){
	%mini = SMMinigame;
	%game = %mini.gameGroup;
	%gameCount = %game.getCount();
	%points = %mini.teamPoints[%team.getName()];
	if(%points <= 0)
		return true;
	for(%i = 0; %i < %gameCount; %i++){
		%gamer = %game.getObject(%i);
		if(%gamer.inLobby || %gamer.player.getDatablock() == nameToId("zipTied"))
			continue;
		if(!%team.winsWith(%gamer.team) || %mini.currentPilotingTeam != %team.getID())
			return false;
	}
	return true;
}
//their points are removed when they are the team piloting and added otherwise
function Sky_Marshals::pointCheck(%team){
	%mini = SMMinigame;
	if(%mini.currentPilotingTeam == %team.getID())
		%mini.teamPoints[%team.getName()]-= 2;
	else
		%mini.teamPoints[%team.getName()]++;
}
//sets up team points and minigame variables
function Sky_Marshals::teamSetup(%team){
	%mini = SMMinigame;
	%mini.currentTeam[%mini.currentTeamCount] = %team.getName();
	%mini.teamPoints[%team.getName()] = 200;
}
//distribute points to team members
//scoring goals, everyone gains score for their decisions during a game, a negative score isn't possible
//you can only score all of your points if your team won
//do not score based on other player's decisions
function Sky_Marshals::scoring(%team,%client){
	//positive impacts
	//freeing teamates
	//killing/restraining enemys
	//if can pilot, the number of times piloted
	//negative impacts
	//freeing enemys
	//killing/restraining friends
	%percentGained = 0.25;
	if(!%team.winCheck())
		%percentGained = %percentGained / 2;
	if(%client.SMKarma < 0)
		%client.SMKarma = 0;
	%pointsGained = mFloor(%percentGained * %client.SMKarma);
	%client.setItem("Score",%client.score + %pointsGained);
	%client.schedule(1000,chatMessage,"\c6You gained\c3" SPC %pointsGained SPC "\c6points this round with a total round score of\c4" SPC %client.SMKarma);
	%client.smKarma = 0;
}
//point scoring and event functions
function Sky_Marshals::onKill(%team,%client,%deadClient){
	if(%team != %deadClient.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Sky_Marshals::onDeath(%team,%client,%sourceClient){

}
function Sky_Marshals::onDamageDoor(%team,%client,%damage){

}
function Sky_Marshals::onDamageLight(%team,%client,%damage){

}
function Sky_Marshals::onRestrain(%team,%client,%restrainedClient){
	if(%team != %restrainedClient.team)
		%client.SMKarma+=50;
	else
		%client.SMKarma-=50;
}
function Sky_Marshals::onUnRestrain(%team,%client,%restrainedClient){
	if(%team == %restrainedClient.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Sky_Marshals::onRestrained(%team,%client,%sourceClient){

}
function Sky_Marshals::onUnRestrained(%team,%client,%sourceClient){

}
function Sky_Marshals::onPilot(%team,%client){
	%client.SMKarma+=1;
}
//Space Pirates
//wins when their points are 0 or if they're the only remaining team
function Space_Pirates::winCheck(%team){
	%mini = SMMinigame;
	%game = %mini.gameGroup;
	%gameCount = %game.getCount();
	%points = %mini.teamPoints[%team.getName()];
	if(%points <= 0)
		return true;
	for(%i = 0; %i < %gameCount; %i++){
		%gamer = %game.getObject(%i);
		if(%gamer.inLobby || %gamer.player.getDatablock() == nameToId("zipTied"))
			continue;
		if(!%team.winsWith(%gamer.team) || %mini.currentPilotingTeam != %team.getID())
			return false;
	}
	return true;
}
//their points are removed when they are the team piloting and added otherwise
function Space_Pirates::pointCheck(%team){
	%mini = SMMinigame;
	if(%mini.currentPilotingTeam == %team.getID())
		%mini.teamPoints[%team.getName()]-= 2;
	else
		%mini.teamPoints[%team.getName()]++;
}
//sets up team points and minigame variables
function Space_Pirates::teamSetup(%team){
	%mini = SMMinigame;
	%mini.currentTeam[%mini.currentTeamCount] = %team.getName();
	%mini.teamPoints[%team.getName()] = 10;
}
//distribute points to team members
function Space_Pirates::scoring(%team,%client){
	//positive impacts
	//freeing teamates
	//killing/restraining enemys
	//breaking cabin doors
	//breaking lights
	//negative impacts
	//freeing enemys
	//killing/restraining friends
	%percentGained = 0.25;
	if(!%team.winCheck())
		%percentGained = %percentGained / 2;
	if(%client.SMKarma < 0)
		%client.SMKarma = 0;
	%pointsGained = mFloor(%percentGained * %client.SMKarma);
	%client.setItem("Score",%client.score + %pointsGained);
	%client.schedule(1000,chatMessage,"\c6You gained\c3" SPC %pointsGained SPC "\c6points this round with a total round score of\c4" SPC %client.SMKarma);
	%client.smKarma = 0;
}
//point scoring and event functions
function Space_Pirates::onKill(%team,%client,%deadClient){
	if(%team != %deadClient.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Space_Pirates::onDeath(%team,%client,%sourceClient){

}
function Space_Pirates::onDamageDoor(%team,%client,%damage){
	%client.SMKarma+=%damage/4;
}
function Space_Pirates::onDamageLight(%team,%client,%damage){
	%client.SMKarma+=%damage/4;
}
function Space_Pirates::onRestrain(%team,%client,%restrainedClient){
	if(%team != %restrainedClient.team)
		%client.SMKarma+=50;
	else
		%client.SMKarma-=50;
}
function Space_Pirates::onUnRestrain(%team,%client,%restrainedClient){
	if(%team == %restrainedClient.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Space_Pirates::onRestrained(%team,%client,%sourceClient){

}
function Space_Pirates::onUnRestrained(%team,%client,%sourceClient){

}
function Space_Pirates::onPilot(%team,%client){
	%client.SMKarma-=1;
}
//Terrorists
//wins when the plane crashes
function Terrorists::winCheck(%team){
	%mini = SMMinigame;
	return %mini.currentAltitude <= 0;
}
//no points so just an empty function
function Terrorists::pointCheck(%team){}
//sets up team points and minigame variables
function Terrorists::teamSetup(%team){
	%mini = SMMinigame;
	%mini.currentTeam[%mini.currentTeamCount] = %team.getName();
}
//distribute points to team members
function Terrorists::scoring(%team,%client){
	//positive impacts
	//freeing teamates
	//killing/restraining enemys
	//breaking cabin doors
	//breaking lights
	//negative impacts
	//freeing enemys
	//killing/restraining friends
	%percentGained = 0.25;
	if(!%team.winCheck())
		%percentGained = %percentGained / 2;
	if(%client.SMKarma < 0)
		%client.SMKarma = 0;
	%pointsGained = mFloor(%percentGained * %client.SMKarma);
	%client.setItem("Score",%client.score + %pointsGained);
	%client.schedule(1000,chatMessage,"\c6You gained\c3" SPC %pointsGained SPC "\c6points this round with a total round score of\c4" SPC %client.SMKarma);
	%client.smKarma = 0;
}
//point scoring and event functions
function Terrorists::onKill(%team,%client,%deadClient){
	if(%team != %deadClient.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Terrorists::onDeath(%team,%client,%sourceClient){

}
function Terrorists::onDamageDoor(%team,%client,%damage){
	%client.SMKarma+=%damage/4;
}
function Terrorists::onDamageLight(%team,%client,%damage){
	%client.SMKarma+=%damage/4;
}
function Terrorists::onRestrain(%team,%client,%restrainedClient){
	if(%team != %restrainedClient.team)
		%client.SMKarma+=50;
	else
		%client.SMKarma-=50;
}
function Terrorists::onUnRestrain(%team,%client,%restrainedClient){
	if(%team == %restrainedClient.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Terrorists::onRestrained(%team,%client,%sourceClient){

}
function Terrorists::onUnRestrained(%team,%client,%sourceClient){

}
function Terrorists::onPilot(%team,%client){
	%client.SMKarma+=1;
}
//Bodyguard
//wins when the game if their target survives
function Bodyguards::winCheck(%team){
	%winning = true;
	for(%i = 0; %i < %gameCount; %i++){
		%gamer = %game.getObject(%i);
		if(%gamer.inLobby || %gamer.player.getDatablock() == nameToId("zipTied"))
			continue;
		if(!%team.winsWith(%gamer.team) && %mini.currentPilotingTeam == %team.getID())
			%winning = false;
	}
	%mini = SMMinigame;
	%target = %mini.bodyGuardTarget;
	return !%target.inLobby && %winning;
	
}
//no points so just an empty function
function Bodyguards::pointCheck(%team){}
//sets up team points and minigame variables
function Bodyguards::teamSetup(%team){
	%mini = SMMinigame;
	%game = %mini.gameGroup;
	%gameCount = %game.getCOunt();
	%target = %game.getObject(getRandom(0, %gameCount - 1));
	while(%target.team != nameToId(Terrorists) && %target.team != nameToId(Bodyguards))
		%target = %game.getObject(getRandom(0, %gameCount - 1));
	%mini.bodyGuardTarget = %target;
	%tagetName = %target.fakeNameSM $= "" ? %target.getSimpleName() : %target.fakeNameSM;
	//message all team members who the target is
	for(%i = 0; %i < %gameCount; %i++){
		%gamer = %game.getObject(%i);
		if(%gamer.team == nameToId(Bodyguards))
			%gamer.chatMessage("\c6You must protect " @ %targetName);
	}
}
//distribute points to team members
function Bodyguards::scoring(%team,%client){
	//positive impacts
	//time target survived
	//freeing friends
	//negative impacts
	//freeing enemys
	//killing/restraining friends
	//target dead
	%percentGained = 0.25;
	if(!%team.winCheck())
		%percentGained = %percentGained / 2;
	if(%client.SMKarma < 0)
		%client.SMKarma = 0;
	%pointsGained = mFloor(%percentGained * %client.SMKarma);
	%client.setItem("Score",%client.score + %pointsGained);
	%client.schedule(1000,chatMessage,"\c6You gained\c3" SPC %pointsGained SPC "\c6points this round with a total round score of\c4" SPC %client.SMKarma);
	%client.smKarma = 0;
}
//point scoring and event functions
function Bodyguards::onKill(%team,%client,%deadClient){
	if(%team != %deadClient.team && SMMinigame.bodyGuardTarget != %deadClient && %team != SMMinigame.bodyGuardTarget.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Bodyguards::onDeath(%team,%client,%sourceClient){

}
function Bodyguards::onDamageDoor(%team,%client){

}
function Bodyguards::onDamageLight(%team,%client){

}
function Bodyguards::onRestrain(%team,%client,%restrainedClient){
	if(%team != %restrainedClient.team && SMMinigame.bodyGuardTarget != %restrainedClient && %team != SMMinigame.bodyGuardTarget.team)
		%client.SMKarma+=50;
	else
		%client.SMKarma-=50;
}
function Bodyguards::onUnRestrain(%team,%client,%restrainedClient){
	if(%team == %restrainedClient.team || SMMinigame.bodyGuardTarget == %restrainedClient || %team == SMMinigame.bodyGuardTarget.team)
		%client.SMKarma+=25;
	else
		%client.SMKarma-=25;
}
function Bodyguards::onRestrained(%team,%client,%sourceClient){

}
function Bodyguards::onUnRestrained(%team,%client,%sourceClient){

}
function Bodyguards::onPilot(%team,%client){
	%client.SMKarma+=1;
}
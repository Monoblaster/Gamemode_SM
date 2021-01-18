//starts a map vote
function startMapVote(){
	if($SMMinigame::Debug){talk("Starting map vote");}
	new scriptObject(SMMapVoter){
	voters = 0;
	};
	%maps = SMMinigame.maps;
	for(%i = 0; %i < %maps.getCount(); %i++){
		SMMapVoter.setField(%maps.getObject(%i).mapName, 0);
	}
	SMMinigame.messageAll('ServerMessage',"\c6Vote for the next map! Use \c4/vote \c3# \c6to vote use \c4/mapList \c6to see the maps!");
}
//counts up vote count and returns it
function closeMapVote(){
	%maps = SMMinigame.maps;
	if(isObject(SMMapVoter)){
	%vote = SMMapVoter;
	%vote.voters = 0;
	%maxVotes = 0;
	%maxMaps = 0;
	//finds the max voted maps and adds them to an array
	for(%i = 0; %i < %maps.getCount(); %i++){
		%mapName = %maps.getObject(%i).mapName;
		%mapVotes = %vote.getField(%mapName);
		if(%mapVotes > %maxVotes){
			%maxMap[0] = %mapName;
			%maxMaps = 1;
			%maxVotes = %mapVotes;
		} else if(%mapVotes == %maxVotes){
			%maxMap[%maxMaps] = %mapName;
			%maxMaps++;
		}
	}
	%finalMap = %maxMap[getRandom(0,%maxMaps - 1)];
	if(%maxMaps > 1){
		for(%i = 0; %i < %maxMaps; %i++){
			%tiedMaps = %tiedMaps @ "\c3" @ %maxMap[%i] @ "\c6";
			if(%maxMaps > 2){
				if(%i + 1 < %maxMaps){
					%tiedMaps = %tiedMaps @ ", ";
				} else{
					%tiedMaps = %tiedMaps @ ", and";
				}
			} else{
				if(%i + 1 < %maxMaps){
				%tiedMaps = %tiedMaps @ " and ";
				}
			}
		}
		SMMinigame.messageAll('ServerMessage',"\c6Map vote is closed! There was a tie between " @ %tiedMaps @ "!");
		SMMinigame.messageAll('ServerMessage',"\c3" @ %finalMap @ " \c6has been chosesn randomly!");
	} else{
		SMMinigame.messageAll('ServerMessage',"\c6Map vote is closed! The chosen map was \c3" @ %finalMap @ "\c6!");
	}
	SMMapVoter.delete();
	return %finalMap;
	} else{
		if($SMMinigame::Debug){talk("No vote started returning random map");}
		return %maps.getObject(getRandom(%maps.getCount() - 1)).mapName;
	}
}
//returns the map list to a client
function serverCmdMapList(%client){
	%maps = SMMinigame.maps;
	%client.ChatMessage("\c6Maps:");
	for(%i = 0; %i < %maps.getCount(); %i++){
		%client.ChatMessage("\c3" @ %i + 1 SPC %maps.getObject(%i).mapName);
	}
}
//lets the player vote for a map
function serverCmdVote(%client,%mapNum){
	%maps = SMMinigame.maps;
	%vote = SMMapVoter;
	%mapnum = %mapnum - 1;
	if(0 <= %mapNum && %mapNum < %maps.getCount() && isObject(%vote)){
		if($SMMinigame::Debug){talk("Checking for a repeated vote for map vote " @ %mapNum);}
		for(%i = 0; %i < %vote.voters; %i++){
			%voter = %vote.getField("v" @ %i);
			if($SMMinigame::Debug){talk("Looking at " @ %voter);}
			if(getField(%voter, 1) $= %client.getBLID()){
				if(getField(%voter, 0) $= %mapNum){
					if($SMMinigame::Debug){talk("Repeated vote " @ %voter);}
					%client.chatMessage("\c3You already voted for this map!");
					return;
				}
				if($SMMinigame::Debug){talk("Changed vote" @ %voter);}
				%vote.setField(%maps.getObject(getField(%voter, 0)).mapName, %vote.getField(%maps.getObject(getField(%voter, 0)).mapName) - 1);
				%vote.setField(%maps.getObject(%mapNum).mapName, %vote.getField(%maps.getObject(%mapNum).mapName) + 1);	
				%vote.setField("v" @ %i, %mapNum @ "\t" @ %client.getBlid());
				%client.ChatMessage("\c3You have instead voted for " @ %maps.getObject(%mapNum).mapName @ "!");
				return;
			}
		}
		if($SMMinigame::Debug){talk("New vote");}
		%vote.setField(%maps.getObject(%mapNum).mapName, %vote.getField(%maps.getObject(%mapNum).mapName) + 1);
		%vote.setField("v" @ %vote.voters, %mapNum @ "\t" @ %client.getBlid());
		%vote.voters++;
		%client.ChatMessage("\c3You  have voted for " @ %maps.getObject(%mapNum).mapName @ "!");
	} else{
		serverCmdMapList(%client);
	}
}
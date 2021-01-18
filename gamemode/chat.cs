$lobbyCanTalkToInGame = false;
$inGameCanTalkToLobby = false;
function GameConnection::sendLobbyMessage(%this, %text){
	%msg = "\c7[\c4Lobby\c7] \c3" @ %this.getSimpleName() @ "\c6: " @ %text;
	echo(%msg);
	for(%i = 0; %i < ClientGroup.getCount(); %i++){
		%client = ClientGroup.getObject(%i);
		//want people outside of the minigame to hear this too;
		if(%client.inLobby || !isObject(%client.minigame))
			%client.chatMessage(%msg);
	}
}

function GameConnection::sendInGameMessage(%this, %text){
	%msg = "\c7[\c2In-Game\c7] \c3" @ %this.getSimpleName() @ "\c6: " @ %text;
	echo(%msg);
	for(%i = 0; %i < ClientGroup.getCount(); %i++){
		%client = ClientGroup.getObject(%i);
		//want people outside of the minigame to hear this too;
		if(%client.inLobby || !isObject(%client.minigame) || $lobbyCanTalkToInGame)
			%client.chatMessage(%msg);
	}
}

function GameConnection::sendLoadingMessage(%this, %text){
	%msg = "\c7[\c1Loading\c7] \c3" @ %this.getSimpleName() @ "\c6: " @ %text;
	echo(%msg);
	for(%i = 0; %i < ClientGroup.getCount(); %i++){
		%client = ClientGroup.getObject(%i);
		//want people outside of the minigame to hear this too;
		if(%client.inLobby || !isObject(%client.minigame))
			%client.chatMessage(%msg);
	}
}
function GameConnection::knownByMessage(%this, %text){
	%roleColor = %this.role.color;
	%roleName = %this.role.knownByName;
	%msg = "\c7[" @ %roleColor @ %roleName @ "\c7] \c3" @ %this.getSimpleName() @ "\c6: " @ %text;
	%group = SMMInigame.gameGroup;
	SMMinigame.logAdminSomething("c", %this, %msg);
	for(%i = 0; %i < %group.getCount(); %i++){
		%client = %group.getObject(%i);
		if(!%client.inLobby && %client.isKnownTo(%this))
			%client.chatMessage(%msg);
	}
}
function GameConnection::sendGameMessage(%this, %text, %rangeValue){
	switch(%rangeValue){
		case 0:
			if(isEventPending($smGameSchedule))
				%this.sendRangedMessage(%text, 10, "whispers");
			else
				%this.knownByMessage(%text);
		case 1:
			%this.sendRangedMessage(%text, 35, "says");
		case 2:
			%this.sendRangedMessage(strUpr(%text), 50, "yells");
	}
}
function GameConnection::sendRangedMessage(%this, %text, %distance, %adj){
	%name = %this.fakeNameSM;
	SMMinigame.logAdminSomething("c", %this, " \c6" @ %adj @ ", " @ %text);
	if(%name $= "")
		%name = %this.getSimpleName();
	for(%i = 0; %i < clientGroup.getCount(); %i++){
		%client = clientGroup.getObject(%i);
		
		if(isObject(%client.player) && !%client.inLobby){
			%difference = vectorDist(getWords(%this.player.getTransform(),0,2), getWords(%client.player.getTransform(),0,2));
			//normal text
			if(%difference <= %distance){
				%msg = "\c3" @ %name @ " \c6" @ %adj @ ", " @ %text;
				%client.chatMessage(%msg);
			}else if(%difference <= (%distance * 1.25)){
				%msg = "\c3" @ %name @ " \c6" @ %adj @ ", " @ degradeText(%text, (%difference - %distance) / %distance);
				%client.chatMessage(%msg);
			}else if(%difference <= (%distance * 2)){
				%name  = "Someone";
				%degradeText = degradeText(%text, (%difference - %distance) / %distance);
				if(strLen(%degradeText) == 0)
					%degradeText = "something";
				%msg = "\c7" @ %name @ " \c6" @ %adj @ ", " @ %degradeText;
				%client.chatMessage(%msg);
			}
			
		}
	}
}

function degradeText(%str, %percent){
	%iterations = mFloor(strLen(%str) * %percent);
	for(%i = 0; %i < %iterations; %i++){
		%randomChar = getRandom(0, strLen(%str) - 1);
		%str = getSubStr(%str, 0, %randomChar) @ getSubStr(%str, %randomChar + 1, strLen(%str));
	}
	return %str;
}

package smChat{
	function serverCmdMessageSent(%client, %text){
		%text = trim(stripMLcontrolChars(%text));
		if(strLen(%text) > 0){
			//so chat eval still works
			if(!(getSubStr(%text,0,1) $= "\\" && %client.canEval)){
				//loading tags are nice
				if(!%client.hasSpawnedOnce)
					return %client.sendLoadingMessage(%text);
				//don't want people outside of the minigame to send lobby messages
				if(%client.inLobby && isObject(%client.minigame))
					return %client.sendLobbyMessage(%text);
				if(!%client.inLobby && isObject(%client.minigame) && $inGameCanTalkToLobby)
					return %client.sendInGameMessage(%text);
				if(!%client.inLobby && isObject(%client.player) && isObject(%client.minigame)){
					//local chat stuff
					if(getSubStr(%text, strLen(%text) - 1, 1) $= "!" || (strCmp(strUpr(%text), %text) == 0 && strCmp(%text, "A") >= 0)){
						//yelling
						if(getSubStr(%text, strLen(%text) - 1, 1) !$= "!")
							%text = %text @ "!";
						return %client.sendGameMessage(%text, 2);
					}
					//normal chat
					return %client.sendGameMessage(%text, 1);
				}
			}
			parent::serverCmdMessageSent(%client, %text);
		}
	}
	
	function serverCmdTeamMessageSent(%client, %text){
		%text = trim(stripMLcontrolChars(%text));
		if(strLen(%text) > 0){
			if(!%client.inLobby && isObject(%client.minigame) && $inGameCanTalkToLobby)
					return %client.sendInGameMessage(%text);
			if(!%client.inLobby && isObject(%client.player) && isObject(%client.minigame)){
				//whisper in local chat
				return %client.sendGameMessage(%text, 0);
			} else{
				return serverCmdMessageSent(%client, %text);
			}
			parent::serverCmdTeamMessageSent(%client, %text);
		}
	}
};
deactivatePackage("smChat");
activatePackage("smChat");

//events
registerOutputEvent(GameConnection, AddToJoinList);
registerOutputEvent(GameConnection, RemoveFromJoinList);
registerOutputEvent(GameConnection, AddToPilotList);
registerOutputEvent(GameConnection, RemoveFromPilotList);
//teams
//sets up new team object and adds it to the parent %data is divided by tabs
function MiniGameSO::newTeam(%this,%name,%color,%data){
		if($SMMinigame::Debug){talk("Creating Team " @ %name);}
	%obj = new scriptObject(strReplace(%name," ", "_")){
		class = smTeam;
		
		name = %name;
		color = %color;
		memberNum = 0;
		roleNum = 0;
	};
	for(%i = 0; %i < getFieldCount(%data); %i++){
		%str = getField(%data,%i);
		%obj.setField(getWord(%str,0),getWords(%str,1,strLen(%str)));
	}
	
	%this.teams.add(%obj);
	return %obj;
}
//sets up a new role object and adds it to the parent %data is divided by tabs
function smTeam::newRole(%this,%name,%color,%data){
	if($SMMinigame::Debug){talk("Creating role for team " @ %this.name @ ", " @ %name);}
	%obj = new ScriptObject(strReplace(%name," ", "_")){
		class = smRole;
		
		team = %this;
		name = %name;
		color = %color;
	};
	for(%i = 0; %i < getFieldCount(%data); %i++){
		%str = getField(%data,%i);
		%obj.setField(getWord(%str,0),getWords(%str,1,strLen(%str)));
	}
		
	%this.role[%this.roleNum] = %obj;
	%this.roleNum++;
	return %obj;
}
//maps
//creates a map object for a map
function MiniGameSO::newMap(%this,%mapName){
	if($SMMinigame::Debug){talk("Creating SMMap_" @ %mapName);}
	%obj = new scriptObject("SMMap_" @ %mapName){
		class = SMMap;
		
		mapName = %mapName;
		spawnCount = 0;
	};
	if(%mapName $= "Lobby"){
		%this.lobby = %obj;
	} else{
		%this.maps.add(%obj);
	}
	%obj.addSpecialBricks();
}
//searchs for maps and creates their objects
function MiniGameSO::searchForMaps(%this){
	if($SMMinigame::Debug){talk("Starting Map Search");}
	%brickgroup = $SMMinigame::BrickGroup;
	%mapStart = $SMMinigame::MapStart;
	%mapStartLength = strLen($SMMinigame::MapStart);
	%blacklistLength = 0;
	for(%i = 0; %i < %brickgroup.NTNameCount; %i++){
		%blacklistflag = false;
		%brickName = %brickgroup.NTName[%i];
		if($SMMinigame::Debug){talk("Looking at " @ %brickName);}
		if(strPos(%brickName ,%mapStart) > -1){
			%name = getSubStr(%brickname, %mapStartLength, strpos(getsubstr(%brickname, %mapStartLength, strlen(%brickname)),"_"));
			//prevents constant map adding and deleting
			for(%j = 0; %j < %blackListLength; %j++){
				if(%blackList[%j] $= %name){
					if($SMMinigame::Debug){talk("Blacklisted " @ %name);}
					%blackListFlag = true;
				}
			}
			if(!%blackListFlag){
				if($SMMinigame::Debug){talk("Map confirmed " @ %name);}
				if(isObject("SMMap_" @ %name)){

					if($SMMinigame::Debug){talk("Deleting previous map object " @ "SMMap_" @ %name);}
					eval("(\"SMMap_\" @ %name).delete();");
				}
				%this.newMap(%name);
				%blacklist[%blackListLength] = %name;
				%blackListLength++;
			}
		}
	}
}
//adds all special bricks into the map object like spawns, doors, and crates special brick names can be changed in $SMMinigame Prefs in init.cs
function SMMap::addSpecialBricks(%this){
	%c = 0;
	if($SMMinigame::Debug){talk("Starting Special Brick Search");}
	while($SMMinigame::SpecialBrickName[%c] !$= ""){
		%brickgroup = $SMMinigame::BrickGroup;
		%brickName = $SMMinigame::SpecialBrickName[%c];
		%searchCount = %brickgroup.getField("NTObjectCount_" @ %this.getName() @ "_" @ %brickName);
		%searchName = "NTObject_" @ %this.getName() @ "_" @ %brickName;
		if($SMMinigame::Debug){talk("Looking at " @ %searchName @ " length of " @ %searchCount);}
		for(%i = 0; %i < %searchCount; %i++){
			%this.setField(%brickName @ %i, %brickgroup.getField(%searchName @ "_" @ %i));
			%this.setField(%brickName @ "Count", %this.getField(%brickName @ "Count") + 1);
		}
		%c++;
	}
}
//starts search for maps and sets up zones
function serverGetMaps(){
	SMMinigame.searchForMaps();
	//set up zones
	%lobby = SMMap_Lobby;
	%maps = SMminigame.maps;
	for(%i = 0; %i < %lobby.joinAreaCount; %i++){
		%lobby.joinArea[%i].setZone(1,10,0);
	}
	for(%j = 0; %j < %maps.getCount(); %j++){
		%map = %maps.getObject(%j);
		for(%i = 0; %i < %map.PilotSeatCount; %i++){
			%map.PilotSeat[%i].setZone(1,10,0);
		}
	}
}
//returns a random spawn from a map
function SMMap::getRandomSpawnBrick(%this,%name){
	return %this.getField(%name @ getRandom(%this.getField(%name @ "count") - 1)).getSpawnPoint();
}
//events to handle people in special zones
//adds players to the lobby join list
function GameConnection::AddToJoinList(%this){
	if(SMMinigame.isMember(%this)){
		//if the player has an acitve ban.
		if($Server::SMMinigame::gameBan[%this.BL_ID] > $Server::SMMinigame::RoundNumber){
			//do an impulse away from the join brick for looks
			%pos = $inputtarget_self.getPosition();
			%searchPos = %this.player.getPosition();
			%searchObj = %this.player;
			%force = 100;
			%verticalForce = 100;
			%distanceFactor = 5;
			%impulseVec = VectorSub (%searchPos, %pos);
			%impulseVec = VectorNormalize (%impulseVec);
			%impulseVec = VectorScale (%impulseVec, %force * %distanceFactor);
			%searchObj.applyImpulse (%searchPos, %impulseVec);
			%impulseVec = VectorScale ("0 0 1", %verticalForce * %distanceFactor);
			%searchObj.applyImpulse (%pos, %impulseVec);
			return;
		}
		if($SMMinigame::Debug){talk("Adding " @ %this.getSimpleName() @ " to join list");}
		SMMinigame.joinGroup.add(%this);
	} else{
	%this.chatMessage("Get in the minigame if you want to join bub");
	}
}
//removes players from the lobby join list
function GameConnection::RemoveFromJoinList(%this){
	if(SMMinigame.isMember(%this)){
		if($SMMinigame::Debug){talk("Removing " @ %this.getSimpleName() @ " from join list");}
		SMMinigame.Joingroup.remove(%this);
	} else{
	%this.chatMessage("Get in the minigame if you want to leave bub");
	}
}
//adds a possible pilot to the piloting list
function GameConnection::AddToPilotList(%this){
	if(SMMinigame.isMember(%this) && %this.role.canfly){
		SMMinigame.logAdminSomething("p", %this, "started piloting");
		SMMinigame.PilotGroup.add(%this);
		if($SMMinigame::Debug){talk("Adding " @ %this.getSimpleName() @ " to the pilot list");}
	} else{
		%this.chatMessage("\c3You can't fly! Try to find someone who can.");
	}
}
function GameConnection::RemoveFromPilotList(%this){
	if(SMMinigame.pilotGroup.isMember(%this) && SMMinigame.isMember(%this)){
		SMMinigame.logAdminSomething("p", %this, "stopped piloting");
		SMMinigame.PilotGroup.remove(%this);
		if($SMMinigame::Debug){talk("Removing " @ %this.getSimpleName() @ " from the pilot list");}
	}
}

//removes a possible pilot from the piloting list
//logs something to the current log file
function MiniGameSO::logSomething(%this,%str){
	if(%this.logFile !$= ""){
		%log = new fileObject(){};
		%log.openForAppend(%this.logFile);
		%log.writeLine(%str);
		%log.close();
		%log.delete();
	}
}
function MiniGameSO::logAdminSomething(%this,%catagory,%target,%str){
	if(%this.logAdminFile !$= ""){
		%log = new fileObject(){};
		%log.openForAppend(%this.logAdminFile);
		%log.writeLine(%catagory TAB %target.getSimpleName() TAB %target.role.color SPC %target.role.name  TAB %str);
		%log.close();
		%log.delete();
	}
}
//sends the games logs to everyone
function MiniGameSO::sendGameLog(%this){
	if(%this.logFile !$= ""){
		%log = new fileObject(){};
		%log.openForRead(%this.logFile);
		while(!%log.isEof()){
			%this.messageAll('chatMessage',%log.readLine());
		}
		%log.close();
		%log.delete();
	}
}
//sends admin logs to the client
function serverCmdLog(%client, %catagory, %target){
	if(!%client.isAdmin)
		return;
	switch$(%catagory){
		case "d":
		case "p":
		case "c":
		case "m":
		default:
			%invalidCatagory = true;
	}
	if(%catagory $= "" || %invalidCatagory){
		%client.chatMessage("\c3Enter a catagory:");
		%client.chatMessage("\c4d\c6: Damage");
		%client.chatMessage("\c4p\c6: Piloting");
		%client.chatMessage("\c4c\c6: Chat");
		%client.chatMessage("\c4m\c6: Map");
		return;
	}
	%log = new fileObject(){};
	%log.openForRead(%client.minigame.LogAdminFile);
	%client.chatMessage("\c3Log:");
	while(!%log.isEoF()){
		%line = %log.readLine();
		if(getField(%line, 0) $= %catagory && (!%target || getField(%line, 1).getSimplename() $= %target))
			%client.chatMessage("\c4" @ getField(%line, 1) SPC getField(%line, 2) SPC "\c6" @ getField(%line, 3));
	}
}
//returns true if the player's role is known to the client
function GameConnection::isKnownTo(%this,%client){
	if(!isObject(%client))
		return false;
	%field = %this.role.knownBy;
	for(%i = 0; %i < getWordCount(%field); %i++){
		if(getWord(%field, %i) $= %client.role.getName() || getWord(%field, %i) $= "all")
			return true;
	}
	return false;
}
function smRole::roleKnownTo(%role, %checkRole){
	%knownByWords = %role.knownBy;
	%knownByCount = getWordCount(%knownByWords);
	for(%i = 0; %i < %knownByCount; %i++){
		%knownBy = getWord(%knownByWords, %i);
		if(%knownBy $= %checkRole.getName() || %knownBy $= "all")
			return true;
	}
	return false;
}

function smTeam::winsWith(%team, %checkTeam){
	%winsWithWords = %team.winsWith;
	%winsWithCount = getWordCount(%winsWithWords);
	for(%i = 0; %i < %winsWithCount; %i++){
		%winsWithWord = getWord(%winsWithWords, %i);
		if(%winsWithWord $= %checkTeam.getName()){
			return true;
		}
	}
	return false;
}

function MinigameSO::checkForDeadPilots(%mini){
	%pilots = %mini.pilotGroup;
	for(%i = 0; %i < %pilots.getCount(); %i++){
		%pilot = %pilots.getObject(%i);
		if(!isObject(%pilot))
			%pilots.remove(%pilot);
		if(%pilot.inLobby)
			%pilots.remove(%pilot);
	}
}

package smMini{
	
	//sets up player when they join
	function MinigameSO::addMember(%obj,%client){
		%client.inLobby = true;
		return parent::addMember(%obj,%client);
	}
	//pick spawn points when they're in lobby send them to lobby otherwise their map
	function MiniGameSO::pickSpawnPoint(%obj,%client){
		if(%client.inLobby){
			//makes sure that only people who play are included
			if(%client.hasSpawnedOnce){
				%client.setDefaultAvatar();
			}
			if(!%client.tutorialDone){
				return SMMap_Lobby.getRandomSpawnBrick("tutorialSpawn");
			}
			if($SMMinigame::Debug){talk("Sending " @ %client.getSimpleName() @ " to lobby");}
			return SMMap_Lobby.getRandomSpawnBrick("spawn");
		} else if(%obj.currentMap !$= ""){
			if($SMMinigame::Debug){talk("Sending " @ %client.getSimpleName() @ " to " @ %client.role.spawnName);}
			return %obj.currentMap.getRandomSpawnBrick(%client.role.spawnName);
		}
		if($SMMinigame::Debug){talk("No spawn point found sending to default");}
		
		parent::pickSpawnPoint(%obj, %client);
	}
	function GameConnection::spawnPlayer(%client){
		Parent::spawnPlayer(%client);
		if(%client.inLobby){
			%client.player.addItem(badgeItem, %client);
		}
	}
	//when someone dies send them to the lobby if in a minigame and no where else + n0 d34th m3ss4g3s
	function GameConnection::onDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc){
		if( isObject(%client.miniGame)){
			if(!%client.inLobby){
				SMMinigame.logSomething(%sourceClient.team.color @ %sourceClient.getSimpleName() @ "\c7(" @ %sourceClient.role.color @ %sourceClient.role.name @ "\c7)" @ "\c4 killed " @ %client.team.color @ %client.getSimpleName() @ "\c7(" @ %client.role.color @ %client.role.name @ "\c7)");
				//teamate check and stuff
				%client.team.onDeath(%client,%sourceClient);
				%sourceClient.team.onKill(%sourceClient,%client);
			}
			%client.inLobby = true;
			//stolen from decompiled
			if (%sourceObject.sourceObject.isBot)
			{
				%sourceClientIsBot = 1;
				%sourceClient = %sourceObject.sourceObject;
			}
			%player = %client.Player;
			if (isObject (%player))
			{
				%player.setShapeName ("", 8564862);
				if (isObject (%player.tempBrick))
				{
					%player.tempBrick.delete ();
					%player.tempBrick = 0;
				}
				%player.client = 0;
			}
			else 
			{
				warn ("WARNING: No player object in GameConnection::onDeath() for client \'" @ %client @ "\'");
			}
			if (isObject (%client.Camera) && isObject (%client.Player))
			{
				if (%client.getControlObject () == %client.Camera && %client.Camera.getControlObject () > 0)
				{
					%client.Camera.setControlObject (%client.dummyCamera);
				}
				else 
				{
					%client.Camera.setMode ("Corpse", %client.Player);
					%client.setControlObject (%client.Camera);
					%client.Camera.setControlObject (0);
				}
			}
			%client.Player = 0;
			if (isObject (%client.miniGame))
			{
				if (%sourceClient == %client)
				{
					%client.incScore (%client.miniGame.Points_KillSelf);
				}
				else if (%sourceClient == 0)
				{
					%client.incScore (%client.miniGame.Points_Die);
				}
				else 
				{
					if (!%sourceClientIsBot)
					{
						%sourceClient.incScore (%client.miniGame.Points_KillPlayer);
					}
					%client.incScore (%client.miniGame.Points_Die);
				}
			}
			%clientName = %client.getPlayerName ();
			if (isObject (%sourceClient))
			{
				%sourceClientName = %sourceClient.getPlayerName ();
			}
				else if (isObject (%sourceObject.sourceObject) && %sourceObject.sourceObject.getClassName () $= "AIPlayer")
			{
				%sourceClientName = %sourceObject.sourceObject.name;
			}
			else 
			{
				%sourceClientName = "";
			}
			%mg = %client.miniGame;
			return;
		}
		parent::onDeath(%client, %sourceObject, %sourceClient, %damageType, %damLoc);
	}
	function Armor::onRemove(%db, %obj){
		SMMinigame.checkForDeadPilots();
		Parent::onRemove(%db, %obj);
	}
	function Player::playDeathAnimation(%this){
		if(isObject(%this.client.minigame))
			%this.client.dropAllTools();
		return Parent::playDeathAnimation(%this);
	}
	function serverCmdSuicide(%client){
		if(isObject(%client.minigame))
			return;
		Parent::severCmdSuicide(%client);
	}
};
deactivatepackage("smMini");
activatepackage("smMini");
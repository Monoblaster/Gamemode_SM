//sends all players in the joinlist into the game and starts setup
registerOutputEvent(fxDTSBrick, repairDoorCheck);
$Server::SMMinigame::RoundNumer = 0;
function getJoinArea(){
	%lobby = SMMap_Lobby;
	$SMMinigame::JoinListCount = 0;
	for(%i = 0; %i < %lobby.joinAreaCount; %i++){
		%joinArea = %lobby.joinArea[%i];
		
		%maxHeight = 20;
		
		%pos = vectorAdd(%joinArea.getWorldBoxCenter(), "0 0" SPC %maxHeight / 2);
		
		%box = vectorAdd(%joinArea.getObjectBox(), "0 0" SPC %maxHeight);
		
		%bob = new StaticShape(){
			position  = %pos;
			box = %box;
		};
		
		%mask = $TypeMasks::PlayerObjectType;
		
		talk(%pos SPC %box SPC %mask);
		
		initContainerBoxSearch(%pos, %box, %mask);
		
		while(%searchObj = containerSearchNext()){
			talk(%searchObj SPC "veh");
			$SMMinigame::JoinList[$SMMinigame::JoinListCount] = %searchObj;
			$SMMinigame::JoinListCount++;
		}
	}
}

function MiniGameSO::gameStartCheck(%this,%attemptStart,%firstgoaround){
	%lobby = SMMap_Lobby;
	if(%firstgoaround){
		%this.currentMap = "SMMap_" @ closeMapVote();
	}
	if(%this.joinGroup.getCount() >= $SMMinigame::minPlayers){
		if(%attemptStart){
			%this.messageAll("","\c3Starting in...");
			return %this.gameCountDown(5);
		}
		if($SMMinigame::Debug){talk("Starting Game");}
		for(%i = 0; %i < %lobby.joinAreaCount; %i++){
			%lobby.joinArea[%i].setEmitter(-1);
			%lobby.joinArea[%i].setColor(40);
			%lobby.joinArea[%i].setColorFX(0);
		}
		%this.startGracePeriod();
	} else{
		if(!%attemptStart){
			%this.messageAll('',"\c3Not enough players to start! Go into the join area if you want to join.");
			for(%i = 0; %i < %lobby.joinAreaCount; %i++){
				%lobby.joinArea[%i].setEmitter("LaserEmitterA");
			}
		}
		%this.schedule(1000,"gameStartCheck",true, false);
	}
}
//starts game and grace period timer
function MinigameSO::startGracePeriod(%this){
	$itemsDecay = false;
	//sets up logs and starts a new log file
	%this.logFile = "config/SM/log/game " @ strreplace(strreplace(getdatetime() @ ".txt","/", "_"),":", "_");
	%this.logAdminFile = "config/SM/log/game " @ strreplace(strreplace(getdatetime() @ "Admin.txt","/", "_"),":", "_");
	%this.logSomething("\c6Starting game on \c5" @ %this.currentMap);
	//role list is made
	%this.makeRoleList(%this.joingroup.getcount(), Sky_Marshals TAB Space_Pirates TAB Terrorists TAB Bodyguards);
	%this.sendPlayersToMap();
	%this.setupWinning();
	//sets up score and brick health tracking
	%this.currentAltitude = $SMMinigame::AltitudeStart;
	$electricHealth = $SMMinigame::ElectricHealthStart;
	%this.setupMap();
	//creates looting tables and closes boxes to pickups
	setupItems();
	closeboxes();
	//sets up lobby game here
	//timer to ending grace period
	%this.schedule(10000, "finishGracePeriod");
}
//gives items to all players and starts the game clock
function MinigameSO::finishGracePeriod(%this){
	if($SMMinigame::Debug){talk("Grace Period Over");}
	%this.messageGame('MsgUploadStart',"\c5This is your control tower, land as soon as possible as suspected criminals may be onboard.");
	//we set it up here so players can see their player before the round begins
	%this.sendItems();
	$smGameSchedule = %this.schedule(1000,"planeTick");
}
//countdown timer used until game starts
function MinigameSO::gameCountDown(%this,%count){
	if(%count > 0){
		%this.messageAll("","\c4" @ %count @ "\c3...");
		%this.schedule(1000,"gameCountDown", %count - 1);
	} else{
		%this.gameStartCheck(false, false);
	}
}
//sends all remaining players who were in the game back to the lobby and starts end scoring
function MiniGameSO::FinishGame(%this){
	
	if($SMMinigame::Debug){talk("Ending Game");}
	%this.setEnviroment(false);
	cancel($smGameSchedule);
	%lobby = SMMap_Lobby;
	for(%i = 0; %i < %lobby.joinAreaCount; %i++){
		%lobby.joinArea[%i].setColor(5);
		%lobby.joinArea[%i].setColorFX(3);
	}
	
	%game = %this.gamegroup;
	//send all players back
	for(%i = 0; %i < %game.getCount(); %i++){
		%gamer = %game.getObject(%i);
		if(!%gamer.inLobby){
			%gamer.inLobby = true;
			%gamer.spawnPlayer();
		}
	}
	%game.clear();
	%this.sendGameLog();
	%this.logFile = "";
	$itemsDecay = true;
	startMapVote();
	$Server::SMMinigame::RoundNumber++;
	//do round ban list cleanup
	for(%i = $Server::SMMinigame::GameBanCount - 1; %i >= 0; %i++){
		if($Server::SMMinigame::GameBan[$Server::SMMinigame::GameBanID] <= $Server::SMMinigame::RoundNumer){
			if(%i + 1 >= $Server::SMMinigame::GameBanCount){
				$Server::SMMinigame::GameBanCount--;
			}
		}
	}
	%this.schedule(30000,"gameStartCheck", false, true);
}
function MinigameSO::setEnviroment(%this,%dark){
	if(%dark){
		Sun.color = "0 0 0 1";
		Sun.ambient = "0.05 0.05 0.051";
		Sun.shadowColor = "0.05 0.05 0.05 1";
		sun.elevation = 90;
		Sun.azimuth = 250;
	} else{
		Sun.color = "0.3 0.3 0.3 1";
		Sun.ambient = "0.4 0.4 0.4 1";
		Sun.shadowColor = "0.3 0.3 0.3 1";
		sun.elevation = 90;
		Sun.azimuth = 250;
	}
	Sun.sendUpdate();
}

//sends all players in the joingroup to the map!
function MinigameSO::SendPlayersToMap(%this){
	%join = %this.joinGroup;
	%game = %this.gameGroup;
	for(%i = 0; %i < %join.getCount(); %i++){
		%client = %join.getObject(%i);
		if(isObject(%client)){
			%client.inLobby = false;
			%game.add(%client);
			%client.setUpPlayer(%i);
		}
	}
	//seperate loop so teams are displayed correctly in role text
	for(%i = 0; %i < %game.getCount(); %i++){
		%client = %game.getObject(%i);
		if(isObject(%client)){
			%client.sendRoleText();
		}
	}
}
//sets the map up before a match
function MinigameSO::SetupMap(%this){
	%map = %this.currentMap;
	updateLight(false);
	for(%i = 0; %i <  %map.CabinDoorCount; %i++){
		%map.cabinDoor[%i].updateDoor(true);
	}
}
//takes n role and spawns the player 
function GameConnection::setUpPlayer(%this,%count){
	%role = SMMinigame.roleList.role[%count];
	%game = smminigame.gameGroup;
	%team = %role.team;
	%this.role = %role;
	%this.team = %team;
	%this.spawnPlayer();
	if($SMMinigame::Debug){talk("Sending role to " @ %this.getSimpleName() @ " as " @ %This.role.name SPC %this.team.name);}
	SMMinigame.logSomething("\c4" @ %this.getSimpleName() @ "\c6 spawned as a " @ %role.color @ %role.name @ "\c6 on the " @ %role.team.color @ %role.team.name @ "\c6 team!");
}
//gives info on role
function GameConnection::sendRoleText(%this){
	%mini = %this.minigame;
	%role = %this.role;
	%team = %this.team;
	%game = %this.minigame.gameGroup;
	//team message
	%this.chatMessage("\c3You are a " @ %role.color @ %role.name @ "\c3 on the " @ %role.team.color @ %role.team.name @ "\c3 team! You goal is to \c4" @ %role.goalMessage @ "\c3!");
	%this.centerPrint("\c3You are a " @ %role.color @ %role.name, 3);
	//special ability messages
	if(%role.badge) %this.chatMessage("\c6You have a badge instead of passport, show this to anyone to tell you who you are!");
	if(%role.fakePass) %this.chatMessage("\c6Your passport is fake! Be careful who you show it to as they would think you were up to something.");
	if(%role.canFly && !%role.hijacks) %this.chatMessage("\c6You are a trained pilot! You can sit in a pilot's seat and keep the plane on course!");
	if(%role.canFly && %role.hijacks) %this.chatMessage("\c6You have been trained to pilot a plane! Sitting in the pilot's seat will start the hijacking process!");
	//teamates
	%this.chatMessage("\c3Known people:");
	%knowssomeone = false;
	%gameCount = %game.getCount();
	for(%i = 0; %i < %gameCount; %i++){
		%gamer = %game.getObject(%i);
		if(%gamer.isKnownTo(%this)){
			%name = %gamer.fakeNameSM;
			if(%name $= "")
				%name = %gamer.getSimpleName();
			%knowssomeone = true;
			%this.chatMessage("\c6" @ %name SPC"(" @ %gamer.role.color @ %gamer.role.name @ "\c6)");
		}
	}
	if(!%knowssomeone){
		%this.chatMessage("\c6No one");
	}
	//who sees your team messages
	%roleCount = 0;
	for(%i = 0; %i < %mini.currentTeamCount; %i++){
		%team = %mini.currentTeam[%i];
		for(%j = 0; %j < %team.roleNum; %j++){
			%thisRole = %team.role[%j];
			if(%thisRole.roleKnownTo(%role)){
				%role[%roleCount] = %thisRole.color @ %thisRole.name;
				%roleCount++;
			}
		}
	}
	%product = "";
	if(%roleCount > 1){
		for(%i = 0; %i < %roleCount; %i++){
			%product = %product @ %role[%i] @ "\c6";
			if(%roleCount > 2){
				if(%i + 1 < %roleCount){
					%product = %product @ ", ";
				} else{
					%product = %product @ ", and";
				}
			} else{
				if(%i + 1 < %roleCount){
				%product = %product @ " and ";
				}
			}
		}
		%this.chatMessage(%product SPC "can see your pre round chat but you can only see known player's messages (whisper/teamchat to talk)");
	} else{
		%this.chatMessage(%role[0] SPC "\c6can see your pre round chat but you can only see known player's messages (whisper/teamchat to talk)");
	}
	%this.setingameavatar();
	%this.player.setShapeNameDistance(0);
}

//sends all player's items to them to give a bit of breathing room at the start of rounds
function MinigameSO::SendItems(%this){
	if($SMMinigame::Debug){talk("Sending Items To Players");}
	%game = %this.gameGroup;
	for(%i = 0; %i < %game.getCount(); %i++){
		%client = %game.getObject(%i);
		if(isObject(%client) && !%client.inLobby){
			%c = 0;
			%role = %client.role;
			%player = %client.player;
			%player.setDatablock("playerFirstPerson");
			while(%role.getField("inv" @ %c) !$= ""){
				switch$(%role.getField("inv" @ %c)){
					case "gun":
						%player.addItem(%client.equippedGun,%client);
					case "zipTie":
						%player.addItem(zipTieItem,%client);
					case "badge":
						%player.addItem(badgeItem,%client);
					case "pilotGun":
						%player.addItem(servicePistolItem, %client);
					case "passport":
						%player.addItem(passportItem,%client);
						%player.setupPassport(%client.role.fakePass);
					case "fistItem":
						%player.addItem(fistItem, %client);
					case "clipper":
						%player.addItem(clipperItem, %client);
					default:
						%player.addItem(gunItem,%client);
				}
				%c++;
			}
		}
	}
	openBoxes();
}

//messages clients only in the gameGroup
function MinigameSO::messageGame(%this, %msgType, %msg){
	%game = SMMinigame;
	for(%i = 0; %i < %game.numMembers; %i++){
		%client = %game.member[%i];
		if(isObject(%client) && (!%client.inLobby || %client.spectating)){
			%client.playSound(%msgType);
			%client.ChatMessage(%msg);
		}
			
	}
}
//checks if a door is broken
function fxDTSBrick::repairDoorCheck(%this,%client){
	%player = %client.player;
	%pos = %player.currTool;
	%item = %player.tool[%pos];
	if(%item == nameToId("shovelItem") ||%item == nameToId("hockeyStickItem") || %item == nameToID("sledgeHammerItem")){
		%map = SMMinigame.currentmap;
		for(%i = 0; %i < %map.cabinBlockCount; %i++){
			%block = %map.getField("cabinBlock" @ %i);
			if(%block == %this.getID())
				break;
		}
		%door = %map.cabinDoor[%i];
		if(%door.cabinDoorHealth <= 0){
			%door.updateDoor(true);
			%this.disappear(0);
			%player.tool[%pos] = 0;
			messageClient (%client, 'MsgItemPickup', '', %pos, 0);
			ServerCmdUnUseTool(%client);
		}
		
	} else{
		%client.centerPrint("\c3You can use a long melee weapon to block the door", 2);
	}
}
function fxDTSBrick::updateDoor(%this, %repair){
	%map = SMMinigame.currentMap;
	if(%repair){
		%this.disappear("0");
		%this.door(4);
		%this.cabinDoorHealth = $SMMinigame::cabinDoorHealthStart;
		for(%i = 0; %i < %map.cabinDoorCount; %i++){
			%door = %map.cabinDoor[%i];
			if(%door == %this.getId()){
				break;
			}
		}
		%block = %map.getField("cabinBlock" @ %i);
		%block.disappear(0);
		if($SMMinigame::Debug){talk("Cabin Door Up	");}
	}else if(%this.cabinDoorHealth <= 0){
		logSomething("\c6A cabin door was broken down");
		logAdminSomething("m",0,"\c6A cabin door was broken down");
		%this.disappear("-1");
		%this.playSound("brickBreakSound");	
		if($SMMinigame::Debug){talk("Cabin Door Down");}
		for(%i = 0; %i < %map.cabinDoorCount; %i++){
			%door = %map.cabinDoor[%i];
			if(%door == %this.getId()){
				break;
			}
		}
		%block = %map.getField("cabinBlock" @ %i);
		%block.disappear(-1);
	}
}
function updateLight(%kill){
	%map = SMMinigame.currentMap;
	if(%kill){
		//removing player decals is neccessary
		logSomething("\c6Lights were knocked out");
		logAdminSomething("m",0,"\c6Lights were knocked out");
		if($SMMinigame::Debug){talk("Lights out");}
		
		Sun.sendUpdate();
		$lightsOn = false;
		SMMinigame.setEnviroment(true);
		for(%i = 0; %i < %map.lightCount; %i++){
			%map.light[%i].setLight(-1);	
		}
	} else{
		if($SMMinigame::Debug){talk("Lights on");}
		SMMinigame.setEnviroment(false);
		$lightsOn = true;
		for(%i = 0; %i < %map.lightCount; %i++){
			%map.light[%i].setLight(playerLight);
		}
	}
}
//stolen from additem event and fixed to work with the gamemode
function Player::addItem(%player,%image,%client)
{
	if(!isObject(%image))
		return;
   for(%i = 0; %i < %player.getDatablock().maxTools; %i++)
   {
		%tool = %player.tool[%i];
		if(%image $= "zipTieItem" && %tool == nameToId("zipTieItem")){
			%player.zipTies++;
			messageClient(%client,'MsgItemPickup');
			return;
		}
		if(isObject(%tool)){
			if(%tool == %image.getId())
			break;
		}
		if(!isObject(%tool))
		{
			%player.tool[%i] = %image.getId();
			%player.weaponCount++;
			messageClient(%client,'MsgItemPickup','',%i,%image.getId());
			if(%image $= "zipTieItem")
				%player.zipTies++;
			return;
		}
   }
	%player.dropItem(%image);
}
//effect when an electric box is hit
function fxDTSBrick::electricHitEffect(%this){
	%color = %this.colorID;
	%this.setEmitter("radioWaveExplosionEmitter");
	%this.setColor(3);
	%this.playsound("radioWaveExplosionSound");
	%this.schedule(200, setEmitter, "");
	%this.schedule(200, setColor, %color);
}
//effect when a door is hit
function fxDTSBrick::doorHitEffect(%this){
	%this.setEmitter("wrenchExplosionEmitter");
	%this.schedule(200, setEmitter, "");
}
//sets alitmeter in cabin to a number
function setAltimeter(%num){
	%map = SMMinigame.currentmap;
	%map.altimeter00.setPrintCount(%num % 10);
	%num = mFloor(%num / 10);
	%map.altimeter10.setPrintCount(%num % 10);
	%num = mFloor(%num / 10);
	%map.altimeter20.setPrintCount(%num % 10);
	%num = mFloor(%num / 10);
	%map.altimeter30.setPrintCount(%num % 10);
	%num = mFloor(%num / 10);
	%map.altimeter40.setPrintCount(%num % 10);
}
//drops tools
function GameConnection::dropAllTools(%this, %remove){
	%client = %this;
	%player = %client.player;
	for(%i = 0; %i < %player.getDatablock().maxTools; %i++){
		%item = %player.tool[%i];
		if(isObject(%item)){
			if(%item.getName() $= "fistItem")
				continue;
			if(%item.getName() $= "zipTieItem"){
				for(%j = 0; %j < %this.player.zipTies; %j++){
					%this.player.dropItem("zipTieItem");
				}
				if(%remove){
					%this.player.zipTies = 0;
				}
				
			}
			if(%item.getName() $= "passportItem" || %item.getName() $= "HolsteredItem"){
				continue;
			}
			%this.player.dropItem(%item);
		}
		if(%remove){
			%player.tool[%i] = 0;
			messageClient (%client, 'MsgItemPickup', '', %i, 0);
		}
	}
	if(%this.flashlight){
		%this.player.dropItem("flashlightItem");
		if(%remove){
			%this.player.flashlight = false;
		}
	}
}
//drops a specific item
function Player::dropItem(%this, %item){
	if(!%item.canDrop)
		return;
	%player = %this;
	%client = %this.client;
	%zScale = getWord (%player.getScale(), 2);
	%muzzlepoint = VectorAdd (%player.getPosition(), "0 0" SPC  1.5 * %zScale);
	%muzzlevector = %player.getEyeVector();
	%muzzlepoint = VectorAdd (%muzzlepoint, %muzzlevector);
	%playerRot = rotFromTransform ( %player.getTransform() );
	%thrownItem = new Item ()
	{
		dataBlock = %item;
	};
	%thrownItem.setScale ( %player.getScale() );
	MissionCleanup.add (%thrownItem);
	%thrownItem.setTransform (%muzzlepoint  @ " " @  %playerRot);
	%thrownItem.setVelocity ( %player.getVelocity());
	%thrownItem.schedulePop();
	%thrownItem.miniGame = %client.miniGame;
	%thrownItem.bl_id = %client.getBLID();
	%thrownItem.setCollisionTimeout (%player);
	if(%item.getname() $= "passportItem"){
		%thrownItem.passportFace = %this.passportFace;
		%thrownItem.passportColor = %this.passportColor;
	}
	return %item;
}
package smGamemode{
	function serverCmdStartTalking(%client){
		if(!isObject(%client.minigame)){
			return parent::serverCmdStartTalking(%client);
		}
	}
	function ProjectileData::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity){
		//subtract local door health if door collision
		if(("_" @ SMMinigame.currentMap @ "_CabinDoor") $= %col.getName()){
			if($SMMinigame::Debug){talk("Cabin Door Hit");}
			SMMinigame.logAdminSomething("m", %obj.sourceObject.client, "hit cabin door");
			%col.cabinDoorHealth -= %obj.dataBlock.directDamage;
			%col.doorhitEffect();
			%col.updateDoor();
			%obj.sourceObject.client.team.onDamageDoor(%obj.sourceObject.client,%obj.dataBlock.directDamage);
		}
		if(("_" @ SMMinigame.currentMap @ "_Elect") $= %col.getName()){
			if($SMMinigame::Debug){talk("Electrical Panel Hit");}
			SMMinigame.logAdminSomething("m", %obj.sourceObject.client, "hit electrical pannel");
			%obj.sourceObject.client.team.onDamageLight(%obj.sourceObject.client,%obj.dataBlock.directDamage);
			if($electricHealth <= 0){
				updateLight(1);
			} else{
				%col.electricHitEffect();
				$electricHealth -= %obj.dataBlock.directDamage;
			}
		}
		return parent::onCollision(%this, %obj, %col, %fade, %pos, %normal, %velocity);
	}
	function Armor::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType){
		SMMinigame.logAdminSomething("d", %obj.client, "damaged by" SPC %sourceObject.sourceObject.client);
		Parent::Damage(%data, %obj, %sourceObject, %position, %damage, %damageType);
	}
	function Item::schedulePop(%this){
		if($itemsDecay)
			return parent::schedulePop(%this);
		%this.schedule(2000, schedulePop, "");	
	}
	function serverCmdLight(%client){
		if(%client.inLobby && isObject(%client.minigame)){
			if(%client.currentSpectate == -1)
				%client.startSpectating();
			else
				%client.stopSpectating();
			return;
		}
		if(%client.player.flashlight || !isObject(%client.minigame)){
			return Parent::serverCmdLight(%client);
		}
		if(%client.player.getMountedImage(0).item.reload && %client.player.getMountedImage(0).item.maxMag != %client.player.toolMag[%client.player.currTool]){
			return Parent::serverCmdLight(%client);
		}
	}
	function serverCmdGreenLight(%client){
		return serverCmdLight(%client);
	}
	function Armor::onMount (%this, %obj, %vehicle, %node){
		if(%obj.getDatablock().getName() $= "passport")
			return;
		return parent::onMount (%this, %obj, %vehicle, %node);
	}
};

deactivatePackage("smGamemode");
activatePackage("smGamemode");
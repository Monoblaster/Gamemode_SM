exec("./extraResources.cs");
addExtraResource("Add-Ons/Gamemode_SM/items/custom/face.ifl");
function Player::startZipTie(%player,%restrain){
	if((%hitObj = ZipTieRaycast(%player,0)) > 0){
		%player.client.ProgressBar(900,10,0);
		%player.schedule(1000,finishZipTie,%restrain,%hitObj);
		return %hitObj;
	}
	return -1;
}
function Player::finishZipTie(%player,%restrain,%hitObj){
	if((%hitObj = ZipTieRaycast(%player,0)) == %hitObj && (%player.tool[%player.currTool] == nameToId("ZipTieItem") && %restrain) || (%player.tool[%player.currTool] == nameToId("ClipperItem") && !%restrain)){
		%hitObj.zipTie(%restrain,%player);
	} else
		%player.client.centerPrint((%restrain ? "Zip Tie" : "Clipper") SPC "missed",3);
}
function Player::zipTie(%this, %restrain, %sender){
	%playerRoleColor = %this.client.role.color;
	%playerRoleName = %this.client.role.name;
	%playerTeamColor = %this.client.team.color;
	%senderRoleColor = %sender.client.role.color;
	%senderRoleName = %sender.client.role.name;
	%senderTeamColor = %sender.client.team.color;
	if(%restrain && %this.getDatablock() != nameToID(ZipTied)){
		if(isObject(%this.client)){
			%this.client.centerPrint("\c3You have been restrained",3);
			%this.client.dropAllTools(true);
			%this.client.team.onRestrained(%this,%sender);
			%sender.client.team.onRestrain(%sender,%this);
		}
		SMMinigame.logSomething(%senderTeamColor @ %sender.client.getSimpleName() @ "\c7(" @ %senderRoleColor @ %senderRoleName @ "\c7) restrained " @ %playerTeamColor @ %this.client.getSimpleName() @ "\c7(" @ %playerRoleColor @ %playerRoleName @ "\c7)");
		%this.previousDatablockZT = %this.getDatablock();
		%this.setDatablock(ZipTied);
		%this.playthread(0,"sit");
		%this.playthread(1,"armreadyboth");
		if(%this.currTool != -1) {
			%this.updateArm(0);
			%this.unMountImage(0);
		}
		%sender.zipTies--;
		%sender.zipTieCheck();
		return true;
	} else if(!%restrain && %this.getDatablock() == nameToID(ZipTied)){
		if(isObject(%this.client)){
			%this.client.centerPrint("\c3You are no longer restrained",3);
			%this.client.onUnRestrained(%this,%sender);
			%sender.client.onUnRestrain(%sender,%this);
		}
		SMMinigame.logSomething(%senderTeamColor @ %sender.client.getSimpleName() @ "\c7(" @ %senderRoleColor @ %senderRoleName @ "\c7) unrestrained " @ %playerTeamColor @ %this.client.getSimpleName() @ "\c7(" @ %playerRoleColor @ %playerRoleName @ "\c7)");
		%this.setDatablock(%this.previousDatablockZT);
		%this.playThread(0,"root"); 
		%this.playThread(1,"root"); 
		return true;
	} else if(%restrain && %this.getDatablock() == nameToID(ZipTied)){
		if(isObject(%sender.client)){
			%sender.client.centerPrint("\c3They are already restrained",3);
		}
	} else if(!%restrain && %this.getDatablock() != nameToID(ZipTied)){
		if(isObject(%sender.client)){
			%sender.client.centerPrint("\c3They are not restrained",3);
		}
	}
	return false;
}
function Player::zipTieCheck(%this){
	if(%this.zipTies <= 0){
		%c = 0;
		while(%c < %this.getDatablock().maxTools){
			%item = %this.tool[%c];
			if(%item.uiName $= "Zip Tie"){
				messageClient(%this.client,'MsgItemPickup','',%c,0);
				%this.tool[%c] = 0;
				if(%this.currTool == %c) {
					%this.updateArm(0);
					%this.unMountImage(0);
				}
			}
			%c++;
		}
	}
}


function ZipTieRaycast(%player, %slot){
	%start = %player.getEyePoint();
	%muzzleVec = %player.getMuzzleVector (%slot);
	%muzzleVecZ = getWord (%muzzleVec, 2);
	%range = 2;
	%vec = VectorScale ( %muzzleVec, %range * getWord(%player.getScale(), 2) );
	%end = VectorAdd (%start, %vec);
	%mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType;
	%raycast = containerRayCast (%start, %end, %mask, %player);
	if ( !%raycast )
	{
		return;
	}
	%hitObj = getWord (%raycast, 0);
	if($TypeMasks::PlayerObjectType & %hitObj.getType())
		return %hitObj;
	return -1;
}
function Player::mountPassport(%this,%mount){
	if(%mount){
		%item = new aiPlayer(){
			datablock = "passport";
		};
		%invisBot = new aiPlayer(){
			datablock = "passport";
		};
		%invisBot.hideNode("backing");
		%invisBot.hideNode("paper");
		%invisBot.hideNode("face");
		%this.unMountImage(0);
		%item.setNodeColor("backing", "0.60 0.10 0 1");
		%item.setNodeColor("paper", "0.90 0.90 0.90 1");
		%item.setFaceName(%this.passportFace);
		%item.setNodeColor("face", %this.passportColor);
		%this.mountObject(%invisBot,0);
		%invisBot.mountObject(%item,0);
		%this.passport = %item;
		%this.passportHolder = %invisBot ;
		%this.playThread(0,"armReadyRight");
		//removing collision
		%item.kill();
		%invisBot.kill();
	} else if(%this.passport){
		%this.playThread(0,"root");
		
		%this.unMountObject(0);
		%this.passportHolder.unMountObject(0);
		%this.passport.delete();
		%this.passportHolder.delete();
		
	}
}
function Player::setupPassport(%this, %fake){
	if(%fake){
		%face = $SMMinigame::randomFace[getRandom(0, $SMMinigame::randomFaceNum - 1)];
		while(%face $= %this.client.faceName){
			%face = $SMMinigame::randomFace[getRandom(0, $SMMinigame::randomFaceNum - 1)];
		}
		%this.passportFace = %face;
		%this.passportColor = $SMMinigame::randomSkin[getRandom(0, $SMMinigame::randomSkinNum - 1)];
		%darken = getRandom() / 2.5;
		%darken = %darken SPC %darken SPC %darken;
		%passportColor = vectorSub(%passportColor, %darken) SPC 1;
	} else{
		%this.passportFace = %this.client.faceName;
		%this.passportColor = %this.client.headColor;		
	}
}

function GameConnection::ProgressBar(%client,%duration,%divisions,%progress){
	for(%i = 0; %i < %divisions; %i++){
		if(%i > %progress)
			%message = %message @ "\c7|";
		else
			%message = %message @ "|";
	}

	%client.centerPrint(%message, 1);
	
	if(%divisions > %progress)
		%client.schedule(%duration / %divisions, ProgressBar, %duration, %divisions, %progress + 1);
}

package smItems
{
   function serverCmdUseTool(%client,%slot)
   {
      %player = %client.player;
      if(!isObject(%player))
         return parent::serverCmdUseTool(%client,%slot);
      if(%player.getDatablock() == nameToID(ZipTied))
		return;
		if(isObject(%player.passport) && %player.tool[%player.currtool] !$= "passportItem"){
			%player.mountPassport(0);
		}
      parent::serverCmdUseTool(%client,%slot);
   }
   function servercmdRotateBrick(%client,%direction)
   {
      %player = %client.player;
      if(!isObject(%player))
         return Parent::servercmdRotateBrick(%client,%direction);
      if(%player.getDatablock() == nameToID(ZipTied))
         return;
      Parent::servercmdRotateBrick(%client,%direction);
   }
   function servercmdShiftBrick(%client,%x,%y,%z)
   {
      %player = %client.player;
      if(!isObject(%player))
         return Parent::servercmdShiftBrick(%client,%x,%y,%z);
      if(%player.getDatablock() == nameToID(ZipTied))
         return;
      Parent::servercmdShiftBrick(%client,%x,%y,%z);
   }
   function serverCmdUnUseTool(%client)
   {
      %player = %client.player;
      if(!isObject(%player))
         return Parent::serverCmdUnUseTool(%client);
      if(%player.getDatablock() == nameToID(ZipTied))
         return;
		if(isObject(%player.passport)){
			%player.mountPassport(0);
		}
      Parent::serverCmdUnUseTool(%client);
   }
   function servercmdActivateStuff(%client)
   {
       %player = %client.player;
      if(!isObject(%player))
         return Parent::servercmdActivateStuff(%client);
      if(%player.getDatablock() == nameToID(ZipTied))
         return;
      Parent::servercmdActivateStuff(%client);
   }
   function Player::ActivateStuff(%player)
   {
      if(%player.getDatablock() == nameToID(ZipTied))
         return;
      Parent::activateStuff(%player);
   }
	function Item::setThrower(%this, %newThrower){
		talk(%this SPC %newThrower);
		return parent::Respawn(%newThrower);
	}
	
	function serverCmdDropTool(%client, %position){
		%player = %client.player;
      if(!isObject(%player))
         return parent::serverCmdDropTool(%client,%slot);
      if(%player.getDatablock() == nameToID(ZipTied))
         return;
		%player = %client.player;
		if(%player.tool[%position] == nameToId(fistItem))
			return;	
		if(%player.tool[%position] == nameToID(zipTieItem)){
			%player.zipTies--;
			%client.throwATool(%player.tool[%position]);
			%player.zipTieCheck();
			return;
		}
		if(%player.tool[%position] == nameToID(passportItem)){
			return;
		}
		parent::serverCmdDropTool(%client, %position);
	}
	function GameConnection::throwATool(%this, %item){
			%player = %this.player;
			%client = %this;
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
			%thrownItem.setVelocity ( VectorScale(%muzzlevector, 20 * %zScale) );

			%thrownItem.schedulePop();

			%thrownItem.miniGame = %client.miniGame;
			%thrownItem.bl_id = %client.getBLID();
			%thrownItem.setCollisionTimeout (%player);
			return thrownItem;
	}
	function player::mountImage(%pl,%im,%slot)
	{
		if(!%pl.getDatablock().isEmptyPlayer)
		{
			if(%slot == 0)
			{
				if(%im $= "passportImage")
				{
					%pl.mountPassport(1);
					return;
				}
			}
		}
		return parent::mountImage(%pl,%im,%slot);
	}
	function player::unmountImage(%pl,%slot)
	{
		if(!%pl.getDatablock().isEmptyPlayer)
		{
			if(%slot == 0)
			{
				if(isObject(%pl.passport))
				{
					%pl.mountPassport(0);
					return;
				}
			}
		}
		return parent::unmountImage(%pl,%slot);
	}
	function Player::playDeathAnimation(%this){
		if(isObject(%this.passport)){
			%this.mountPassport(0);
		}
		return Parent::playDeathAnimation(%this);
	}
	function Armor::OnRemove(%db, %obj){
		if(%db.getName !$= "Passport")
			%obj.mountPassport(0);
		Parent::OnRemove(%db, %obj);
	}
	function Player::setDatablock(%player, %datablock){
		if(isObject(%player.passsport)){
			%client.player.mountPassport(0);
		}
		Parent::setDatablock(%player, %datablock);
	}
};
deactivatePackage("smItems");
activatePackage("smItems");
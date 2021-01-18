$SMMinigame::randomShirt[0] = "worm-sweater";
$SMMinigame::randomShirt[1] = "DrKleiner";
$SMMinigame::randomShirt[2] = "Hoodie";
$SMMinigame::randomShirt[3] = "Mod-Suit";
$SMMinigame::randomShirt[4] = "Meme-Mongler";
$SMMinigame::randomShirt[5] = "AAA-None";
$SMMinigame::randomShirtNum = 6;

$SMMinigame::randomFace[0] = "Smiley";
$SMMinigame::randomFace[1] = "SmileyCreepy";
$SMMinigame::randomFace[2] = "SmileyBlonde";
$SMMinigame::randomFace[3] = "KleinerSmiley";
$SMMinigame::randomFace[4] = "SmileyFemale1";
$SMMinigame::randomFace[5] = "SmileyEvil1";
$SMMinigame::randomFace[6] = "SmileyEvil2";
$SMMinigame::randomFaceNum = 7;

$SMMinigame::randomHat[0] = 0;
$SMMinigame::randomHat[1] = 4;
$SMMinigame::randomHat[2] = 7;
$SMMinigame::randomHatNum = 3;


$SMMinigame::randomChest[0] = 0;
$SMMinigame::randomChest[1] = 1;
$SMMinigame::randomChestNum = 2;

$SMMinigame::randomSkin[0] = "1 0.604 0.424 1";
$SMMinigame::randomSkin[1] = "1 0.878 0.612 1";
$SMMinigame::randomSkin[2] = "0.957 0.878 0.784 1";
$SMMinigame::randomSkinNum = 3;



function serverCmdSetName(%client, %name){
	%name = trim(stripMLcontrolChars(%name));
	if(!%client.inLobby)
		return;
	if(strLen(%name) > 20){
		return %client.chatMessage("\c3Name is too long");
	}
	if(strLen(%name) <= 0){
		return %client.chatMessage("\c3Name is too short");
	}
	%client.fakeNameSM = %name;
	%client.chatMessage("\c6Name set to \c3" @ %name);
	%client.nameCheck();
	%client.SMSave();

}
function GameConnection::setInGameAvatar(%this){
	%client = %this;
	if(!%client.gameAvatar){
		%client.smDecalName = %client.decalName;
		%client.smFaceName = %client.faceName;
		%client.smChest = %client.chest;
		%client.smHat = %client.hat;
		%client.smHip = %client.hip;
		%client.smLArm = %client.lArm;
		%client.smlHand = %client.LHand;
		%client.smlLeg = %client.lLeg;
		%client.smrHand = %client.rHand;
		%client.smrArm = %client.rArm;
		%client.smrLeg = %client.rLeg;
		%client.SMpack = %client.Pack;
		%client.smSecondPack = %client.secondPack;
	}
	%skinColor = $SMMinigame::randomSkin[getRandom(0, $SMMinigame::RandomSkinNum - 1)];
	%darken = getRandom() / 2.5;
	%darken = %darken SPC %darken SPC %darken;
	%skinColor = vectorSub(%skinColor, %darken) SPC 1;
	%shirtColor = getRandom() SPC getRandom() SPC getRandom() SPC 1;
	%shoeColor = getRandom()/4 SPC getRandom()/4 SPC getRandom()/4 SPC 1;
	%client.decalName = $SMMinigame::randomShirt[getRandom(0, $SMMinigame::RandomShirtNum - 1)];
	%client.faceName = $SMMinigame::randomFace[getRandom(0, $SMMinigame::RandomFaceNum - 1)];
	%client.chest = $SMMinigame::randomChest[getRandom(0, $SMMinigame::RandomChestNum - 1)];
	%client.hat = $SMMinigame::randomHat[getRandom(0, $SMMinigame::RandomHatNum - 1)];
	%client.hip = 0;
	%client.lArm = 0;
	%client.lHand = 0;
 	%client.lLeg = 0;
	%client.rArm = 0;
	%client.rHand = 0;
	%client.rLeg = 0;
	%client.pack = 0;
	%client.secondPack = 0;
	%hatColor = getRandom() SPC getRandom() SPC getRandom() SPC 1;
	%hipColor = getRandom()/2 SPC getRandom()/2 SPC getRandom()/2 SPC 1;
	if(isObject(%client.equippedSkin) && %client.equippedSkin !$= "DefaultSkin"){
		%skinColor = %client.equippedSkin.skinColor;
	}
	if(isObject(%client.equippedFace && %client.equippedSkin !$= "DefaultFace")){
		%client.faceName = %client.equippedFace.faceName;
	}
	if(isObject(%client.equippedShirt && %client.equippedSkin !$= "DefaultShirt")){
		%shirtColor = %client.equippedShirt.shirtColor;
		%client.chest = %client.equippedShirt.chest;
		%client.decalName = %client.equippedShirt.decalName;
	}
	if(isObject(%client.equippedPant && %client.equippedSkin !$= "DefaultPants")){
		%client.hipColor = %client.equippedPant.hipColor;
	}
	if(isObject(%client.equippedShoe && %client.equippedSkin !$= "DefaultShoes")){
		%client.shoeColor = %client.equippedShoe.shoeColor;
	}
	if(isObject(%client.equippedHat && %client.equippedSkin !$= "DefaultHat")){
		%client.hat = %client.equippedHat.hat;
		%hatColor = %client.equippedHat.hatColor;
	}
	if(!%client.gameAvatar){
		%client.smChestColor = %client.chestColor;
		%client.smlArmColor = %client.lArmColor;
		%client.smrArmCOlor = %client.rArmColor;
		%client.smheadColor = %client.headColor;
		%client.smlHandColor = %client.lHandColor;
		%client.smrHandColor = %client.rHandCOlor;
		%client.smlLegColor = %client.lLegColor;
		%client.smrLegColor = %client.rLegColor;
		%client.smhatColor = %client.hatColor;
		%client.smhipColor = %client.hipColor;
	}
	%client.applyBodyParts();
	%client.chestColor = %shirtColor;
	%client.lArmColor = %shirtColor;
	%client.rArmColor = %shirtColor;
	%client.headColor = %skinColor;
	%client.lHandColor = %skinColor;
	%client.rHandColor = %skinColor;
	%client.lLegColor = %shoeColor;
	%client.rLegColor = %shoeColor;
	%client.hatColor = %hatColor;
	%client.hipColor = %hipColor;
	%client.applyBodyColors();
	%client.gameAvatar = true;
}

function GameConnection::setDefaultAvatar(%this){
	if(!%this.gameAvatar)
		return;
	%client = %this;
	%client.decalName = %client.SMDecalName;
	%client.faceName = %client.SMfacename;
	%client.chest = %client.SMchest;
	%client.hat = %client.SMhat;
	%client.hip = %client.SMhip;
	%client.lArm = %client.SMlarm;
	%client.lHand = %client.SMlhand;
	%client.lLeg = %client.SMlleg;
	%client.rArm = %client.SMrarm;
	%client.rHand = %client.SMrhand;
	%client.rLeg = %client.SMrleg;
	%client.pack = %client.SMpack;
	%client.secondPack = %client.SMsecondpack;
	%client.chestColor = %client.SMchestcolor;
	%client.lArmColor = %client.SMlarmcolor;
	%client.lHandColor = %client.SMlhandcolor;
	%client.lLegColor = %client.SMllegcolor;
	%client.rArmColor = %client.SMrarmcolor;
	%client.rHandColor = %client.SMrhandcolor;
	%client.rLegColor = %client.SMrlegcolor;
	%client.hatColor = %client.SMhatcolor;
	%client.hipColor = %client.SMhipColor;
	%client.headColor = %client.SMheadColor;
	%client.applyBodyColors();
	%client.gameAvatar = false;
	%client.applyBodyParts();
}

function Player::getPlayerInfo(%this, %sender){
	%client = %this.client;
	%senderClient = %sender.client;
	if(%client.fakeNameSM $= "")
		%name = %client.getSimpleName();
	else
		%name = %client.fakeNameSM;
	%msg = "\c6This is \c3" @ %name @ "\n";
	if(%client.isKnownTo(%senderClient)){
		%role = %client.role.color @ %client.role.knownByName;
		%msg = %msg @ "\c6They are a " @ %role; 
	}
	return %msg;
}
function GameConnection::removeFakeName(%this, %realNameReason){
	if(!%this.inLobby){
		return %this.schedule(1000,"removeFakeName");
	}
	
	%this.setItem("fakeNameSM", "");
	if(%realNameReason){
		%this.chatMessage("\c3Someone's real name is the same as your in game name! Please choose a different one with /setName");
	} else{
		%this.chatMessage("\c3Someone's in game name is the same as your in game name! Please choose a different one with /setName");
	}
}
function GameConnection::nameCheck(%this){
	%name = %this.getSimpleName();
	%fakename = %this.fakeNameSM;
	if(%fakeName $= "")
	return;
	for(%i = 0; %i < ClientGroup.getCount(); %i++){
		%check = ClientGroup.getObject(%i);
		if(%this != %check){
			if(%name $= %check.fakeNameSM){
				%check.schedule(33,"removeFakeName", true);
			}
			if(%fakeName $= %check.getSimpleName()){
				%this.schedule(33,"removeFakeName", true);
			}
			if(%fakeName $= %check.fakeNameSM){
				%this.schedule(33,"removeFakeName", false);
			}
		}
	}
}
package smAppearance{
	function GameConnection::AddToJoinList(%this){
		if(%this.fakeNameSM $= "" && !$Server::SMMinigame::FakeNameNotification[%this.bl_id]){
			%this.chatMessage("\c3If you don't want to use your username in game try /setName name");
			$Server::SMMinigame::FakeNameNotification[%this.bl_id] = true;
		}
		return parent::AddToJoinList(%this);
	}
	function Player::activateStuff(%this){
		%start = %this.getEyePoint();

		%muzzleVec = %this.getMuzzleVector (%slot);
		%muzzleVecZ = getWord (%muzzleVec, 2);

	
	
		%range = 100;
	


		%vec = VectorScale ( %muzzleVec, %range * getWord(%this.getScale(), 2) );
		%end = VectorAdd (%start, %vec);

		%mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType;
		%raycast = containerRayCast (%start, %end, %mask, %this);
		if(%raycast != 0){
			if(getWord(%raycast, 0).getClassName() !$= "fxDTSBrick"  && !%this.client.inLobby){
				if($lightson || (!$lightsOn && isObject(%this.Light))){
					%hitObj = getWord (%raycast, 0);
					%this.client.centerPrint(%hitObj.getPlayerInfo(%this), 3);
				}else{
					%this.client.centerPrint("\c6It's too dark to see anything", 3);
				}
			}
		}
		return parent::activateStuff(%this);
	}
	
	function servercmdupdatebodycolors(%client, %headColor, %hatColor, %accentColor, %packColor, %secondPackColor, %chestColor, %hipColor, %LLegColor, %RLegColor, %LArmColor, %RArmColor, %LHandColor, %RHandColor, %decalName, %faceName){
		if(!%client.inLobby && %client.hasSpawnedOnce){
			return;
		}
		parent::servercmdupdatebodycolors(%client, %headColor, %hatColor, %accentColor, %packColor, %secondPackColor, %chestColor, %hipColor, %LLegColor, %RLegColor, %LArmColor, %RArmColor, %LHandColor, %RHandColor, %decalName, %faceName);
	}
	function servercmdupdatebodyparts(%client, %hat, %accent, %pack, %secondPack, %chest, %hip, %LLeg, %RLeg, %LArm, %RArm, %LHand, %RHand){
		if(!%client.inLobby && %client.hasSpawnedOnce){
			return;
		}
		parent::servercmdupdatebodyparts(%client, %hat, %accent, %pack, %secondPack, %chest, %hip, %LLeg, %RLeg, %LArm, %RArm, %LHand, %RHand);
	}
};
deactivatePackage("smAppearance");
activatePackage("smAppearance");	
%r = getRandom(0,1);
if(!$CreepyFlash){
$CreepyFlash = true;
if(%r == 0){
	exec("config/client/avatarfavorites/2.cs");
} else if(%r == 1){
	exec("config/client/avatarfavorites/3.cs");
}
} else{
	$CreepyFlash  = false;
	exec("config/client/avatarfavorites/1.cs");
}
clientCmdUpdatePrefs();
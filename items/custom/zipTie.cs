datablock AudioProfile(zipTieUseSound)
{
   filename    = "./zipTie.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock ItemData(zipTieItem:hammerItem)
{
	shapeFile = "./zipTie.dts";
	uiName = "Zip Tie";
	ColorShiftColor = "1 1 1 1";
	image = zipTieImage;
	iconName = "";
	
};

datablock ShapeBaseImageData(zipTieImage)
{
   // Basic Item properties
	shapeFile = "./zipTie.dts";
	showbricks=0;
	Projectile = "";
	ColorShiftColor = "1 1 1 1";
	
	emap = 1;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	correctMuzzleVector = 0;
	className = "WeaponImage";
	Item = zipTieItem;
	ammo = " ";
	projectileType = Projectile;
	melee = 1;
	doRetraction = 0;
	armReady = 1;
	doColorShift = 1;
	
	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0;
	stateTransitionOnTimeout[0] = "Ready";
   
	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = 1;
   
	stateName[2] = "Fire";
	stateTimeoutValue[2] = 0.01;
	stateAllowImageChange[2] = 1;
	stateScript[2] = "onFire";
	stateWaitForTimeout[2] = 1;
	stateTransitionOnTimeout[2] = "CheckFire";
	
	stateName[3] = "WaitForFire";
	stateTimeoutValue[3] = 0.01;
	stateWaitForTimeout[3] = 1;
	stateAllowImageChange[3] = 1;
	stateTransitionOnTimeout[3] = "CheckFire";
	
	stateName[4] = "CheckFire";
	stateTransitionOnTriggerUp[4] = "Ready";
	stateAllowImageChange[4] = 1;
	stateTransitionOnTriggerDown[4] = "WaitForFire";
};


function zipTieImage::onFire(%this, %obj, %slot){
	if($Sever::ZipTies::ZipTieCooldown[%obj.client.BL_ID] > $Sim::Time)
		return;
	%obj.playThread (2, activate);
	if((%hitObj = %obj.startZipTie(true)) > 0){
		if(%hitObj.getDatablock() != nameToID(ZipTied)){
			$Sever::ZipTies::ZipTieCooldown[%obj.client.BL_ID] = 2 + $Sim::Time;
			%obj.playAudio(0,zipTieUseSound);
		} else
			%obj.client.chatMessage("\c3They are already restrained");
	}
}


datablock PlayerData(ZipTied : PlayerFirstPerson){
	hName = "Zip Tied";
	jumpForce = 0;
	runForce = 4000;
	maxUnderwaterSideSpeed = 0;
	maxUnderwaterBackwardSpeed = 0;
	maxUnderwaterForwardSpeed = 0;
	maxSideSpeed = 0;
	maxBackwardSpeed = 0;
	maxForwardSpeed = 0;
	maxSideCrouchSpeed = 0;
	maxBackwardCrouchSpeed = 0;
	maxForwardCrouchSpeed = 0;
	crouchBoundingBox = "5 5 4";
	boudingBox = "5 5 4";
};
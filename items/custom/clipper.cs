datablock AudioProfile(clipperUseSound)
{
   filename    = "./clipper.wav";
   description = AudioClosest3d;
   preload = true;
};

datablock ItemData(clipperItem:hammerItem)
{
	shapeFile = "./clipper.dts";
	doColorShift = false;
	uiName = "Clipper";
	image = clipperImage;
	Projectile = "";
	iconName = "";
};

datablock ShapeBaseImageData(clipperImage)
{
   // Basic Item properties
  shapeFile = "./clipper.dts";
	showbricks=0;
	Projectile = "";
	ColorShiftColor = "1 1 1 1";
	
	emap = 1;
	mountPoint = 0;
	offset = "0 0 0";
	eyeOffset = "0 0 0";
	correctMuzzleVector = 0;
	className = "WeaponImage";
	Item = clipperItem;
	ammo = " ";
	projectileType = Projectile;
	melee = 1;
	doRetraction = 0;
	armReady = 1;
	doColorShift = 0;
	
	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0;
	stateTransitionOnTimeout[0] = "Ready";
   
	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Fire";
	stateAllowImageChange[1] = 1;
   
	stateName[2] = "Fire";
	stateTimeoutValue[2] = 0.01;
	stateSequence[2] = "Fire";
	stateScript[2] = "onFire";
	stateAllowImageChange[2] = 1;
	stateWaitForTimeout[2] = 1;
	stateTransitionOnTimeout[2] = "CheckFire";
	
	stateName[3] = "WaitForFire";
	stateTimeoutValue[3] = 0.01;
	stateWaitForTimeout[3] = 1;
	stateAllowImageChange[3] = 1;
	stateTransitionOnTimeout[3] = "CheckFire";
	
	stateName[4] = "CheckFire";
	stateTransitionOnTriggerUp[4] = "Ready";
	stateAllowImageChange[3] = 1;
	stateTransitionOnTriggerDown[4] = "WaitForFire";
};

function clipperImage::onFire(%this, %obj, %slot){
	if($Sever::ZipTies::ClipperCooldown[%obj.client.BL_ID] > $Sim::Time)
		return;
	%obj.playThread (2, activate);
	if((%hitObj = %obj.startZipTie(false)) > 0){
		if(%hitObj.getDatablock() == nameToID(ZipTied)){
			$Sever::ZipTies::ClipperCooldown[%obj.client.BL_ID] = 2 + $Sim::Time;
			%obj.playAudio(0,clipperUseSound);
		}
	}
}

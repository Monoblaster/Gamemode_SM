datablock ItemData(passportItem:HammerItem)
{
	shapeFile = "";
	doColorShift = false;
	uiName = "Passport";
	image = "passportImage";
	Projectile = "";
	iconName = "";
};

datablock ItemData(passportImage:HammerImage)
{
	shapeFile = "";
	doColorShift = false;
	showbricks=0;
	Projectile = "";
	stateSound[2] = "";
};

datablock PlayerData(passport : PlayerStandardArmor)
{
	shapeFile = "./Passport.dts";
	uiName = "";
	hName = "Passport";//cannot contain spaces
	boundingBox = "1 1 1";
};

function passport::onDisabled(%passport){}

function passportImage::onPreFire(%this, %obj, %slot){}

function passportImage::onFire(%this, %obj, %slot){}

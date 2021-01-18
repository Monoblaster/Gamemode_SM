datablock ItemData(badgeItem:HammerItem)
{
	shapeFile = "./badge.dts";
	doColorShift = true;
	uiName = "Badge";
	ColorShiftColor = "0.8 0.75 0.15 1";
	image = badgeImage;
	Projectile = "";
	iconName = "";
};

datablock ShapeBaseImageData(badgeImage:HammerImage)
{
   // Basic Item properties
   shapeFile = "./badge.dts";
   ColorShiftColor = "0.8 0.75 0.15 1";
   doColorShift = true;
   showbricks=0;
   Projectile = "";
   stateSound[2] = "";
};

function badgeImage::onPreFire(%this, %obj, %slot){}
function badgeImage::onFire(%this, %obj, %slot){}

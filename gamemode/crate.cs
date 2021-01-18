//scavenging stuff
//there is a finite ammount of each item for fun
//not every crate will have something
//weapons will mainly be found in crates
//zip ties and clippers mainly found in cabinets
registerOutputEvent(fxDTSBrick, getCrate);
registerOutputEvent(fxDTSBrick, getDrawer);
registerOutputEvent(fxDTSBrick, getSpawnCrate,"vector 40000 -40000 0");
registerOutputEvent(fxDTSBrick, getSpawnDrawer,"vector 40000 -40000 0");
$SMMinigame::crate[0] = "nothing" TAB 0.75;
$SMMinigame::crate[1] = "baseballBatItem" SPC "fryingPanItem" TAB 0.85;
$SMMinigame::crate[2] = "hockeyStickItem" SPC "shovelItem" SPC "meatCleaverItem" TAB 0.95;
$SMMinigame::crate[3] = "sledgeHammerItem" TAB 1;
$SMMinigame::drawer[0] = "nothing" TAB 0.80;
$SMMinigame::drawer[1] = "zipTieItem" SPC "clipperItem" TAB 0.95;
$SMMinigame::drawer[2] = "flashlight" TAB 1;
function openBoxes(){
//sets effects and everything for crates and drawers
	%map = SMMinigame.currentmap;
	for(%i = 0; %i < %map.crateCount; %i++){
		%box = %map.getField("crate" @ %i);
		%box.setRayCasting(true);
		%box.setEventEnabled(0,1);
		%box.setColorFX(4);
	}
	for(%i = 0; %i < %map.drawerCount; %i++){
		%box = %map.getField("drawer" @ %i);
		%box.setRayCasting(true);
		%box.setEventEnabled(0,1);
		%box.setColorFX(4);
	}
}
function closeBoxes(){
//sets effects and everything for crates and drawers
	%map = SMMinigame.currentmap;
	for(%i = 0; %i < %map.crateCount; %i++){
		%box = %map.getField("crate" @ %i);
		%box.setRayCasting(false);
		%box.setEventEnabled(0,0);
		%box.setColorFX(0);
	}
	%map = SMMinigame.currentmap;
	for(%i = 0; %i < %map.drawerCount; %i++){
		%box = %map.getField("drawer" @ %i);
		%box.setRayCasting(false);
		%box.setEventEnabled(0,0);
		%box.setColorFX(0);
	}
}
function setUpItems(){
	%map = SMMinigame.currentMap;
	//crates
	if(isObject(crates))
		crates.delete();
	%obj = new scriptObject(crates){
		crateCount = 0;
		drawerCount  = 0;
	};
	
	%itemCount = 0;
	for(%i = 1; %i <= %map.crateCount; %i++){
		%weight = %i / %map.crateCount;
		%c = 0;
		%group = $SMMinigame::crate[%c];
		while(%group !$= ""){
			if(%weight <= getField(%group, 1)){
				%field = getField(%group, 0);
				%item[%itemCount] = getWord(%field, getRandom(0,getWordCount(%field) - 1));
				%itemCount++;
				break;
			}
			%c++;
			%group = $SMMinigame::crate[%c];
		}
	}
	for(%i = %itemCount - 1; %i > 0; %i--){
		%r = getRandom(0, %i);
		%temp = %item[%i];
		%item[%i] = %item[%r];
		%item[%r] = %temp;
		%obj.setField("crate" @ %i, %item[%i]);
	}
	%obj.crate[0] = %item[0];
	%obj.crateCount = %itemCount;
	SMminigame.mapItems = %obj;
	//drawers
	%itemCount = 0;
	for(%i = 1; %i <= %map.drawerCount; %i++){
		%weight = %i / %map.drawerCount;
		%c = 0;
		%group = $SMMinigame::drawer[%c];
		while(%group !$= ""){
			if(%weight <= getField(%group, 1)){
				%field = getField(%group, 0);
				%item[%itemCount] = getWord(%field, getRandom(0,getWordCount(%field) - 1));
				%itemCount++;
				break;
			}
			%c++;
			%group = $SMMinigame::drawer[%c];
		}
	}
	for(%i = %itemCount - 1; %i > 0; %i--){
		%r = getRandom(0, %i);
		%temp = %item[%i];
		%item[%i] = %item[%r];
		%item[%r] = %temp;
		%obj.setField("drawer" @ %i, %item[%i]);
	}
	%obj.drawer[0] = %item[0];
	%obj.drawerCount = %itemCount;
	SMminigame.mapItems = %obj;
}
function getRandomCrate(){
	%obj = SMMinigame.mapItems;
	%thing = %obj.getField("crate" @ (%obj.crateCount - 1));
	%obj.crateCount = %obj.crateCount--;
	return %thing;
}
function getRandomDrawer(){
	%obj = SMMinigame.mapItems;
	%thing = %obj.getField("drawer" @ (%obj.drawerCount - 1));
	%obj.drawerCount = %obj.drawerCount--;
	return %thing;
}
function fxDTSBrick::getCrate(%this,%client){
	%this.setRayCasting(false);
	%this.setcolorFX(0);
	%r = getRandomCrate();
	if($SMMinigame::Debug){talk(%client.getSimpleName() @ " found " @ %r);}
	if(%r !$= "nothing"){
		%client.player.addItem(%r, %client);
		%client.centerPrint("\c3You found a \c4" @ %r.uiname, 2);
	}else
		%client.centerPrint("\c3You found nothing", 2);
}
function fxDTSBrick::getSpawnCrate(%this,%vec,%client){
	%r = getRandomCrate();
	if($SMMinigame::Debug){talk(%client.getSimpleName() @ " found " @ %r);}
	if(%r !$= "nothing")
		%this.spawnItem(%vec,%r);
}
function fxDTSBrick::getDrawer(%this,%client){
	%this.setRayCasting(false);
	%this.setcolorFX(0);
	%r = getRandomDrawer();
	if($SMMinigame::Debug){talk(%client.getSimpleName() @ " found " @ %r);}
	if(%r !$= "nothing" && %r !$= "flashlight"){
		%client.player.addItem(%r, %client);
		%client.centerPrint("\c3You found a \c4" @ %r.uiname, 2);
	}else if(%r $= "flashlight"){
		if(%client.player.flashlight)
			%client.player.dropItem("flashlightItem");
		%client.player.flashlight = true;
		%client.centerPrint("\c3You found a \c4Flashlight \c3(Press r)", 2);
	}else
		%client.centerPrint("\c3You found nothing", 2);
}
function fxDTSBrick::getSpawnDrawer(%this,%vec,%client){
	%r = getRandomDrawer();
	if($SMMinigame::Debug){talk(%client.getSimpleName() @ " found " @ %r);}
	if(%r $= "flashlight"){
		%this.spawnItem(%vec, flashLightItem);
	}else if(%r !$= "nothing")
		%this.spawnItem(%vec,%r);
}
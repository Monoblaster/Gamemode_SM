//what do we save?
//points
//stats
//hats these are auto saved by the hat addon so no worry there
//bought shirts, faces, and skin colors 
//bought guns
$SMFileRoute = "config/SM/save/";

package smSaving{
	function GameConnection::autoAdminCheck(%this){
		%blid = %this.getBLID();
		if(isFile($SMFileRoute @ %blid @ ".txt")){
			%this.SMLoad();
			%this.nameCheck();
		} else{
			%this.SMDefaultSave();
			%this.SmSave();
		}
		return parent::autoAdminCheck(%this);
	}
};
deactivatePackage("smSaving");
activatePackage("smSaving");
function GameConnection::SMLoad(%this){
	%blid = %this.getBLID();
	%file = new fileObject(){};
	%file.openForRead($SMFileRoute @ %blid @ ".txt");
	//puts everything into variables
	%this.score = %file.readLine();
	%stats = %file.readLine();
	%cosmetics = %file.readLine();
	%Guns = %file.readLine();
	%this.fakeNameSM = %file.readLine();
	%outfit = %file.readLine();
	%this.EquippedGun = %file.readLine();
	%this.tutorialDone = %file.readLine();
	%file.close();
	%file.delete();
	echo("Loading " @ %this.getSimpleName());
	//loads stats
	%this.SMStatCount = 0;
	for(%i = 0; %i < getFieldCount(%stats); %i++){
		%field = getField(%stats, %i);
		%this.SMStat[%i] = %field;
		%this.SMStatCount++;
	}
	//loads cosmetics
	%this.SMCosmeticsCount = 0;
	for(%i = 0; %i < getFieldCount(%cosmetics); %i++){
		%field = getField(%cosmetics, %i);
		%this.SMCosmetics[%i] = %field;
		%this.SMCosmeticsCount++;
	}
	//loads guns
	%this.SMgunCount = 0;
	for(%i = 0; %i < getFieldCount(%guns); %i++){
		%field = getField(%guns, %i);
		%this.SMGun[%i] = %field;
		%this.SMGunCount++;
	}
	//loads outfit
	//skin
	//faces
	//shirt
	//pants
	//shoe
	//Hat
	%this.EquippedSkin = getField(%Outfit, 0);
	%this.EquippedFace = getField(%Outfit, 1);
	%this.EquippedShirt = getField(%Outfit, 2);
	%this.EquippedPant = getField(%Outfit, 3);
	%this.EquippedShoe = getField(%Outfit, 4);
	%this.EquippedHat = getField(%Outfit, 5);
}
function GameConnection::SMSave(%this){
	if($Server::SMMinigame:IsSaving)
		%this.schedule(1000,SMSave);
	if(!isObject(%this))
		return;
	$Server::SMMinigame:IsSaving = true;
	%blid = %this.getBLID();
	%file = new fileObject(){};
	%file.openForWrite($SMFileRoute @ %blid @ ".txt");
	echo("saving " @ %this.getSimpleName());
	//saves stats
	for(%i = 0; %i < %this.SMStatCount; %i++){
		%stats = %stats TAB %this.SMStat[%i];
	}
	//saves cosemtics
	if(isObject(%this.SMCosmetics[0]))
		%cosmetics = %this.SMcosmetics[0].getName();
	for(%i = 1; %i < %this.SMCosmeticsCount; %i++){
		if(isObject(%this.SMCosmetics[%i]))
			%SMCosmetics[%i] = %this.SMCosmetics[%i].getName();
		%cosmetics = %cosmetics TAB %SMCosmetics[%i];
	}
	//saves guns
	if(isObject(%this.SMgun[0]))
		%guns = %this.SMgun[0].getName();
	for(%i = 1; %i < %this.SMGunCount; %i++){
		if(isObject(%this.SMgun[%i]))
			%SMGun[%i] = %this.SMGun[%i].getName();
		%gun = %gun TAB %SMGun[%i];
	}
	%file.writeLine(%this.score);
	%file.writeLine(%stats);
	%file.writeLine(%cosmetics);
	%file.writeLine(%guns);
	%file.writeLine(%this.fakeNameSM);
	%file.writeLine(%this.EquippedSkin TAB %this.EquippedFace TAB %this.EquippedShirt TAB %this.EquippedPant TAB %this.EquippedShoe TAB %this.EquippedHat);
	%file.writeLine(%this.EquippedGun);
	%file.writeLine(%this.tutorialDone);
	%file.close();
	%file.delete();
	$Server::SMMinigame:IsSaving = false;
}
function GameCOnnection::SMDefaultSave(%this){
	%this.SMcosmetics[0] = "DefaultSkin" ;
	%this.SMcosmetics[1] = "DefaultFace" ;
	%this.SMcosmetics[2] = "DefaultShirt" ;
	%this.SMcosmetics[3] = "DefaultPants" ;
	%this.SMcosmetics[4] = "DefaultShoes" ;
	%this.SMcosmetics[5] = "DefaultHat" ;
	%this.SMCosmeticsCount = 6;
	%this.EquippedSkin = "DefaultSkin";
	%this.EquippedFace = "DefaultFace";
	%this.EquippedShirt = "DefaultShirt";
	%this.EquippedPant = "DefaultPants";
	%this.EquippedShoe = "DefaultShoes";
	%this.EquippedHat = "DefaultHat";
	%this.SMgun[0] = "revolverItem";
	%this.SMgunCount = 2;
	%this.EquippedGun = "revolverItem";
	%this.tutorialDone = false;
	%this.fakeNameSM = "";
	%this.SMSave();
}
function GameConnection::addItem(%this,%field,%data){
	%count = %field @ "Count";
	if(%this.getField(%count) !$= ""){
		echo("adding " @ %field SPC %data @ " to " @ %this.getSimpleName());
		%this.setField(%field @ %this.getField(%count), %data);
		%this.setField(%count, %this.getField(%count) + 1);
		%this.SMSave();
	}
}
function GameConnection::setItem(%this, %field, %data){
	echo("setting " @ %field SPC %data @ " for " @ %this.getSimpleName());
	%this.setField(%field, %data);
	%this.SMSave();
}
function GameConnection::hasItem(%this, %field, %data){
	%count = %field @ "Count";
	if(%this.getField(%count) !$= ""){
		for(%i = 0; %i < %this.getField(%count); %i++){
			%item = %this.getField(%field @ %i);
			if(%item $= %data){
				return true;
			}
		}
	} else if(%this.getField(%field) !$= ""){
		%item = %this.getField(%field);
			if(%item $= %data){
				return true;
			}
	}
	return false;
}

function serverLoadShop(){
	if(isObject(SMShop)){
		SMShop.delete();
	}
	%shop = new scriptObject(SMShop){	
		class = "Shop";
	};
	MissionCleanup.add(%shop);
	//ui name
	//shop list
	//inventory list
	//objects hold data for the shop
	%shop.catagoryCount = 7;
	
	
	
	%shop.catagory[0] = "Guns" TAB "Gun" TAB "smGun";
	
	%shop.GunCount = -1;
	%shop.gun[%shop.GunCount++] = huntingMagnumItem TAB 0 TAB "Big Iron";
	%shop.gun[%shop.GunCount++] = gamePistolItem TAB 0 TAB "Your basic pistol";
	%shop.gun[%shop.GunCount++] = automaticPistolItem TAB 0 TAB "Faster gun";
	%shop.gun[%shop.GunCount++] = taserItem TAB 0 TAB "Zap";
	%shop.gun[%shop.GunCount++] = revolverItem TAB 0 TAB "default gun";
	%shop.gun[%shop.GunCount++] = machinePistolItem TAB 0 TAB "Even faster gun";
	%shop.gun[%shop.GunCount++] = servicePistolItem TAB 0 TAB "Fast?";
	
	%shop.catagory[1] = "Skins" TAB "skin" TAB "smCosmetics";
	
	%shop.skinCount = 0;
	newShopItemObject("VampireSkin", "Vampire", 
	"skinColor" SPC "1 1 1 1"
	, 1, 0, "Makes you look very pale");
	
	%shop.catagory[2] = "Faces" TAB "face" TAB "smCosmetics";
	
	%shop.faceCount = 0;
	newShopItemObject("MonglerFace", "Mongler", 
	"faceName" SPC "memeBlockMongler"
	, 2, 0, ":)");
	
	%shop.catagory[3] = "Shirts" TAB "shirt" TAB "smCosmetics";
	
	%shop.shirtCount = 0;
	newShopItemObject("PrisonerShirt", "Prisoner", 
	"shirtColor" SPC "1 0.61 0 1" 
	TAB "chest" SPC "0"
	TAB "decalName" SPC "Mod-Prisoner"
	, 3, 0, "Ever wanted to be a volentary prisoner?");
	newShopItemObject("PilotShirt", "Pilot", 
	"shirtColor" SPC "0 0 0.50 1" 
	TAB "chest" SPC "0"
	TAB "decalName" SPC "Mod-Pilot"
	, 3, 0, "Aiport souvenir shops are great");
	
	%shop.catagory[4] = "Pants" TAB "pant" TAB "smCosmetics";
	
	%shop.pantCount = 0;
	newShopItemObject("BluePants", "Jeans", 
	"hipColor" SPC "0.25 0.25 1 1"
	, 4, 0, "Nice cozy jeans");
	
	%shop.catagory[5] = "Shoes" TAB "shoe" TAB "smCosmetics";
	
	%shop.shoeCount = 0;
	newShopItemObject("GreenShoes", "Green Snakeskin Boots", 
	"shoeColor" SPC "0 1 0 1"
	, 5, 0, "Who said snake skin wasn't stylish");
	
	%shop.catagory[6] = "Hats" TAB "hat" TAB "smCosmetics";
	
	%shop.hatCount = 0;
	newShopItemObject("NiceHat", "Nice Hat", 
	"hatColor" SPC "0 0 0 1"
	TAB "hat" SPC "1"
	, 6, 0, "Idk just a test item");
	
	//these are only availible in the equipping screen
	%shop.HiddenSkinCount = 0;
	newShopItemObject("DefaultSkin", "Random", "" , 1, 0, "Your Default Item", 1);
	%shop.HiddenFaceCount = 0;
	newShopItemObject("DefaultFace", "Random", "" , 2, 0, "Your Default Item", 1);
	%shop.HiddenShirtCount = 0;
	newShopItemObject("DefaultShirt", "Random", "" , 3, 0, "Your Default Item", 1);
	%shop.HiddenPantCount = 0;
	newShopItemObject("DefaultPants", "Random", "" , 4, 0, "Your Default Item", 1);
	%shop.HiddenShoeCount = 0;
	newShopItemObject("DefaultShoes", "Random", "" , 5, 0, "Your Default Item", 1);
	%shop.HiddenHatCount = 0;
	newShopItemObject("DefaultHat", "Random", "" , 6, 0, "Your Default Item", 1);
}

function newShopItemObject(%name, %uiName, %data, %catagory, %price, %desc, %hide){
	if(isObject(%name))
		return addShopItemObject(%name.getName(), %catagory, %price, %desc, %hide);
	%obj = new scriptObject(%name){
		class = "ShopObject";
		uiName = %uiName;
	};
	for(%i = 0; %i < getFieldCount(%data); %i++){
		%str = getField(%data,%i);
		%obj.setField(getWord(%str,0),getWords(%str,1,strLen(%str)));
	}
	addShopItemObject(%obj.getName(), %catagory, %price, %desc, %hide);
}
function addShopItemObject(%obj, %catagory, %price, %desc, %hide){
	%catagoryname = getField(SMShop.getField("catagory" @ %catagory), 1);
	if(!%hide){
		SMShop.setField(%catagoryname @ SMShop.getField(%catagoryname @ count), %obj TAB %price TAB %desc);
		SMShop.setField(%catagoryname @ count, SMShop.getField(%catagoryname @ count) + 1);
	} else{
		SMShop.setField("Hidden" @ %catagoryName @ SMShop.getField("Hidden" @ %catagoryName @ count), %obj TAB %price TAB %desc);
		SMShop.setField("Hidden" @ %catagoryname @ count, SMShop.getField("Hidden" @ %catagoryname @ count) + 1);
	}
}
function serverCmdShop(%client, %catagory, %number){
	if(0 < %catagory && %catagory <= SMshop.catagoryCount){
		%tab = SMShop.getField("catagory" @(%catagory - 1));
		%tabCount = SMShop.getField(getField(%tab,1) @ Count);
		if(%number !$= ""){
				//buying
				if(0 < %number && %number <= %tabCount){
					%item = SMShop.getField(getField(%tab,1) @ %number - 1);
					%client.SMpurchasing = %item TAB getField(%tab,2);
					%client.chatMessage("\c6Would you like to purchase \c3" @ getField(%item, 0).uiName @ "\c6 for \c4" @ getField(%item, 1) @ " points\c6?");
					%client.chatMessage("\c3 /Yes \c6or \c3/No");
					return;
				}
				serverCmdShop(%client, %catagory, "");
				return;
		}
		if(%tab !$= ""){
			%client.chatMessage("\c2" @  getField(%tab, 0));
			for(%i = 0; %i < %tabCount; %i++){
				%item = SMShop.getField(getField(%tab,1) @ %i);
				%client.chatMessage("\c5" @ (%i + 1) @ " \c6| \c3" @ getField(%item, 0).uiName @ "\c4 " @ getField(%item, 2) @ " \c6costs: \c4" @ getField(%item, 1));
			}
		}
		return;
	}
	%client.chatMessage("\c2Welcome to the shop! \c3/shop \c4catagory# \c2item#");
	for(%i = 0; %i < SMShop.catagoryCount; %i++){
		%catagory = SMShop.catagory[%i];
		%client.chatMessage("\c5" @ (%i + 1) @ " \c6| \c3" @ getField(%catagory, %tab @ Count));
	}
}
function serverCmdYes(%client){
	if(%client.SMpurchasing !$= ""){
		%item = %client.smPurchasing;
		%price = getField(%item, 1);
		%name = getField(%item, 0).uiname;
		if(%client.hasItem(getField(%item, 3),getField(%item, 0)))
			%client.chatMessage("\c3You already own this item");
		else if(%client.score >= %price){
			%client.score -= %price;
			%client.chatMessage("\c6You have purchased \c3" @ %name @ "\c6 for \c4" @ %price @ " points\c6! Use \c4/equip\c6 to equip it");
			//add inventory and stuff
			%client.addItem(getField(%item, 3), getField(%item, 0).getName());

		} else
			%client.chatMessage("\c3You don't have enough points");
		%client.smPurchasing = "";
		return;
	}
	%client.chatMessage("\c3You have no items to buy");
}
function serverCmdNo(%client){
	if(%client.SMpurchasing !$= ""){
		%client.chatMessage("\c6Denied");
		%client.smPurchasing = "";
		return;
	}
	%client.chatMessage("\c3You have no items to deny");
}

function isInShopCatagory(%catagory, %item){
	%catagoryName = getField(SMShop.catagory[%catagory],1);
	%catagorysCount = SMShop.getField(%catagoryName @ "count");
	for(%i = 0; %i < %catagorysCount; %i++){
		%shopItem = getField(SMShop.getField(%catagoryName @ %i), 0);
		if(%shopItem $= %item)
			return true;
	}
	//hidden catagory
	%catagoryName = "Hidden" @ getField(SMShop.catagory[%catagory],1);
	%catagorysCount = SMShop.getField(%catagoryName @ "count");
	for(%i = 0; %i < %catagorysCount; %i++){
		%shopItem = getField(SMShop.getField(%catagoryName @ %i), 0);
		if(%shopItem $= %item)
			return true;
	}
	return false;
}

function serverCmdEquip(%client, %field, %item){
	if(!%client.inLobby){
		%client.chatMessage("\c3You cannot equip new things during the game");
		return;
	}
	if(%field $= ""){
		//check if we have something equipped, if not show none as the item;
		%client.chatMessage("\c2Equipment \c3/equip \c4catagory# \c2item#");
		for(%i = 0; %i < SMShop.catagorycount; %i++){
			%catagory = SMShop.getField("catagory" @ %i);
			%equipped = %client.getField("equipped" @ getField(%catagory, 1));
			%catagoryname = getSubStr(getField(%catagory, 0), 0, strLen(getField(%catagory, 0)) - 1);
			%client.chatMessage("\c5" @ %i + 1 @ "\c6 | \c3" @ "Equipped " @ %catagoryname @ ": \c4" @ (isObject(%equipped) ? %equipped.uiName : "None"));
		}
		return;
	} else{
		if(0 >= %field || %field > SMshop.catagorycount + 1){
			serverCmdEquip(%client, "", "");
			return;
		}
		%catagory = SMShop.getField("catagory" @ %field - 1);
		%catagoryname = getField(%catagory, 2);
		%itemCount = %client.getField(%catagoryname @ "count");
		%inventory = getField(%catagory, 1);
		%equipped = %client.getField("equipped" @ %inventory).getName();
		if(%item $= ""){
			if(%field <= %itemCount && %field > 0){
				%client.chatMessage("\c2" @ getField(%catagory, 0));
				%c = 0;
				for(%i = 0; %i < %itemCount; %i++){
					%itemName = %client.getField(%catagoryname @ %i);
					if(!isInShopCatagory(%field - 1, %itemName))
						continue;
				
					if(%itemname $= %equipped)
						%client.chatMessage("\c2" @ %c + 1 @ "\c6 ] \c4" @ %itemName.uiName);
					else
						%client.chatMessage("\c5" @ %c + 1 @ "\c6 | \c3" @ %itemName.uiName);
					%c++;
				}
			} else
				serverCmdEquip(%client,"", "");
		} else{
			%c = 0;
			%c2 = 0;
			while(%c != %item && %c2 < %itemCount){
				%itemName = %client.getField(%catagoryname @ %c2);
				if(isInShopCatagory(%field - 1, %itemName))
					%c++;
				%c2++;
			}
			if(%c == %item){
				%client.setItem("equipped" @ %inventory, %itemName);
				%client.chatMessage("\c3Equipping \c4" @ %itemName.uiName);
				return;
			}
			serverCmdEquip(%client, %field, "");
		}
	}
}
serverLoadShop();
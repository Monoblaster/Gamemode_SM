package SM_Inventory{
	function ServerCmdUseTool(%client, %slot){
		if(isObject(%client.minigame) && !%client.equip ){
			%client.player.currhiddentool = %slot;
			if(%client.minigame.isMember(%client)){
				%client.player.currhiddentool = %slot;
			}
		} else{
			Parent::ServerCmdUseTool(%client, %slot);
		}
	}
	function ServerCmdUnUseTool(%client){
		%client.player.currhiddentool = -1;
		Parent::ServerCmdUnUseTool(%client);
	}
	function Armor::onTrigger(%this, %obj, %triggerNum, %val){
		%client = %obj.client;
		if(isObject(%client.minigame)){
			if(%client.minigame.isMember(%client)){
				if(%triggerNum == 4 && %val == 1 && %client.player.currhiddentool != -1){
					%client.equip = true;
					ServerCmdUseTool(%client, %client.player.currhiddentool);
					%client.equip = false;
				}
			}
		}
		Parent::onTrigger(%this, %obj, %triggerNum, %val);
	}
};

deactivatePackage("SM_Inventory");
activatePackage("SM_Inventory");
if(!$RTB::Hooks::ServerControl)
{
	if($Pref::Server::CTPU::NotFirstExecution $= "")
	{
		$Pref::Server::CTPU::NotFirstExecution = 1;
		$Pref::Server::CTPU::Toggle = 1;
		$Pref::Server::CTPU::Range = 3;
		$Pref::Server::CTPU::Block = 0;
		$Pref::Server::CTPU::Copy = 1;
	}
}
else
{
	$Pref::Server::CTPU::NotFirstExecution = 1;
	RTB_registerPref("Click to Pickup Items","Click to Pickup","Pref::Server::CTPU::Toggle",bool,"CTPU",1,0,0);
	RTB_registerPref("Item Pickup Range","Click to Pickup","Pref::Server::CTPU::Range","string 3","CTPU","3",0,0);
	RTB_registerPref("Players Can Block","Click to Pickup","Pref::Server::CTPU::Block",bool,"CTPU",0,0,0);
	RTB_registerPref("Can Pickup Copies","Click to Pickup","Pref::Server::CTPU::Copy",bool,"CTPU",1,0,0);
}

package ClickToPickup
{
	function armor::onCollision(%data,%this,%col,%vec,%vel)
	{
		if(%col.getClassName() $= "Item" && $Pref::Server::CTPU::Toggle)
		{
			if(!%this.client.messagedAboutCTPU)
			{
				%this.client.messagedAboutCTPU = 1;
			}
			return 0;
		}
		return Parent::onCollision(%data,%this,%col,%vec,%vel);
	}
	function armor::onTrigger(%data,%this,%slot,%state)
	{
		%ret = Parent::onTrigger(%data,%this,%slot,%state);
		if(%slot == 0 && %state == 1 && $Pref::Server::CTPU::Toggle && %this.getMountedImage(0) == 0 && %this.getDatablock() != nameToID("zipTied"))
		{
			if($Pref::Server::CTPU::Range > 20)
				$Pref::Server::CTPU::Range = 20;
			else if($Pref::Server::CTPU::Range < 0)
				$Pref::Server::CTPU::Range = 0;
			else if($Pref::Server::CTPU::Range $= "default")
				$Pref::Server::CTPU::Range = 2.5;
			%start = %this.getEyePoint();
			%end = vectorScale(%this.getEyeVector(), $Pref::Server::CTPU::Range*getWord(%this.getScale(),2));
			%mask = $TypeMasks::FxBrickObjectType | $TypeMasks::InteriorObjectType | $TypeMasks::TerrainObjectType | $TypeMasks::ItemObjectType;
			if($Pref::Server::CTPU::Block)
				%mask = %mask | $TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType;
			%rayCast = containerRayCast(%start, vectorAdd(%start, %end), %mask, %this);
			%target = firstWord(%rayCast);
			if(!isObject(%target))
				return;
			if(%target.getClassName() $= "Item" && miniGameCanUse(%this,%target))
			{
				if(%target.dataBlock.getName() $= "flashlightItem"){
					if(%target.dataBlock.getId() == nameToID("flashlightItem") && !%this.flashLight)
						%target.delete();
					%this.flashLight = true;
					return;
				}
				if(%target.dataBlock.getName() $= "zipTieItem"){
					%this.zipTies++;
				}

				%copy = 0;
				for(%x=0;%x<%data.maxTools;%x++){
					if(!isObject(%this.tool[%x]))
						continue;
					if(%this.tool[%x].getID() == %target.dataBlock.getID())
					{
						%copy = 1;
						break;
					}
				}
				if(%copy && %target.dataBlock.getName() $= "zipTieItem"){
					messageClient(%this.client,'MsgItemPickup');
					%target.delete();
				}
				if(%target.canPickup)
					%this.pickup(%target);
				
			}
		}
		return %ret;
	}
};
deactivatePackage(ClickToPickup);
activatePackage(ClickToPickup);
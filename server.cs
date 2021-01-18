exec("Add-Ons/Gamemode_SM/gamemode/core.cs");
exec("Add-Ons/Gamemode_SM/gamemode/crate.cs");
exec("Add-Ons/Gamemode_SM/gamemode/roleList.cs");
exec("Add-Ons/Gamemode_SM/gamemode/winning.cs");
exec("Add-Ons/Gamemode_SM/gamemode/appearance.cs");
exec("Add-Ons/Gamemode_SM/gamemode/chat.cs");
exec("Add-Ons/Gamemode_SM/gamemode/gamemode.cs");
exec("Add-Ons/Gamemode_SM/gamemode/spectating.cs");
exec("Add-Ons/Gamemode_SM/gamemode/inventory.cs");
exec("Add-Ons/Gamemode_SM/extras/ban.cs");
exec("Add-Ons/Gamemode_SM/extras/support.cs");
exec("Add-Ons/Gamemode_SM/extras/vote.cs");
exec("Add-Ons/Gamemode_SM/extras/save.cs");
exec("Add-Ons/Gamemode_SM/extras/shop.cs");
exec("Add-Ons/Gamemode_SM/extras/tutorial.cs");
exec("Add-Ons/Gamemode_SM/extras/lobbyGame.cs");
exec("Add-Ons/Gamemode_sm/items/runner.cs");
exec("Add-Ons/Gamemode_sm/extras/player_firstPerson/server.cs");
exec("Add-Ons/Gamemode_sm/extras/script_clicktopickup/server.cs");
exec("Add-Ons/Gamemode_sm/extras/script_peggfootsteps/server.cs");


package SM_Start{
	function ServerLoadSaveFile_End(){
		parent::serverLoadSaveFile_End();
		exec("Add-Ons/Gamemode_SM/init.cs");
		server_Init();	
	}
};
activatePackage("SM_Start");
schedule(5000, 0, serverDirectSaveFileLoad, "Add-Ons/Gamemode_SM/save.bls", 3, "Add-Ons", 2, true);
$Server::SMMinigame::GameBanCount = 0;
function serverCmdGameBan(%client,%name,%rounds){
	if(!%client.isAdmin && !%client.isSuperAdmin)
		return;
	%banClient = findclientbyname(%name);
	if(!isObject(%banClient)){
		%client.chatMessage("\c3" @ %name @ " is not a client's name");
		return;
	}
	if(%rounds < 0)
		%rounds = 0;
	//search and see if they have an active ban
	%activeBan = false;
	for(%i = 0; %i < $Server::SMMinigame::GameBanCount; %i++){
		if($Server::SMMinigame::GameBanID[%i] == %banCLient.BL_ID && $Server::SMMinigame::GameBan[$Server::SMMinigame::GameBanID[%i]] > $Server::SMMinigame::RoundNumber)
			%activeBan = true;
	}
	if(!%activeBan){
		//you can't start a ban at 0 rounds
		if(%rounds == 0){
			%client.chatMessage("\c3You can't start a game ban at 0 rounds.");
			return;
		}
		$Server::SMMinigame::GameBanID[$Server::SMMinigame::GameBanCount] = %banCLient.BL_ID;
		$Server::SMMinigame::GameBanCount++;
		MessageAll('MsgAdminForce', "\c2" @ %banClient.name SPC "has been game banned for" SPC %rounds SPC "rounds by" SPC %client.name @ ".");
	} else{
		if(%rounds == 0)
			MessageAll('MsgAdminForce', "\c2" @ %banClient.name SPC "has been un game banned by" SPC %client.name @".");
		else
			MessageAll('MsgAdminForce', "\c2" @ %banClient.name @ "'s game ban has been changed to" SPC %rounds SPC "rounds from now by" SPC %client.name @ ".");
	}
	$Server::SMMinigame::GameBan[%banCLient.BL_ID] = %rounds + $Server::SMMinigame::RoundNumber;
	if(SMMInigame.joinGroup.isMember(%banClient)){
		%banClient.RemoveFromjoinList();
		%banClient.AddToJoinList();
	}
}

function serverCmdGameBans(%client){
	if(!%client.isAdmin && !%client.isSuperAdmin)
		return;
	%printedBan = false;
	%client.chatMessage("\c2Game Bans");
	for(%i = 0; %i < $Server::SMMinigame::GameBanCount; %i++){
		%bannedBL_ID = $Server::SMMinigame::GameBanID[%i];
		%bannedClient = findClientByBL_ID(%bannedBL_ID);
		%bannedRounds = $Server::SMMinigame::GameBan[%bannedBL_ID];
		if(%bannedRounds > $Server::SMMinigame::RoundNumber && isObject(%bannedClient)){
			%client.chatMessage("\c4" @ %bannedClient.name @ "\c6 is game banned for \c3" @ (%bannedRounds - $Server::SMMinigame::RoundNumber) @ "\c6 more rounds.");
			%printedBan = true;
		}
	}
	if(!%printedBan)
		%client.chatMessage("\c3None");
}
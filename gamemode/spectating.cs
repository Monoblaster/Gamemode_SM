function GameConnection::startSpectating(%client){
	return;
	echo("start spectating");
	%client.currentSpectate = 0;
	%client.spectatePlayer(0, 1);
	
}
function GameConnection::stopSpectating(%client){
return;
	echo("stop spectating");
	%client.currentSpectate = -1;
	%client.setControlObject(%client.player);
	
}
//subtract offset for every time you successfully get an alive player, if it's 0 then spectate that player
//make sure to increment based on the direction
function GameConnection::spectatePlayer(%client ,%offset, %direction){
	%group = SMMinigame.gameGroup;
	%c = %client.currentSpectate;
	if(%group.getCount() <= 0 || !%client.inLobby)
		return %client.stopSpectating();
	%currentPlayer = %group.getObject(%c);
	for(%i = 0; %i < %offset; %i++){
		while(%currentPlayer.inLobby && %c != %client.currentSpectate){
			echo(%currentPlayer.inLobby SPC (%c != %client.currentSpectate));
			%currentPlayer = %group.getObject(%c = roClamp(%c + %direction, 0, %group.getCount() - 1));
		}
	}
	//spectate the player unless none was found;
	if(%currentPlayer.inLobby && %c == %client.currentSpectate)
		return %client.stopSpectating();
	%client.currentSpectate = %c;
	if(!isObject(%currentPlayer))
		return %client.stopSpectating();
	%client.setControlObject(%client.camera);
	%client.camera.setOrbitMode(%currentPlayer.player, %currentPlayer.player.getTransform(), 0, 10, 10, false);
}
//there's probably a mathy way to do it with modulos but i can't figure it out rn
//roll over clamp
function roClamp(%num, %min, %max){
	echo(%num SPC %min SPC %max);
	if(%max < %min)
		return %num;
	if(%num < %min){
		return roClamp(%max - (%min - %num - 1), %min, %max);
	}
	if(%num > %max){
		return roClamp(%min + (%num - %max - 1), %min, %max);
	}
	return %num;
}
package smSpectating{
	function Observer::onTrigger(%this, %obj, %trigger, %state){
		if(%state == 0 && %obj.isOrbitMode() && %obj.getControllingClient() != %obj.getOrbitObject().client && %client.currentSpectate != -1){
			%client = %obj.getControllingClient();
			switch(%trigger){
				case 0:
					%client.spectatePlayer(1, 1);
				case 4:
					%client.spectatePlayer(1, -1);
			}
		}
		return parent::onTrigger(%this, %obj, %trigger, %state);
	}
};
deactivatePackage("smSpectating");
activatePackage("SmSpectating");

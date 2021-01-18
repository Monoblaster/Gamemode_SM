registerOutputEvent(GameConnection, finishTutorial);
function GameConnection::finishTutorial(%client){
	if(!%client.tutorialDone){
		%client.setItem("tutorialDone", true);
	}
}
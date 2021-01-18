//Borrowed from Visolator to make public bricks easier to work with
function serverCmdPublicAccess(%this)
{
	if(!%this.isSuperAdmin)
		return;

	%this.chatMessage("You now have trust with the public.");
	setMutualBrickgroupTrust(%this.getBLID(), 888888, 2);
}
registerInputEvent("FxDTSBrick", "onRelay", "Self FxDTSBrick" TAB "Player Player" TAB	"Client GameConnection" TAB "MiniGame MiniGame" TAB "Bot Bot");
registerOutputEvent(Bot, setSMAppearance);
registerOutputEvent(Bot, setupPassport, "bool");
//just gonna copy code here
function AiPlayer::setupPassport(%bot,%fake){
	if(isObject(%bot.passport))
		%bot.passport.delete();
	if(%fake){
		%face = $SMMinigame::randomFace[getRandom(0, $SMMinigame::randomFaceNum - 1)];
		while(%face $= %bot.faceName){
			%face = $SMMinigame::randomFace[getRandom(0, $SMMinigame::randomFaceNum - 1)];
		}
		%passportFace = %face;
		%passportColor = $SMMinigame::randomSkin[getRandom(0, $SMMinigame::randomSkinNum - 1)];
		%darken = getRandom() / 2.5;
		%darken = %darken SPC %darken SPC %darken;
		%passportColor = vectorSub(%passportColor, %darken) SPC 1;
	} else{
		%passportFace = %bot.faceName;
		%passportColor = %bot.headColor;
	}
	%passport = new AiPlayer(){
		dataBlock = "passport";
	};
	%passport.setFaceName(%passportFace);
	%passport.setNodeColor("face", %passportColor);
	%passport.setNodeColor("backing", "0.60 0.10 0 1");
	%passport.setNodeColor("paper", "0.90 0.90 0.90 1");
	%bot.mountObject(%passport, 0);
	%bot.passport = %passport;
}
function AiPlayer::setSMAppearance(%bot){
	%skinColor = $SMMinigame::randomSkin[getRandom(0, $SMMinigame::RandomSkinNum - 1)];
	%darken = getRandom() / 2.5;
	%darken = %darken SPC %darken SPC %darken;
	%skinColor = vectorSub(%skinColor, %darken);
	%shirtColor = getRandom() SPC getRandom() SPC getRandom();
	%shoeColor = getRandom()/4 SPC getRandom()/4 SPC getRandom()/4;
	%decalName = $SMMinigame::randomShirt[getRandom(0, $SMMinigame::RandomShirtNum - 1)];
	%faceName = $SMMinigame::randomFace[getRandom(0, $SMMinigame::RandomFaceNum - 1)];
	%chest = $SMMinigame::randomChest[getRandom(0, $SMMinigame::RandomChestNum - 1)];
	%hat = $SMMinigame::randomHat[getRandom(0, $SMMinigame::RandomHatNum - 1)];
	%hip = 0;
	%lArm = 0;
	%lHand = 0;
 	%lLeg = 0;
	%rArm = 0;
	%rHand = 0;
	%rLeg = 0;
	%pack = 0;
	%secondPack = 0;
	%hatColor = getRandom() SPC getRandom() SPC getRandom();
	%hipColor = getRandom()/2 SPC getRandom()/2 SPC getRandom()/2;
	serverCmdUpdateBodyParts(%bot,%hat,0,%pack,%secondPack,0,%hip,%lleg,%rleg,%larm,%rarm,%lhand,%rhand);
	serverCMDupdateBodyColors(%bot,%skinColor,%hatColor,"1 1 1 1", "1 1 1 1", "1 1 1 1", %shirtColor,%hipColor,%ShoeColor,%shoeColor,%shirtColor,%shirtColor,%skinColor,%skinCOlor,%decalName, %faceName);
	GameConnection::ApplyBodyParts(%bot);
	GameConnection::ApplyBodyCOlors(%bot);
}
package supportSM{
	function fxDTSBrick::onRelay (%obj, %client){
		$InputTarget_["Player"] = %client.player;
		$InputTarget_["Client"] = %client;
		$InputTarget_["Minigame"] = "";
		$InputTarget_["Bot"] = %obj.hBot;
		
		Parent::onRelay(%obj, %client);
	}
};
deactivatePackage("supportSM");
activatePackage("supportSM");

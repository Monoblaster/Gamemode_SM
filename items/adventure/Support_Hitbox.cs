//Support_Hitbox.cs
//Usage:
//if( matchBodyArea( getHitbox( %obj, %col, %pos ), $yourhitbox ); )
//{
//  your silly code here 
//}

$headTest = "plume triPlume septPlume visor helmet pointyHelmet flareHelmet scoutHat bicorn copHat knitHat headSkin";
$chestTest = "chest femchest armor bucket cape quiver tank";
$legTest = "lshoe rshoe lpeg rpeg pants skirtHip";
$armTest = "lhand rhand lhook rhook larm rarm";

function matchBodyArea( %list, %test )
{
	return false;
  %list = "\t" @ %list @ "\t";
  %cnt = getWordCount( %test );

  for ( %i = 0 ; %i < %cnt ; %i++ )
  {

    %word = "\t" @ getWord( %test, %i ) @ "\t";

    if ( striPos( %list, %word ) >= 0 )
    {
      return true;
    }

  }

  return false;
}

function Player::getLastImpactPosition( %this )
{
   return $_detectHitboxPosition;
}

function getLastImpactPosition()
{
   return $_detectHitboxPosition;
}

function getHitbox( %obj, %col, %pos )
{
   $_detectHitboxActive = true;
   $_detectHitboxList = "";
   $_detectHitboxPosition = %pos;

   paintProjectile::onCollision( "", %obj, %col, "", %pos, "" );
   cancel( %col.tempColorSchedule );
   %list = trim( $_detectHitboxList );

   deleteVariables( "$_detectHitbox*" );

   if ( ( %col.getType() & $TypeMasks::PlayerObjectType ) && isObject( %cl = %col.client ) )
   {
      %cl.applyBodyColors();
      %cl.applyBodyParts();
   }

   if ( %col.isCrouched() )
   {
      %list = trim( %list TAB headSkin );
   }

   if (matchBodyArea( %list, "headskin"))
   {
      %col.spawnExplosion(goremodHitProjectile,"2 2 2"); //not sure of arguments :V

      %obj.client.play2d(gamedamage2Sound);
      %col.playThread(0 , jump);
      %col.playThread(2 , jump);
      %col.schedule(50, "playThread", "0", "plant");
      %col.schedule(50, "playThread", "2", "plant");
   }

   return %list;
}

package hitboxDetection
{
   function shapeBase::setNodeColor( %this, %node, %color )
   {
    if ( $_detectHitboxActive )
    {
     $_detectHitboxList = trim( $_detectHitboxList TAB %node );
     return;
    }

    parent::setNodeColor( %this, %node, %color );
   }
};

activatePackage( "hitboxDetection" );
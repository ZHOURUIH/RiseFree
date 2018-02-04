using UnityEngine;
using System.Collections;

public class CommandCharacterSpeedSlow : Command
{
	public float mSpeed;
	public override void init()
	{
		base.init();
		mSpeed = 0.0f;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		if(mSpeed < GameDefine.SHAKE_BIKE_SPEED)
		{
			Animation animation = character.getAnimation();
			animation.CrossFade(GameDefine.ANIM_RIDE);
		}
	}
}

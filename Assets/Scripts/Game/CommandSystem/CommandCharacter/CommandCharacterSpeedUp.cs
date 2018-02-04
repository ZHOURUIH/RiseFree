using UnityEngine;
using System.Collections;

public class CommandCharacterSpeedUp : Command
{
	public float mSpeedDelta;
	public float mSpeed;
	public override void init()
	{
		base.init();
		mSpeedDelta = 0.0f;
		mSpeed = 0.0f;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		string anim = "";
		string animNext = "";
		// 达到加速动画播放条件
		if (mSpeedDelta > GameDefine.SPEED_UP_DELTA)
		{
			if (mSpeedDelta < GameDefine.SPEED_UP_FAST_DELTA)
			{
				anim = GameDefine.ANIM_SPEED_UP;
			}
			else
			{
				anim = GameDefine.ANIM_SPEED_UP_SHARP;
			}
		}

		// 是否摇车
		if (mSpeed >= GameDefine.SHAKE_BIKE_SPEED)
		{
			animNext = GameDefine.ANIM_SHAKE_BIKE;
		}
		else
		{
			animNext = GameDefine.ANIM_RIDE;
		}

		if (anim != "")
		{
			Animation animation = character.getAnimation();
			animation.CrossFade(anim);
			animation.CrossFadeQueued(animNext);
		}
	}
}
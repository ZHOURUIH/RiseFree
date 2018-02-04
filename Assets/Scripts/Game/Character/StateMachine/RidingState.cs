using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 正常骑行状态
public class RidingState : PlayerState
{
	public RidingState(PLAYER_STATE type)
		:
		base(type)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		// 根据当前速度设置骑行动作的速度
		float speed = mPlayer.getCharacterData().mSpeed;
		if (speed > 0.01f)
		{
			float normalRunSpeed = 8.0f;
			float animSpeed = speed / normalRunSpeed;
			MathUtility.clamp(ref animSpeed, 0.0f, 3.0f);
			mAnimation[GameDefine.ANIM_RIDE].speed = animSpeed;
			mAnimation[GameDefine.ANIM_SHAKE_BIKE].speed = animSpeed;
			string queuedCloneSuffix = " - Queued Clone";
			if (mAnimation[GameDefine.ANIM_RIDE + queuedCloneSuffix] != null)
			{
				mAnimation[GameDefine.ANIM_RIDE + queuedCloneSuffix].speed = animSpeed;
			}
			if (mAnimation[GameDefine.ANIM_SHAKE_BIKE + queuedCloneSuffix] != null)
			{
				mAnimation[GameDefine.ANIM_SHAKE_BIKE + queuedCloneSuffix].speed = animSpeed;
			}
		}
	}
	public override void leave()
	{
		mAnimation[GameDefine.ANIM_RIDE].speed = 1.0f;
		mAnimation[GameDefine.ANIM_SHAKE_BIKE].speed = 1.0f;
	}
}
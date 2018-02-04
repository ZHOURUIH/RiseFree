using UnityEngine;
using System.Collections;

// 跳跃状态,包括腾空
public class JumpState : PlayerState
{
	public JumpState(PLAYER_STATE type)
		:
		base(type)
	{
		mEnableStatus.enableAll(false);
	}
	public override void enter()
	{
		// 跳跃动作
		mAnimation.CrossFade(GameDefine.ANIM_PRE_JUMP);
		mAnimation.PlayQueued(GameDefine.ANIM_START_JUMP);
		mAnimation.PlayQueued(GameDefine.ANIM_JUMP_UP);
		mAnimation.PlayQueued(GameDefine.ANIM_JUMP_LOOP);
	}
	public override void leave()
	{
		;
	}
}
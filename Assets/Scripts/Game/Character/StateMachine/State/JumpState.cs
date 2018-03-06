using UnityEngine;
using System.Collections;

public class JumpParam : StateParam
{
	public float mJumpSpeed;
	public JumpParam(float speed)
	{
		mType = PLAYER_STATE.PS_JUMP;
		mJumpSpeed = speed;
	}
}

// 跳跃状态
public class JumpState : PlayerState
{
	public JumpState(PLAYER_STATE type)
		:
		base(type)
	{
		mEnableStatus.enableAll(false);
	}
	public override void enter(StateParam param)
	{
		// 添加竖直方向上的速度
		mPlayer.getCharacterData().mVerticalSpeed = (param as JumpParam).mJumpSpeed;
		// 跳跃动作
		mAnimation.CrossFade(GameDefine.ANIM_JUMP_UP);
		mAnimation.PlayQueued(GameDefine.ANIM_JUMP_LOOP);
	}
	public override void leave()
	{
		;
	}
}
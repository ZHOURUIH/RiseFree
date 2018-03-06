using UnityEngine;
using System.Collections;

// 被攻击状态
public class AttackedState : PlayerState
{
	public AttackedState(PLAYER_STATE type)
		:
		base(type)
	{
		mEnableStatus.enableAll(false);
	}
	public override void enter(StateParam param)
	{
		// 状态持续时间为摔倒动作的持续时间
		AnimationClip clip = mAnimation.GetClip(GameDefine.ANIM_FALL_DOWN);
		mStateTime = clip.length;
	}
	public override void leave()
	{
		;
	}
}
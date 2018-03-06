using UnityEngine;
using System.Collections;

public class CommandCharacterJump : Command
{
	public float mJumpSpeed;
	public override void init()
	{
		base.init();
		mJumpSpeed = GameDefine.JUMP_SPEED;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterOther other = character as CharacterOther;
		Animation anim = other.getAnimation();
		anim.Play(GameDefine.ANIM_PRE_JUMP);
		anim.PlayQueued(GameDefine.ANIM_START_JUMP);
		float preJumpLength = anim.GetClip(GameDefine.ANIM_PRE_JUMP).length;
		float startJumpLength = anim.GetClip(GameDefine.ANIM_START_JUMP).length;
		// 动作播放完成后才进入跳跃状态
		CommandCharacterAddState cmdAddState = newCmd(out cmdAddState, true, true);
		cmdAddState.mState = PLAYER_STATE.PS_JUMP;
		cmdAddState.mParam = new JumpParam(mJumpSpeed);
		pushDelayCommand(cmdAddState, character, preJumpLength + startJumpLength);
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + " : jump speed : " + mJumpSpeed;
	}
}
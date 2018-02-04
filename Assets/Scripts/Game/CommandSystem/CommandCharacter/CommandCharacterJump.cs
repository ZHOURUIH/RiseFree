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
		CommandCharacterAddState cmdAddState = newCmd(out cmdAddState);
		cmdAddState.mState = PLAYER_STATE.PS_JUMP;
		pushCommand(cmdAddState, character);
		// 添加竖直方向上的速度
		character.getCharacterData().mVerticalSpeed = mJumpSpeed;
	}
	public override string showDebugInfo()
	{
		return base.showDebugInfo() + "jump speed : " + mJumpSpeed;
	}
}
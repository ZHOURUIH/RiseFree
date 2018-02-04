using UnityEngine;
using System.Collections;

public class CommandCharacterStartRun : Command
{
	public override void init()
	{
		base.init();
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		Animation animation = character.getAnimation();
		animation[GameDefine.ANIM_STARTING].time = 0.0f;
		animation[GameDefine.ANIM_STARTING].speed = 1.0f;
		animation.CrossFade(GameDefine.ANIM_STARTING);
		animation.CrossFadeQueued(GameDefine.ANIM_RIDE);
		CommandCharacterAddState cmd = newCmd(out cmd);
		cmd.mState = PLAYER_STATE.PS_RIDING;
		pushCommand(cmd, character);
	}
}

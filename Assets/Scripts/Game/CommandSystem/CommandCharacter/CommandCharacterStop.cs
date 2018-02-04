using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CommandCharacterStop : Command
{
	public bool mFadeStop;
	public bool mFadeStanding;
	public override void init()
	{
		base.init();
		mFadeStop = true;
		mFadeStanding = true;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		Animation animation = character.getAnimation();
		if(mFadeStop)
		{
			// 倒播起步动作
			animation[GameDefine.ANIM_STARTING].time = animation[GameDefine.ANIM_STARTING].clip.length;
			animation[GameDefine.ANIM_STARTING].speed = -1.0f;
			animation.CrossFade(GameDefine.ANIM_STARTING);
			if (mFadeStanding)
			{
				animation.CrossFadeQueued(GameDefine.ANIM_STANDING);
			}
			else
			{
				animation.PlayQueued(GameDefine.ANIM_STANDING);
			}
		}
		else
		{
			if (mFadeStanding)
			{
				animation.CrossFade(GameDefine.ANIM_STANDING);
			}
			else
			{
				animation.Play(GameDefine.ANIM_STANDING);
			}
		}
		// 添加idle状态
		CommandCharacterAddState cmdState = newCmd(out cmdState);
		cmdState.mState = PLAYER_STATE.PS_IDLE;
		pushCommand(cmdState, character);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : fade stop : " + mFadeStop + ", fade standing : " + mFadeStanding;
	}
}


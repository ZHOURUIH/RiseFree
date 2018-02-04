using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CommandCharacterAttacked : Command
{
	public SCENE_ITEM mAttackSource;
	public override void init()
	{
		base.init();
		mAttackSource = SCENE_ITEM.SI_MAX;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		if (!character.isType(CHARACTER_TYPE.CT_OTHER))
		{
			return;
		}
		CharacterOther other = character as CharacterOther;
		// 玩家有护盾时,攻击没有任何影响
		if (other.hasState(PLAYER_STATE.PS_PROTECTED))
		{
			return;
		}
		if(mAttackSource == SCENE_ITEM.SI_LAND_MINE || mAttackSource == SCENE_ITEM.SI_MISSILE)
		{
			// 进入被攻击状态
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_ATTACKED;
			pushCommand(cmdState, other);
			// 设置角色的速度,当角色速度小于3km/h时,则不变,否则速度设置为3km/h
			CommandCharacterChangeSpeed cmd = newCmd(out cmd, false);
			cmd.mSpeed = Mathf.Min(MathUtility.KMHtoMS(3.0f), other.getCharacterData().mSpeed);
			pushCommand(cmd, other);
			// 播放摔倒动作
			Animation animation = other.getAnimation();
			animation.CrossFade(GameDefine.ANIM_FALL_DOWN);
			// 当速度为0时,摔到后为站立状态,否则开始骑行
			if (other.getCharacterData().mSpeed <= 0.01f)
			{
				animation.CrossFadeQueued(GameDefine.ANIM_STANDING);
			}
			else
			{
				animation.CrossFadeQueued(GameDefine.ANIM_RIDE);
			}
			// 同时需要降低硬件速度组件中从当前速度加速到目标速度的快慢
			CharacterSpeedHardware speedHardware = other.getFirstComponent<CharacterSpeedHardware>();
			speedHardware.setAcceleration(0.0f);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : attack source : " + mAttackSource;
	}
}


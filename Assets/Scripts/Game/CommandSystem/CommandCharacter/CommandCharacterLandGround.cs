using UnityEngine;
using System.Collections;

public class CommandCharacterLandGround : Command
{
	public float mVerticalSpeed;
	public override void init()
	{
		base.init();
		mVerticalSpeed = 0.0f;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		Animation animation = character.getAnimation();
		if (mVerticalSpeed >= GameDefine.FORCE_LAND_SPEED)
		{
			animation.Play(GameDefine.ANIM_FORCE_LANDING);
		}
		else
		{
			animation.Play(GameDefine.ANIM_LANDING);
		}
		float curSpeed = character.getCharacterData().mSpeed;
		// 起跳时有水平方向的速度
		if (curSpeed > 0.0f)
		{
			// 是否摇车
			if (curSpeed >= GameDefine.SHAKE_BIKE_SPEED)
			{
				animation.CrossFadeQueued(GameDefine.ANIM_SHAKE_BIKE, 0.1f);
			}
			else
			{
				animation.CrossFadeQueued(GameDefine.ANIM_RIDE, 0.1f);
			}
			// 添加骑行状态
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_RIDING;
			pushCommand(cmdState, character);
		}
		// 静止时原地起跳
		else
		{
			animation.CrossFadeQueued(GameDefine.ANIM_STANDING, 0.1f);
			// 添加休闲状态
			CommandCharacterAddState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_IDLE;
			pushCommand(cmdState, character);
		}
	}
}

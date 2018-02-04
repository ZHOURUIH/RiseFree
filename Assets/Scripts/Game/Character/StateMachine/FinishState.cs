using UnityEngine;
using System.Collections;

// 比赛结束状态
public class FinishState : PlayerState
{
	public FinishState(PLAYER_STATE type)
		:
		base(type)
	{
		mEnableStatus.enableAll(false);
	}
	public override void enter()
	{
		// 禁用动态拖尾
		mPlayer.activeTrail(false, false);
		// 将速度降低到0
		CommandCharacterHardwareSpeed cmdHardwareSpeed = newCmd(out cmdHardwareSpeed);
		cmdHardwareSpeed.mSpeed = 0.0f;
		cmdHardwareSpeed.mExternalSpeed = false;
		pushCommand(cmdHardwareSpeed, mPlayer);
	}
	public override void leave()
	{
		// 速度设置为0
		CommandCharacterChangeSpeed cmdSpeed = newCmd(out cmdSpeed);
		cmdSpeed.mSpeed = 0.0f;
		pushCommand(cmdSpeed, mPlayer);
		// 禁用自行车物理组件,禁用硬件速度组件
		mPlayer.activeFirstComponent<CharacterBikePhysics>(false);
		mPlayer.activeFirstComponent<CharacterSpeedHardware>(false);
		// 禁用AI控制器
		if (mPlayer.isType(CHARACTER_TYPE.CT_AI))
		{
			mPlayer.activeFirstComponent<CharacterControllerAI>(false);
		}
		// 断开摄像机的连接
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			pushCommand<CommandCameraLinkTarget>(mCameraManager.getMainCamera());
		}
	}
}
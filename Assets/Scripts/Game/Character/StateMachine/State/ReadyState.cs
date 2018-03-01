using UnityEngine;
using System.Collections;

// 准备状态
public class ReadyState : PlayerState
{
	public ReadyState(PLAYER_STATE type)
		:
		base(type)
	{
		mEnableStatus.enableAll(false);
	}
	public override void enter()
	{
		// 隐藏静态拖尾,并显示动态拖
		mPlayer.activeTrail(false);

		// 将玩家设置到起点位置
		GameTrackBase curTrack = mRaceSystem.getCurGameTrack();
		ObjectTools.MOVE_OBJECT(mPlayer, curTrack.mStartPointList[mPlayer.getCharacterData().mStartIndex]);

		// 添加休闲状态
		CommandCharacterStop cmdStop = newCmd(out cmdStop);
		cmdStop.mFadeStop = false;
		cmdStop.mFadeStanding = false;
		pushCommand(cmdStop, mPlayer);

		// 使角色朝向前方
		Vector3 point0 = mWayPointManager.getPoint(0);
		Vector3 point1 = mWayPointManager.getPoint(1);
		float dir = MathUtility.getAngleFromVector(point1 - point0) * Mathf.Rad2Deg;
		ObjectTools.ROTATE_OBJECT(mPlayer, new Vector3(0.0f, dir, 0.0f));

		// 启用自行车物理组件,启用硬件速度组件,里程计算组件,手动更新角色位置后再挂接摄像机
		mPlayer.activeFirstComponent<CharacterTrackMileage>();
		mPlayer.activeFirstComponent<CharacterBikePhysics>();
		mPlayer.activeFirstComponent<CharacterSpeedHardware>();
		mPlayer.getFirstComponent<CharacterBikePhysics>().correctTransform();
		if(mPlayer.isType(CHARACTER_TYPE.CT_AI))
		{
			mPlayer.activeFirstComponent<CharacterControllerAI>();
		}
		// 只有主角才挂摄像机
		if(mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			CommandCameraLinkTarget cmd = newCmd(out cmd);
			cmd.mTarget = mPlayer;
			cmd.mLinkerName = "SmoothFollow";
			cmd.mLookatOffset = GameDefine.CAMERA_LOOKAT_OFFSET;
			cmd.mImmediately = true;
			cmd.setRelativePosition(GameDefine.CAMERA_RELATIVE);
			pushCommand(cmd, mCameraManager.getMainCamera());
			CameraLinkerSmoothFollow linkerSmoothFollow = mCameraManager.getMainCamera().getComponent("SmoothFollow") as CameraLinkerSmoothFollow;
			linkerSmoothFollow.setCheckGroundLayer(GameUtility.mGroundLayer);
		}
	}
}
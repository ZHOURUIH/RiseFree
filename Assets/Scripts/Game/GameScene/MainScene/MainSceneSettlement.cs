using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 结算流程
public class MainSceneSettlement : SceneProcedure
{
	public MainSceneSettlement()
	{ }
	public MainSceneSettlement(PROCEDURE_TYPE type, GameScene gameScene)
		:
		 base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_SETTLEMENT, 10);
		// 卸载除了角色展示以外的所有场景
		mSceneSystem.unloadOtherScene(GameDefine.ROLE_DISPLAY);
		// 激活场景,初始化场景
		mSceneSystem.activeScene(GameDefine.ROLE_DISPLAY);
		mSceneSystem.initScene(GameDefine.ROLE_DISPLAY);

		// 设置摄像机位置
		RoleDisplay roleDisplay = mSceneSystem.getScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		mCameraManager.getMainCamera().copyCamera(roleDisplay.mCameraPositionObject0);

		// 给主角添加选中状态
		CommandCharacterAddState cmdState = newCmd(out cmdState);
		cmdState.mState = PLAYER_STATE.PS_ON_SELECT_ROLE;
		pushCommand(cmdState, mCharacterManager.getMyself());
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		// 清空所有角色的所有状态
		mRoleSystem.clearAllPlayerState();
		// 销毁所有比赛角色
		mRoleSystem.destroyAllPlayer();
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_SETTLEMENT);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		// 按A键返回到选择角色流程
		if(mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			CommandGameSceneManagerEnter cmdEnter = newCmd(out cmdEnter);
			cmdEnter.mSceneType = GAME_SCENE_TYPE.GST_START;
			pushCommand(cmdEnter, mGameSceneManager);
		}
	}
}
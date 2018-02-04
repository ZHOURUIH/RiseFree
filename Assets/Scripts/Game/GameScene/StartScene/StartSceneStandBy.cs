using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneStandBy : SceneProcedure
{
	public StartSceneStandBy()
	{ }
	public StartSceneStandBy(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_STAND_BY, 1);
		mSceneSystem.activeScene(GameDefine.ROLE_DISPLAY);
		mSceneSystem.initScene(GameDefine.ROLE_DISPLAY);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_STAND_BY);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		// 按A跳转到角色选择流程
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_SELECT_ROLE;
			pushCommand(cmd, mGameScene);
			return;
		}
		if(mGameInputManager.getKeyCurrentDown(KeyCode.Y))
		{
			mGameFramework.stop();
			return;
		}
	}
}
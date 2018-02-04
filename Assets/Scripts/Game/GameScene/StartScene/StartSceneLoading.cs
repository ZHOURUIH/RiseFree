using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneLoading : SceneProcedure
{
	public StartSceneLoading()
	{ }
	public StartSceneLoading(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		mSceneSystem.loadSceneAsync(GameDefine.ROLE_DISPLAY, false, onSceneLoaded);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	protected void onSceneLoaded(float progress, bool done, object userData)
	{
		if(done)
		{
			// 初始化选择用的角色
			mRoleSystem.initSelectRole();
			// 加载结束后在下一帧跳转流程
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd, true, true);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_STAND_BY;
			pushDelayCommand(cmd, mGameScene);
		}
	}
}
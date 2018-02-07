using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneSetting : SceneProcedure
{
	protected float mVolume;
	public StartSceneSetting()
	{ }
	public StartSceneSetting(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		mVolume = mGameSetting.getCurVolume();
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_VOLUME_SETTING, 10);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		MathUtility.clamp(ref mVolume, 0.0f, 1.0f);
		mGameSetting.setCurVolume(mVolume);
		mGameSetting.applyToConfig();
	}
	protected override void onPrepareExit(SceneProcedure nextProcedure)
	{
		base.onPrepareExit(nextProcedure);
		if (nextProcedure.isThisOrParent(PROCEDURE_TYPE.PT_START_SELECT_ROLE) || nextProcedure.isThisOrParent(PROCEDURE_TYPE.PT_START_SELECT_TRACK))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_VOLUME_SETTING);
		}
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		if (mGameInputManager.getKeyCurrentDown(KeyCode.X))
		{
			// 准备在0.5秒之后跳转到选择角色流程
			CommandGameScenePrepareChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = mGameScene.getLastProcedureType();
			cmd.mPrepareTime = 0.5f;
			pushCommand(cmd, mGameScene);
			return;
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			mScriptVolumeSetting.setVolume(mVolume += 0.01f);
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.B))
		{
			mScriptVolumeSetting.setVolume(mVolume -= 0.01f);
		}
	}
}
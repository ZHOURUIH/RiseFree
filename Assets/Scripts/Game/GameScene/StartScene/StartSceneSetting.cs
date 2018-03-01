using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneSetting : SceneProcedure
{
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
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_VOLUME_SETTING, 10);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		;
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
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_CLICK_BUTTON);
			return;
		}
		if (mGameInputManager.getKeyDown(KeyCode.A))
		{
			setVolume(mGameSetting.getCurVolume() + 0.01f);
		}
		if (mGameInputManager.getKeyDown(KeyCode.B))
		{
			setVolume(mGameSetting.getCurVolume() - 0.01f);
		}
	}
	protected void setVolume(float volume)
	{
		MathUtility.clamp(ref volume, 0.0f, 1.0f);
		mScriptVolumeSetting.setVolume(volume);
		mGameSetting.setCurVolume(volume);
		mGameSetting.applyToConfig();
		GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_CLICK_BUTTON);
		GameScene gameScene = mGameSceneManager.getCurScene();
		GameSceneComponentAudio componentAudio = gameScene.getFirstActiveComponent<GameSceneComponentAudio>();
		componentAudio.setVolume(volume);
	}
}
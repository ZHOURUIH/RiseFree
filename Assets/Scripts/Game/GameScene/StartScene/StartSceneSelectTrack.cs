using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class StartSceneSelectTrack : SceneProcedure
{
	public StartSceneSelectTrack()
	{ }
	public StartSceneSelectTrack(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		if (lastProcedure.isThisOrParent(PROCEDURE_TYPE.PT_START_SETTING))
		{
			LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_BUTTOM_PROMPT, 0);
		}
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_SELECT_TRACK, 0);
		RoleDisplay roleDisplay = mSceneSystem.getScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		Transform cameraPos1 = roleDisplay.mCameraTransform1;
		GameCamera mainCamera = mCameraManager.getMainCamera();
		ObjectTools.MOVE_OBJECT(mainCamera, mainCamera.getPosition(), cameraPos1.localPosition, 0.5f);
		ObjectTools.ROTATE_OBJECT(mainCamera, mainCamera.getRotation(), cameraPos1.localEulerAngles, 0.5f);
		CommandStartSceneSelectTrack cmdTrack = newCmd(out cmdTrack);
		cmdTrack.mTrack = mRaceSystem.getTrackIndex();
		cmdTrack.mPlayAudio = false;
		pushCommand(cmdTrack, mGameScene);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onPrepareExit(SceneProcedure nextProcedure)
	{
		base.onPrepareExit(nextProcedure);
		if (nextProcedure.isThisOrParent(PROCEDURE_TYPE.PT_START_SELECT_ROLE))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_SELECT_TRACK);
		}
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_SELECT_TRACK, true);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		if (mGameInputManager.getKeyCurrentDown(KeyCode.Y))
		{
			CommandGameScenePrepareChangeProcedure cmd = newCmd(out cmd, true, false);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_SELECT_ROLE;
			cmd.mPrepareTime = 1.0f;
			pushCommand(cmd, mGameScene);
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_CLICK_BUTTON);
			return;
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.X))
		{
			LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_BUTTOM_PROMPT);
			CommandGameScenePrepareChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_SETTING;
			cmd.mPrepareTime = 0.25f;
			pushCommand(cmd, mGameScene);
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_CLICK_BUTTON);
			return;
		}
		if (mGameInputManager.turnLeft())
		{
			CommandStartSceneSelectTrack cmdTrack = newCmd(out cmdTrack);
			cmdTrack.mTrack = mRaceSystem.getLastTrackIndex();
			pushCommand(cmdTrack, mGameScene);
		}
		if (mGameInputManager.turnRight())
		{
			CommandStartSceneSelectTrack cmdTrack = newCmd(out cmdTrack);
			cmdTrack.mTrack = mRaceSystem.getNextTrackIndex();
			pushCommand(cmdTrack, mGameScene);
		}
		if (mGameInputManager.getKeyCurrentDown(KeyCode.A))
		{
			CommandGameSceneChangeProcedure cmd = newCmd(out cmd);
			cmd.mProcedure = PROCEDURE_TYPE.PT_START_CONFIRM_SELECTION;
			pushCommand(cmd, mGameScene);
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_CLICK_BUTTON);
			return;
		}
	}
}
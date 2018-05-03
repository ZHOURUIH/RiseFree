using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneStartVideo : SceneProcedure
{
	public LogoSceneStartVideo()
	{ }
	public LogoSceneStartVideo(PROCEDURE_TYPE type, GameScene gameScene)
		:
	base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 显示启动视频,等待视频准备完毕
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_START_VIDEO, 1);
	}
	protected override void onUpdate(float elapsedTime)
	{
		;
	}
	protected override void onExit(SceneProcedure nextProcedure)
	{
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_START_VIDEO);
	}
	protected override void onKeyProcess(float elapsedTime)
	{
		;
	}
	// 通知流程视频准备完毕
	public void notifyStartVideoReady()
	{
		// 播放视频音乐
		CommandGameScenePlayAudio cmdAudio = newCmd(out cmdAudio);
		cmdAudio.mSound = SOUND_DEFINE.SD_LOGO_VIDEO;
		cmdAudio.mVolume = mGameConfig.getFloatParam(GAME_DEFINE_FLOAT.GDF_VOLUME);
		pushCommand(cmdAudio, mGameScene);
		// 通知场景当前流程准备完毕
		mGameScene.notifyProcedurePrepared();
	}
	// 通知流程视频播放完毕
	public void notifyStartVideoDone()
	{
		// 为保视频与待机界面之间无缝衔接,在播放完后就立即加载并且显示待机界面
		LayoutTools.SHOW_LAYOUT(LAYOUT_TYPE.LT_STAND_BY, false, "FirstStart");
		// 停止视频音乐
		pushCommand<CommandGameSceneStopAudio>(mGameScene);
		// 进入开始场景
		CommandGameSceneManagerEnter cmdScene = newCmd(out cmdScene, true, false);
		cmdScene.mSceneType = GAME_SCENE_TYPE.GST_START;
		pushCommand(cmdScene, mGameSceneManager);
	}
}
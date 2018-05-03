using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneLogo : SceneProcedure
{
	public LogoSceneLogo()
	{ }
	public LogoSceneLogo(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		;
	}
	protected override void onInit(SceneProcedure lastProcedure, string intent)
	{
		// 先加载关键帧资源
		mKeyFrameManager.loadAll(false);
		// 预先加载启动视频的背景音乐,因为需要尽量保证在播放视频时视频音效已经加载完
		mAudioManager.loadAudio(SOUND_DEFINE.SD_LOGO_VIDEO);
		// 然后异步加载所有音效
		mAudioManager.loadAll(true);
		// 预加载需要用到的预设
		mObjectManager.loadPrefab(GameDefine.R_PARTICLE_PREFAB_PATH + GameDefine.SHIELD);
		mObjectManager.loadPrefab(GameDefine.R_PARTICLE_PREFAB_PATH + GameDefine.TURBO);
		mObjectManager.loadPrefab(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.ITEM_BOX);
		mObjectManager.loadPrefab(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.LANDMINE);
		mObjectManager.loadPrefab(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.MISSILE);
		for (int i = 0; i < GameDefine.ROLE_COUNT; ++i)
		{
			mObjectManager.loadPrefab(GameDefine.R_CHARACTER_PREFAB_PATH + GameDefine.ROLE_MODEL_NAME[i]);
		}
		// 加载并显示logo布局,也加载全局音效布局
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_LOGO, 0);
		LayoutTools.LOAD_NGUI_SHOW(LAYOUT_TYPE.LT_GLOBAL_AUDIO, 0);
		LayoutTools.LOAD_NGUI_HIDE(LAYOUT_TYPE.LT_DEBUG_INFO, 25);
		LayoutTools.LOAD_NGUI_ASYNC(LAYOUT_TYPE.LT_STAND_BY, 1, null);
		// 隐藏完毕后跳转到启动视频流程
		CommandGameSceneChangeProcedure cmdProcedure = newCmd(out cmdProcedure, true, true);
		cmdProcedure.mProcedure = PROCEDURE_TYPE.PT_LOGO_START_VIDEO;
		pushDelayCommand(cmdProcedure, mGameScene, mScriptLogo.getFullTime());
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
	public override void onNextProcedurePrepared(SceneProcedure nextPreocedure)
	{
		base.onNextProcedurePrepared(nextPreocedure);
		// 视频准备完毕后才卸载logo布局
		LayoutTools.HIDE_LAYOUT(LAYOUT_TYPE.LT_LOGO);
	}
}
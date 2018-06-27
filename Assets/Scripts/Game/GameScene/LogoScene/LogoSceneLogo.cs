using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LogoSceneLogo : SceneProcedure
{
	protected bool mPreloadObjectDone;
	protected Dictionary<string, bool> mPreloadObjectList; // 预加载的资源是否已经加载完毕
	public LogoSceneLogo() { }
	public LogoSceneLogo(PROCEDURE_TYPE type, GameScene gameScene)
		:
		base(type, gameScene)
	{
		mPreloadObjectDone = false;
		mPreloadObjectList = new Dictionary<string, bool>();
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
		preloadObject(GameDefine.R_PARTICLE_PREFAB_PATH + GameDefine.SHIELD);
		preloadObject(GameDefine.R_PARTICLE_PREFAB_PATH + GameDefine.TURBO);
		preloadObject(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.ITEM_BOX);
		preloadObject(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.LANDMINE);
		preloadObject(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.MISSILE);
		for (int i = 0; i < GameDefine.ROLE_COUNT; ++i)
		{
			preloadObject(GameDefine.R_CHARACTER_PREFAB_PATH + GameDefine.ROLE_MODEL_NAME[i]);
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
	public bool isObjectPreloadDone() { return mPreloadObjectDone; }
	//-----------------------------------------------------------------------------------------------------------------------------------------------
	protected void preloadObject(string name)
	{
		// 异步加载时如果返回值不为空,则表示之前已经加载过了
		if (mObjectManager.loadPrefab(name, true, onObjectLoaded) == null)
		{
			mPreloadObjectList.Add(name, false);
		}
	}
	protected void onObjectLoaded(UnityEngine.Object res, object userData)
	{
		string name = userData as string;
		if (mPreloadObjectList.ContainsKey(name))
		{
			mPreloadObjectList[name] = true;
		}
		UnityUtility.logInfo(name + "预加载完毕!");
		mPreloadObjectDone = true;
		foreach (var item in mPreloadObjectList)
		{
			if (!item.Value)
			{
				mPreloadObjectDone = false;
				break;
			}
		}
	}
}
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
		// 加载并显示logo布局
		LayoutTools.LOAD_LAYOUT_SHOW(LAYOUT_TYPE.LT_LOGO, 0);
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
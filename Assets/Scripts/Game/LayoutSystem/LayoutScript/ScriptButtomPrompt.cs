﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptButtomPrompt : LayoutScript
{
	protected txUIStaticSprite mBottomPromptBackground;	// 下方提示背景
	protected txUIStaticSprite mLabelRoot;				// 下方提示所有按钮父级
	protected txUIStaticSprite mGeneralPromptLabel;		// 普通布局要显示的提示
	protected txUIStaticSprite mGamingPromptLabel;		// 游戏中要显示的提示
	protected txUIStaticSprite mBackgroundStart;
	protected txUIStaticSprite mBackgroundEnd;
	protected txUIStaticSprite mLabelAndSoundStart;
	protected txUIStaticSprite mLabelAndSoundEnd;
	protected Vector3 mBackgroundStartPos;				// 下方提示移动开始移动的位置
	protected Vector3 mBackgroundEndPos;				// 结束的位置
	protected Vector3 mLabelAndSoundStartPos;			// 下方文字 和 音乐 开始移动的位置
	protected Vector3 mLabelAndSoundEndPos;				// 结束的位置
	public ScriptButtomPrompt(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mBottomPromptBackground = newObject<txUIStaticSprite>("BottomPromptBackground", 1);
		mLabelRoot = newObject<txUIStaticSprite>("LabelRoot", 0);
		mGeneralPromptLabel = newObject<txUIStaticSprite>(mLabelRoot, "GeneralPromptLabel", 0);
		mGamingPromptLabel = newObject<txUIStaticSprite>(mLabelRoot, "GamingPromptLabel", 0);
		mBackgroundStart = newObject<txUIStaticSprite>("BackgroundStartPos", 0);
		mBackgroundEnd = newObject<txUIStaticSprite>("BackgroundEndPos", 0);
		mLabelAndSoundStart = newObject<txUIStaticSprite>("LabelAndSoundStartPos", 0);
		mLabelAndSoundEnd = newObject<txUIStaticSprite>("LabelAndSoundEndPos", 0);
	}
	public override void init()
	{
		// 得到所有的位置
		mBackgroundStartPos = mBackgroundStart.getPosition();
		mBackgroundEndPos = mBackgroundEnd.getPosition();
		mLabelAndSoundStartPos = mLabelAndSoundStart.getPosition();
		mLabelAndSoundEndPos = mLabelAndSoundEnd.getPosition();
		mLayout.setScriptControlHide(true);
	}
	public override void onReset()
	{
		// 重置位置
		LayoutTools.MOVE_WINDOW(mBottomPromptBackground, mBackgroundStartPos);
		LayoutTools.MOVE_WINDOW(mLabelRoot, mLabelAndSoundStartPos);
		// 重置显示
		LayoutTools.ACTIVE_WINDOW(mLabelRoot, false);
		LayoutTools.ACTIVE_WINDOW(mGeneralPromptLabel, false);
		LayoutTools.ACTIVE_WINDOW(mGamingPromptLabel, false);
		// 重置透明度
		LayoutTools.ALPHA_WINDOW(mBottomPromptBackground, 0.3f);
		LayoutTools.ALPHA_WINDOW(mLabelRoot, 0.0f);
		LayoutTools.ALPHA_WINDOW(mGeneralPromptLabel, 0.0f);
		LayoutTools.ALPHA_WINDOW(mGamingPromptLabel, 0.0f);
	}
	public override void onGameState()
	{
		// 判断进入不同的 模式 显示不同的文字提示
		if (mGameSceneManager.getCurScene().atProcedure(PROCEDURE_TYPE.PT_MAIN_READY))
		{
			LayoutTools.ACTIVE_WINDOW(mGamingPromptLabel);
			LayoutTools.ALPHA_WINDOW(mGamingPromptLabel, 1.0f);
		}
		else if (mGameSceneManager.getCurScene().atProcedure(PROCEDURE_TYPE.PT_START_SELECT_ROLE))
		{
			LayoutTools.ACTIVE_WINDOW(mGeneralPromptLabel);
			LayoutTools.ALPHA_WINDOW(mGeneralPromptLabel, 1.0f);
		}
	}
	public override void onShow(bool immediately, string param)
	{
		// 移动底部背景
		if (immediately)
		{
			LayoutTools.MOVE_WINDOW(mBottomPromptBackground, mBackgroundEndPos);
			LayoutTools.ALPHA_WINDOW(mBottomPromptBackground, 1.0f);
			LayoutTools.ACTIVE_WINDOW(mLabelRoot);
			LayoutTools.ALPHA_WINDOW(mLabelRoot, 1.0f);
			LayoutTools.MOVE_WINDOW(mLabelRoot, mLabelAndSoundEndPos);
		}
		else
		{
			LayoutTools.MOVE_WINDOW_EX(mBottomPromptBackground, mBackgroundStartPos, mBackgroundEndPos, 0.5f, onPromptEnd);
			LayoutTools.ALPHA_WINDOW(mBottomPromptBackground, 0.3f, 1.0f, 0.5f);
		}
	}
	public override void onHide(bool immediately, string param)
	{
		if (immediately)
		{
			LayoutTools.MOVE_WINDOW(mLabelRoot, mLabelAndSoundStartPos);
			LayoutTools.ALPHA_WINDOW(mLabelRoot, 0.0f);
			LayoutTools.ACTIVE_WINDOW(mLabelRoot, false);
			LayoutTools.ALPHA_WINDOW(mBottomPromptBackground, 0.3f);
			LayoutTools.MOVE_WINDOW(mBottomPromptBackground, mBackgroundStartPos);
			LayoutTools.HIDE_LAYOUT_FORCE(mType);
		}
		else
		{
			LayoutTools.MOVE_WINDOW(mLabelRoot, mLabelAndSoundEndPos, mLabelAndSoundStartPos, 0.25f);
			LayoutTools.ALPHA_WINDOW_EX(mLabelRoot, 1.0f, 0.0f, 0.25f, onLabelHide);
		}
	}
	public override void update(float elapsedTime)
	{
		;
	}
	//-------------------------------------------------------------------------------------------------------------------------------------------
	private void onPromptEnd(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		// 背景移动完成 所有按钮文字出现 开始显示 移动
		LayoutTools.ACTIVE_WINDOW(mLabelRoot);
		LayoutTools.ALPHA_WINDOW(mLabelRoot, 0.0f, 1.0f, 0.3f);
		LayoutTools.MOVE_WINDOW(mLabelRoot, mLabelAndSoundStartPos, mLabelAndSoundEndPos, 0.3f);
	}
	protected void onLabelHide(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		LayoutTools.ACTIVE_WINDOW(mLabelRoot, false);
		LayoutTools.ALPHA_WINDOW(mBottomPromptBackground, 1.0f, 0.3f, 0.2f);
		LayoutTools.MOVE_WINDOW_EX(mBottomPromptBackground, mBackgroundEndPos, mBackgroundStartPos, 0.2f, onBackgroundHide);
	}
	protected void onBackgroundHide(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		LayoutTools.HIDE_LAYOUT_FORCE(mType);
	}
}
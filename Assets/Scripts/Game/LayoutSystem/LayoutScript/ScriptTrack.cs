using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptTrack : LayoutScript
{
	public int AiIndex = 4;										// AI的数量
	public int mRing = 3;										// 最大的分割圈数
	protected txUIStaticSprite mTrackRoot;
	protected txUIStaticSprite mBackgroundRoot;
	protected txUIStaticSprite[] mLapDivided;
	protected txUIStaticSprite mPlayerBackground;
	protected txUIStaticSprite mPlayerIcon;
	protected txUIStaticSprite mEnd;
	protected txUIObject mAIRoot;
	protected txUIStaticSprite[] mAI;
	protected txUIStaticSprite mTrackStart;
	protected Vector3 mTrackStartPos;
	protected Vector3 mTrackEndPos;
	protected Vector2 mBackgroundSize;
	protected Vector2 mPlayerBackgroundSize;
	protected Vector2[] mLapDividedPos;
	public ScriptTrack(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mAI = new txUIStaticSprite[AiIndex];
		mLapDivided = new txUIStaticSprite[mRing];
		mLapDividedPos = new Vector2[mRing];
	}
	public override void assignWindow()
	{
		newObject(ref mTrackRoot, "TrackRoot");
		newObject(ref mBackgroundRoot, "BackgroundRoot");
		for (int i = 0; i < mRing; i++)
		{
			txUIStaticSprite divide = null;
			newObject(ref divide, mBackgroundRoot, "LapDivided" + i, 0);
			mLapDivided[i] = divide;
		}
		newObject(ref mPlayerBackground, mTrackRoot, "PlayerBackground");
		newObject(ref mPlayerIcon, mPlayerBackground, "PlayerIcon");
		newObject(ref mEnd, mTrackRoot, "End");
		newObject(ref mAIRoot, mTrackRoot, "AIRoot");
		for (int i = 0; i < AiIndex; i++)
		{
			txUIStaticSprite AI = null;
			newObject(ref AI, mAIRoot, "AI" + i, 0);
			mAI[i] = AI;
		}
		newObject(ref mTrackStart, "TrackStartPos");
	}
	public override void init()
	{
		mTrackStartPos = mTrackStart.getPosition();
		mTrackEndPos = mTrackRoot.getPosition();
		mBackgroundSize = mBackgroundRoot.getWindowSize();
		mPlayerBackgroundSize = mPlayerBackground.getWindowSize();
		for (int i = 0; i < mRing; i++)
		{
			mLapDividedPos[i] = mLapDivided[i].getPosition();
		}
	}
	public override void onReset()
	{
		LayoutTools.MOVE_WINDOW(mTrackRoot, mTrackStartPos);
		for (int i = 0; i < AiIndex; i++)
		{
			LayoutTools.ACTIVE_WINDOW(mAI[i], false);
		}
		for (int i = 0; i < mRing; i++)
		{
			LayoutTools.ACTIVE_WINDOW(mLapDivided[i], false);
		}
	}
	public override void onGameState()
	{
		int circle = mRaceSystem.getCurGameTrack().mCircleCount;
		for (int i = 0; i < mLapDivided.Length; ++i)
		{
			bool visible = i < circle - 1;
			LayoutTools.ACTIVE_WINDOW(mLapDivided[i], visible);
			if (visible)
			{
				Vector2 iconWindowSize = mLapDivided[i].getWindowSize();
				Vector2 windowPos = new Vector2(mBackgroundSize.x / 2.0f + iconWindowSize.x / 2.0f, (float)(i + 1) / circle * mBackgroundSize.y);
				LayoutTools.MOVE_WINDOW(mLapDivided[i], translateIconPos(windowPos, mBackgroundSize));
			}
		}
		int aiCount = mRoleSystem.getPlayerCount() - 1;
		int count = mAI.Length;
		for(int i = 0; i < count; ++i)
		{
			bool visible = i < aiCount;
			LayoutTools.ACTIVE_WINDOW(mAI[i], visible);
			if(visible)
			{
				setPlayerProgress(0.0f, i);
			}
		}
		setPlayerProgress(0.0f, -1);
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.MOVE_WINDOW(mTrackRoot, mTrackStartPos, mTrackEndPos, 0.8f);
		LayoutTools.ALPHA_WINDOW(mTrackRoot, 0.3f, 1.0f, 0.8f);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	// 显示当前玩家的进度,number为玩家编号,-1标识玩家自己,大于等于0标识AI
	public void setPlayerProgress(float progress, int number)
	{
		if(progress > 1.0f)
		{
			UnityUtility.logError("progress must be less than 1.0f");
		}
		Vector2 iconPos = new Vector2(mPlayerBackgroundSize.x / 2.0f, mPlayerBackgroundSize.y * progress);
		iconPos = translateIconPos(iconPos, mPlayerBackgroundSize);
		if(number < 0)
		{
			// 设置显示进度,计算玩家图标位置
			mPlayerBackground.setFillPercent(progress);
			LayoutTools.MOVE_WINDOW(mPlayerIcon, iconPos);
		}
		else
		{
			LayoutTools.MOVE_WINDOW(mAI[number], iconPos);
		}
	}
	//------------------------------------------------------------------------------------------------------
	protected Vector2 translateIconPos(Vector2 pos, Vector2 parentSize)
	{
		return pos - parentSize / 2.0f;
	}
}

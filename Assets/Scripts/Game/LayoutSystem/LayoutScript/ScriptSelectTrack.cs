using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrackItem
{
	public ScriptSelectTrack mScript;
	public txNGUIStaticSprite mTrack;
	public txNGUIStaticSprite mChecked;
	public txNGUIText mLabel;
	public txNGUIStaticSprite mTexture;
	public txUIObject mUIGrid;
	public List<txUIObject> mStarList;
	public Vector3 mOriginPosition;
	public Vector3 mOriginScale;
	public int mOriginTextureDepth;
	public int mOriginTrackDepth;
	public int mOriginCheckedDepth;
	public int mOriginLabelDepth;
	public float mControlValueOffset;
	public string mTrackName;
	public TrackItem(ScriptSelectTrack script)
	{
		mScript = script;
		mStarList = new List<txUIObject>() ;
	}
	public void assignWindow(txNGUIStaticSprite root, string trackName)
	{
		mScript.newObject(out mTrack, root, trackName);
		mScript.newObject(out mChecked, mTrack, "Cheaked", 0);
		mScript.newObject(out mLabel, mChecked, "Label", 1);
		mScript.newObject(out mTexture, mTrack, "Texture");
		mScript.newObject(out mUIGrid, mChecked, "UIGrid");
		for (int i = 0; i < mUIGrid.getChildCount(); ++i)
		{
			txUIObject star = mScript.newObject(out star, mUIGrid, "Star" + i, 0);
			mStarList.Add(star);
		}
		mTrackName = trackName;
	}
	public void init()
	{
		mOriginPosition = mTrack.getPosition();
		mOriginScale = mTrack.getScale();
		mOriginTrackDepth = mTrack.getDepth();
		mOriginCheckedDepth = mChecked.getDepth();
		mOriginLabelDepth = mLabel.getDepth();
		mOriginTextureDepth = mTexture.getDepth();
	}
	public void setControlValueOffset(float offset)
	{
		mControlValueOffset = offset;
	}
	public void setSelected(bool select)
	{
		LayoutTools.ACTIVE_WINDOW(mChecked, select);
	}
	public void lerp(TrackItem curItem, TrackItem nextItem, float percent)
	{
		LayoutTools.MOVE_WINDOW(mTrack, MathUtility.lerp(curItem.mOriginPosition, nextItem.mOriginPosition, percent));
		LayoutTools.SCALE_WINDOW(mTrack, MathUtility.lerp(curItem.mOriginScale, nextItem.mOriginScale, percent));
		mTrack.setDepth((int)MathUtility.lerp(curItem.mOriginTrackDepth, nextItem.mOriginTrackDepth, percent));
		mChecked.setDepth((int)MathUtility.lerp(curItem.mOriginCheckedDepth, nextItem.mOriginCheckedDepth, percent));
		mLabel.setDepth((int)MathUtility.lerp(curItem.mOriginLabelDepth, nextItem.mOriginLabelDepth, percent));
		mTexture.setDepth((int)MathUtility.lerp(curItem.mOriginTextureDepth, nextItem.mOriginTextureDepth, percent));
	}
	public void setStar(int star)
	{
		int count = mStarList.Count;
		for(int i = 0; i < count; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mStarList[i], i < star);
		}
	}
}

public class ScriptSelectTrack : LayoutScript
{
	protected txNGUIStaticSprite mTrackRoot;				//	三个赛道的父级
	protected List<TrackItem> mTrackList;				//	赛道
	protected txNGUITextureAnim mSelectTrackTitle;			//	角色 变 赛道序列帧
	protected txNGUIStaticSprite mLeftArrow;              //	左箭头
	protected txNGUIStaticSprite mRightArrow;				//	右箭头
	// 整体UI移动的标识
	protected txNGUIStaticSprite mTrackRootStart;
	protected Vector3 mTrackRootStartPos;
	protected Vector3 mTrackRootEndPos;

	protected txUIObject mControlHelper;
	protected float mStartOffsetValue;		// 本次移动的起始值
	protected float mTargetOffsetValue;		// 本次移动的目标值
	protected float mCurOffsetValue;		// 当前实际的偏移值
	protected int mShowIndex;				// 当前显示选中的下标
	public ScriptSelectTrack(string name, GameLayout layout)
		:
		base(name, layout)
	{
		mTrackList = new List<TrackItem>();
	}
	public override void assignWindow()
	{
		newObject(out mTrackRoot, "TrackRoot");
		newObject(out mTrackRootStart, "TrackPosStart");
		newObject(out mSelectTrackTitle, "SelectTrackTitle");
		newObject(out mLeftArrow, "LeftArrow", 0);
		newObject(out mRightArrow, "RightArrow", 0);
		newObject(out mControlHelper, "ControlHelper", 1);
		for (int i = 0; i < GameDefine.TRACK_COUNT; ++i)
		{
			TrackItem item = new TrackItem(this);
			item.assignWindow(mTrackRoot, "Track" + i);
			mTrackList.Add(item);
		}
	}
	public override void init()
	{
		mLayout.setScriptControlHide(true);
		int trackCount = mTrackList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			mTrackList[i].init();
		}
		trackCount = mTrackList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			mTrackList[i].setControlValueOffset((float)i / trackCount);
		}
		mTrackRootStartPos = mTrackRootStart.getPosition();
		mTrackRootEndPos = mTrackRoot.getPosition();
	}
	public override void onReset()
	{
		LayoutTools.MOVE_WINDOW(mTrackRoot, mTrackRootStartPos);
		LayoutTools.ALPHA_WINDOW(mTrackRoot, 0.3f);
	}
	public override void onGameState()
	{
		int trackCount = mRaceSystem.getTrackCount();
		for(int i = 0; i < trackCount; ++i)
		{
			mTrackList[i].setStar(mRaceSystem.getTrackDifficultyStar(i));
		}
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.MOVE_WINDOW_EX(mTrackRoot, mTrackRootStartPos, mTrackRootEndPos, 1.0f, onTrackRootMoveEnd);
		LayoutTools.ALPHA_WINDOW(mTrackRoot, 0.3f, 1.0f, 1.0f);

		mSelectTrackTitle.setLoop(LOOP_MODE.LM_ONCE);
		mSelectTrackTitle.setAutoHide(false);
		mSelectTrackTitle.stop(true, false);
		mSelectTrackTitle.play();
	}
	public override void onHide(bool immediately, string param)
	{
		if (immediately)
		{
			LayoutTools.HIDE_LAYOUT_FORCE(mType);
			LayoutTools.SCALE_WINDOW(mLeftArrow, new Vector2(0.7f, 0.7f));
			LayoutTools.ALPHA_WINDOW(mLeftArrow, 0.3f);
			LayoutTools.SCALE_WINDOW(mRightArrow, new Vector2(0.7f, 0.7f));
			LayoutTools.ALPHA_WINDOW(mRightArrow, 0.3f);
			LayoutTools.ACTIVE_WINDOW(mLeftArrow, false);
			LayoutTools.ACTIVE_WINDOW(mRightArrow, false);
			LayoutTools.MOVE_WINDOW(mTrackRoot, mTrackRootStartPos);
			LayoutTools.ALPHA_WINDOW(mTrackRoot, 0.0f);
		}
		else
		{
			LayoutTools.SCALE_WINDOW(mLeftArrow, new Vector2(1.0f, 1.0f), new Vector2(0.3f, 0.3f), 0.25f);
			LayoutTools.ALPHA_WINDOW(mLeftArrow, 1.0f, 0.3f, 0.25f);
			LayoutTools.SCALE_WINDOW(mRightArrow, new Vector2(1.0f, 1.0f), new Vector2(0.3f, 0.3f), 0.25f);
			LayoutTools.ALPHA_WINDOW_EX(mRightArrow, 1.0f, 0.3f, 0.25f, onArrowEnd);
		}
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void showIndex(int index)
	{
		int trackCount = mTrackList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			mTrackList[i].setSelected(index == i);
		}
		// 选择上一个或者是从第0个切换到了最后一个
		arrowAnim(index - mShowIndex == -1 || index - mShowIndex == mTrackList.Count - 1);
		// 设置当前选中的下标,并显示图标
		mShowIndex = index;
		// 设置当前起始和目标值
		mStartOffsetValue = mCurOffsetValue;
		mTargetOffsetValue = -mTrackList[mShowIndex].mControlValueOffset;
		// 默认将范围限定到0-1之间
		MathUtility.clampValue(ref mStartOffsetValue, 0.0f, 1.0f, 1.0f);
		MathUtility.clampValue(ref mTargetOffsetValue, 0.0f, 1.0f, 1.0f);
		// 需要保证起始和目标之间不超过0.5,如果超过0.5则转换范围
		if (Mathf.Abs(mStartOffsetValue - mTargetOffsetValue) > 0.5f)
		{
			MathUtility.clampValue(ref mStartOffsetValue, 0.5f, 1.5f, 1.0f);
			MathUtility.clampValue(ref mTargetOffsetValue, 0.5f, 1.5f, 1.0f);
		}
		// 利用辅助物体开始渐变
		LayoutTools.SCALE_WINDOW_EX(mControlHelper, mControlHelper.getScale(), Vector2.one, 0.3f, onHelperScaling, null);
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------------------
	protected void arrowAnim(bool direction)
	{
		if (direction)
		{
			LayoutTools.SCALE_WINDOW(mLeftArrow, new Vector2(0.7f, 0.7f), new Vector2(1.0f, 1.0f), 0.8f);
			LayoutTools.ALPHA_WINDOW(mLeftArrow, 0.3f, 1.0f, 0.8f);
		}
		else
		{
			LayoutTools.SCALE_WINDOW(mRightArrow, new Vector2(0.7f, 0.7f), new Vector2(1.0f, 1.0f), 0.8f);
			LayoutTools.ALPHA_WINDOW(mRightArrow, 0.3f, 1.0f, 0.8f);
		}
	}
	protected void onArrowEnd(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		LayoutTools.ACTIVE_WINDOW(mLeftArrow, false);
		LayoutTools.ACTIVE_WINDOW(mRightArrow, false);
		LayoutTools.MOVE_WINDOW(mTrackRoot, mTrackRootEndPos, mTrackRootStartPos, 0.25f);
		LayoutTools.ALPHA_WINDOW_EX(mTrackRoot, 1.0f, 0.0f, 0.25f, onArrowAlphaDone);
	}
	protected void onArrowAlphaDone(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		LayoutTools.HIDE_LAYOUT_FORCE(mType);
	}
	protected void onTrackRootMoveEnd(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		LayoutTools.ACTIVE_WINDOW(mLeftArrow);
		LayoutTools.ACTIVE_WINDOW(mRightArrow);
		LayoutTools.SCALE_WINDOW(mLeftArrow, new Vector2(0.7f, 0.7f), new Vector2(1.0f, 1.0f), 0.8f);
		LayoutTools.ALPHA_WINDOW(mLeftArrow, 0.3f, 1.0f, 0.8f);
		LayoutTools.SCALE_WINDOW(mRightArrow, new Vector2(0.7f, 0.7f), new Vector2(1.0f, 1.0f), 0.8f);
		LayoutTools.ALPHA_WINDOW(mRightArrow, 0.3f, 1.0f, 0.8f);
	}
	protected void onHelperScaling(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		updateItem(MathUtility.lerp(mStartOffsetValue, mTargetOffsetValue, component.getTremblingPercent()));
	}
	protected void updateItem(float controlValue)
	{
		// 变化时需要随时更新当前值
		mCurOffsetValue = controlValue;
		int trackCount = mTrackList.Count;
		for (int i = 0; i < trackCount; ++i)
		{
			TrackItem item = mTrackList[i];
			float value = controlValue + item.mControlValueOffset;
			while (value >= 1.0f)
			{
				value -= 1.0f;
			}
			int posCount = mTrackList.Count;
			int index = (int)(value * posCount);
			float percent = (value - (float)index / posCount) / (1.0f / posCount);
			item.lerp(mTrackList[index], mTrackList[(index + 1) % posCount], percent);
		}
	}
}

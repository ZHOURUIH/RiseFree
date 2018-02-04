using UnityEngine;
using System.Collections;

public class ScriptLoading : LayoutScript
{
	public txUIStaticTexture[] mBackground;
	public txUIText mProgressLabel;
	public txUIStaticSprite mTitle;
	public txUIStaticTexture mProgressBar;
	public txUIStaticTexture mProgressBack;
	public int mSeclectIndex = 0;
	public int mTrackCount;
	public ScriptLoading(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mTrackCount = mRaceSystem.getTrackCount();
		mBackground = new txUIStaticTexture[mTrackCount];
	}
	public override void assignWindow()
	{
		for (int i = 0; i < mTrackCount; ++i)
		{
			mBackground[i] = newObject<txUIStaticTexture>("Background" + i, 0);
		}
		mProgressLabel = newObject<txUIText>("ProgressLabel", 1);
		mTitle = newObject<txUIStaticSprite>("Title", 1);
		mProgressBar = newObject<txUIStaticTexture>("ProgressBar", 1);
		mProgressBack = newObject<txUIStaticTexture>(mProgressBar, "ProgressBack", 1);
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		for (int i = 0; i < mTrackCount; ++i)
		{
			LayoutTools.ACTIVE_WINDOW(mBackground[i], false);
		}
		LayoutTools.FILL_WINDOW(mProgressBar, 0.0f);
		refreshProgressLabel();
	}
	public override void onShow(bool immediately, string param)
	{
		// 获取选择界面的赛道等级下标，显示对应的背景
		LayoutTools.ACTIVE_WINDOW(mBackground[mRaceSystem.getTrackIndex()]);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public void setProgress(float progress)
	{
		if(progress >= 1.0f)
		{
			LayoutTools.FILL_WINDOW(mProgressBar, progress);
			refreshProgressLabel();
		}
		else
		{
			LayoutTools.FILL_WINDOW_EX(mProgressBar, mProgressBar.getFillPercent(), progress, 0.3f, onProgressing, null);
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------------------
	protected void onProgressing(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		refreshProgressLabel();
	}
	protected void refreshProgressLabel()
	{
		mProgressLabel.setLabel(StringUtility.floatToString(mProgressBar.getFillPercent() * 100.0f, 1) + "%");
	}
}

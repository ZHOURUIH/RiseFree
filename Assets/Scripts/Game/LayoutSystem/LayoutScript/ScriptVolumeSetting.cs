using UnityEngine;
using System.Collections;

public class ScriptVolumeSetting : LayoutScript
{
	protected txUIObject mUIVolumeRoot;
	protected txUISlider mUIVolumeScrollBar;
	protected txUIObject mUIVolumeRootStart;
	protected Vector3 mPosRootStart;
	protected Vector3 mPosRootEnd;
	protected float mVolume;
	public ScriptVolumeSetting(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(ref mUIVolumeRoot, "UIVolumeRoot");
		newObject(ref mUIVolumeScrollBar, mUIVolumeRoot, "UIVolumeScrollBar");
		newObject(ref mUIVolumeRootStart, "UIVolumeRootStart");
	}
	public override void init()
	{
		mPosRootStart = mUIVolumeRootStart.getPosition();
		mPosRootEnd = mUIVolumeRoot.getPosition();
		mLayout.setScriptControlHide(true);
	}
	public override void onReset()
	{
		mVolume = mGameSetting.getCurVolume();
		setVolume(mVolume);
		LayoutTools.MOVE_WINDOW(mUIVolumeRoot, mPosRootStart);
		LayoutTools.ALPHA_WINDOW(mUIVolumeRoot, 0.0f);
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public override void onShow(bool immediately, string param)
	{
		if(immediately)
		{
			LayoutTools.MOVE_WINDOW(mUIVolumeRoot, mPosRootEnd);
			LayoutTools.ALPHA_WINDOW(mUIVolumeRoot, 1.0f);
		}
		else
		{
			LayoutTools.MOVE_WINDOW(mUIVolumeRoot, mPosRootStart, mPosRootEnd, 0.25f);
			LayoutTools.ALPHA_WINDOW(mUIVolumeRoot, 0.0f, 1.0f, 0.25f);
		}
	}
	public override void onHide(bool immediately, string param)
	{
		if (immediately)
		{
			LayoutTools.MOVE_WINDOW(mUIVolumeRoot, mPosRootStart);
			LayoutTools.ALPHA_WINDOW(mUIVolumeRoot, 0.0f);
			LayoutTools.HIDE_LAYOUT_FORCE(mType);
		}
		else
		{
			LayoutTools.MOVE_WINDOW_EX(mUIVolumeRoot, mPosRootEnd, mPosRootStart, 0.25f, onMoveBackDone);
			LayoutTools.ALPHA_WINDOW(mUIVolumeRoot, 1.0f, 0.0f, 0.25f);
		}
	}
	public void setVolume(float value)
	{
		MathUtility.clamp(ref value, 0.0f, 1.0f);
		mUIVolumeScrollBar.setSliderValue(value);
	}
	//------------------------------------------------------------------------------------	
	protected void onMoveBackDone(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		LayoutTools.HIDE_LAYOUT_FORCE(mType);
	}
}
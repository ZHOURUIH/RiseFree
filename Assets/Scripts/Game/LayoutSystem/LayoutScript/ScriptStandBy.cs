using UnityEngine;
using System.Collections;

public class ScriptStandBy : LayoutScript
{
	protected txUIStaticSprite mBackground;
	protected txUITextureAnim  mStartRiding;
	protected txUIObject	   mGameIn;
	protected txUIText		   mGameInText;
	protected txUIStaticSprite mGameInSprite;
	protected txUIObject	   mGameOut;
	protected txUIText		   mGameOutText;
	protected txUIStaticSprite mGameOutSprite;
	public ScriptStandBy(string name, GameLayout layout)
		:
		base(name, layout)
	{ }
	public override void assignWindow()
	{
		newObject(ref mBackground, "Background", 1);
		newObject(ref mStartRiding, "StartRiding", 1);
		newObject(ref mGameIn, "GameIn", 0);
		newObject(ref mGameInText, mGameIn, "GameInText", 1);
		newObject(ref mGameInSprite, mGameIn, "GameInSprite", 1);
		newObject(ref mGameOut, "GameOut", 0);
		newObject(ref mGameOutText, mGameOut, "GameOutText", 1);
		newObject(ref mGameOutSprite, mGameOut, "GameOutSprite", 1);
	}
	public override void init()
	{
		mStartRiding.setInterval(0.06f);
		mStartRiding.setAutoHide(false);
	}
	public override void onShow(bool immediately, string param)
	{
		if(param == "FirstStart")
		{
			mStartRiding.setPlayEndCallback(onPlayDone);
			mStartRiding.play();
		}
		else
		{
			startRidingDone();
		}
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	// --------------------------------------------------------------------------------------------------------------------
	protected void startRidingDone()
	{
		// 播放开始骑行的序列帧(从44帧开始循环播放)
		mStartRiding.setLoop(LOOP_MODE.LM_PINGPONG);
		mStartRiding.setStartIndex(44);
		mStartRiding.setPlayDirection(false);
		mStartRiding.play();
		LayoutTools.ACTIVE_WINDOW(mGameIn);
		LayoutTools.ACTIVE_WINDOW(mGameOut);
	}
	protected void onPlayDone(txUITextureAnim window, object userData, bool isBreak)
	{
		if (isBreak)
		{
			return;
		}
		startRidingDone();
	}
}
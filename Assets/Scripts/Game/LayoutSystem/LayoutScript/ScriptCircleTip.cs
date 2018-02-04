using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScriptCircleTip : LayoutScript
{
	protected txUITextureAnim mSecondCircle;
	protected txUITextureAnim mThirdCircle;
	protected txUITextureAnim mFourthCircle;
	protected txUITextureAnim mFinishRace;
	protected txUITextureAnim mUnfinishRace;
	protected List<txUITextureAnim> mCircleTipList;
	public ScriptCircleTip(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		mCircleTipList = new List<txUITextureAnim>();
	}
	public override void assignWindow()
	{
		mSecondCircle = newObject<txUITextureAnim>("SecondCircle", 0);
		mThirdCircle = newObject<txUITextureAnim>("ThirdCircle", 0);
		mFourthCircle = newObject<txUITextureAnim>("FourthCircle", 0);
		mFinishRace = newObject<txUITextureAnim>("FinishRace", 0);
		mUnfinishRace = newObject<txUITextureAnim>("UnfinishRace", 0);
	}
	public override void init()
	{
		mCircleTipList.Add(mSecondCircle);
		mCircleTipList.Add(mThirdCircle);
		mCircleTipList.Add(mFourthCircle);
	}
	public override void onReset()
	{
		LayoutTools.ACTIVE_WINDOW(mSecondCircle, false);
		LayoutTools.ACTIVE_WINDOW(mThirdCircle, false);
		LayoutTools.ACTIVE_WINDOW(mFourthCircle, false);
		LayoutTools.ACTIVE_WINDOW(mFinishRace, false);
		LayoutTools.ACTIVE_WINDOW(mUnfinishRace, false);
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public void notifyFinishRace(bool finish)
	{
		txUITextureAnim window = finish ? mFinishRace : mUnfinishRace;
		LayoutTools.ACTIVE_WINDOW(window);
		window.stop();
		window.play();
	}
	// circle为已完成的圈数
	public void notifyFinishedCircle(int circle)
	{
		if(circle > 0 && circle < mCircleTipList.Count)
		{
			LayoutTools.ACTIVE_WINDOW(mCircleTipList[circle - 1]);
			mCircleTipList[circle - 1].stop();
			mCircleTipList[circle - 1].play();
		}
	}
}
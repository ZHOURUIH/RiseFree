using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class ScriptAiming : LayoutScript
{
	protected txNGUITextureAnim mAiming;
	protected txNGUITextureAnim mGreenAimingAnim;
	protected float mOriginHeight;
	public ScriptAiming(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mAiming, "AimingAnim");
		newObject(out mGreenAimingAnim, "GreenAimingAnim");
	}
	public override void init()
	{
		mAiming.setAutoHide(false);
		mAiming.setLoop(LOOP_MODE.LM_LOOP);
		mGreenAimingAnim.setLoop(LOOP_MODE.LM_LOOP);
		mOriginHeight = mAiming.getWindowSize().y;
	}
	public override void onReset()
	{
		base.onReset();
		LayoutTools.ACTIVE_WINDOW(mAiming, false);
		LayoutTools.ACTIVE_WINDOW(mGreenAimingAnim, false);
	}
	public override void onShow(bool immediately, string param)
	{
		mAiming.stop(true, false);
		mAiming.setStartIndex(1);
		mAiming.play();
		mGreenAimingAnim.stop(true, false);
		mGreenAimingAnim.setStartIndex(1);
		mGreenAimingAnim.play();
	}
	public void setAiming(Vector2 screenPos, float iconHeight, bool isAim)
	{
		LayoutTools.ACTIVE_WINDOW(mAiming, isAim);
		LayoutTools.ACTIVE_WINDOW(mGreenAimingAnim, !isAim);
		float heightScale = iconHeight / mOriginHeight;
		Vector2 scale = new Vector2(heightScale, heightScale);
		if (isAim)
		{
			mAiming.setLocalPosition(UnityUtility.screenPosToWindowPos(screenPos, mAiming.getParent(), true));
			LayoutTools.SCALE_WINDOW(mAiming, scale);
		}
		else
		{
			mGreenAimingAnim.setLocalPosition(UnityUtility.screenPosToWindowPos(screenPos, mAiming.getParent(), true));
			LayoutTools.SCALE_WINDOW(mGreenAimingAnim, scale);
		}
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public float getOriginHeight() { return mOriginHeight; }
}


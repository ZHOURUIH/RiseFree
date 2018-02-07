using UnityEngine;
using System.Collections;

public class ScriptLogo : LayoutScript
{
	protected txUIStaticTexture mLogoWindow;  //游戏Logo启动图
	protected float             mFadeInTime  = 1.0f;
	protected float             mFadeOutTime = 1.0f;
	protected float				mStayTime = 2.0f;
	public ScriptLogo(string name, GameLayout layout)
		:
		base(name, layout)
	{ }
	public override void assignWindow()
	{
		newObject(ref mLogoWindow, "UILogoStartLoading", 1);
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.ALPHA_WINDOW(mLogoWindow, 0.0f, 1.0f, mFadeInTime);
		LayoutTools.ALPHA_WINDOW_DELAY(this, mLogoWindow, mFadeInTime + mStayTime, 1.0f, 0.0f, mFadeOutTime);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	public float getFullTime() { return mFadeInTime + mStayTime + mFadeOutTime; }
}
using UnityEngine;
using System.Collections;

public class ScriptEndCountDown : LayoutScript
{
	public txUITextureAnim mTimeCountDown;
	public ScriptEndCountDown(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(ref mTimeCountDown, "TimeCountDown", 0);
	}
	public override void init()
	{
		mTimeCountDown.setInterval(0.08f);
	}
	public override void onReset()
	{
		LayoutTools.ACTIVE_WINDOW(mTimeCountDown, false);
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.ACTIVE_WINDOW(mTimeCountDown);
		mTimeCountDown.stop();
		mTimeCountDown.play();
	}
	public override void onHide(bool immediately, string param)
	{
		LayoutTools.ACTIVE_WINDOW(mTimeCountDown, false);
	}
}

using UnityEngine;
using System.Collections;

public class ScriptReturn : LayoutScript
{
	public txNGUIStaticSprite mReturn;						// 返回按钮
	public ScriptReturn(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(ref mReturn, "Return", 0);
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		LayoutTools.ACTIVE_WINDOW(mReturn, false);
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.ACTIVE_WINDOW(mReturn);
	}
	public override void onHide(bool immediately, string param)
	{
		LayoutTools.ACTIVE_WINDOW(mReturn,false);
	}
}

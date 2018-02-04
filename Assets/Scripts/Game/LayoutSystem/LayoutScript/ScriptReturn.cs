using UnityEngine;
using System.Collections;

public class ScriptReturn : LayoutScript
{
	public txUIStaticSprite mReturn;						// 返回按钮
	public ScriptReturn(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mReturn = newObject<txUIStaticSprite>("Return", 0);
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
public class ScriptConfirmSelection : LayoutScript
{
	protected txUIObject mConfirmSelectionRoot;
	protected txUIObject mConfirmTextRoot;
	public ScriptConfirmSelection(string name, GameLayout layout)
		:
		base(name, layout)
	{ }
	public override void assignWindow()
	{
		newObject(out mConfirmSelectionRoot, "ConfirmSelectionRoot");
		newObject(out mConfirmTextRoot, mConfirmSelectionRoot, "ConfirmTextRoot",0);
	}
	public override void onGameState()
	{
		LayoutTools.ACTIVE_WINDOW(mConfirmTextRoot, false);
		LayoutTools.ALPHA_WINDOW(mConfirmSelectionRoot,0.0f);
		LayoutTools.SCALE_WINDOW(mConfirmSelectionRoot,new Vector2(0.0f, 1.0f));
	}
	public override void init()
	{
		
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.SCALE_WINDOW_EX(mConfirmSelectionRoot, new Vector2(0.0f, 1.0f), new Vector2(1.0f, 1.0f), 0.5f, onDoneScalePanel);
		LayoutTools.ALPHA_WINDOW(mConfirmSelectionRoot, 0.0f, 1.0f, 0.5f);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	//---------------------------------------------------------------------------------------------------------
	protected void onDoneScalePanel(ComponentKeyFrameBase component, object userData, bool breakTremling, bool done)
	{
		if (breakTremling)
		{
			return;
		}
		LayoutTools.ACTIVE_WINDOW(mConfirmTextRoot);
	}
}

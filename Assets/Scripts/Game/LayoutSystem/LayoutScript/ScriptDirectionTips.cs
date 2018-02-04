using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class ScriptDirectionTips :LayoutScript
{
	protected txUIStaticTexture mDirectionTexture;
	public ScriptDirectionTips(LAYOUT_TYPE type, string name, GameLayout layout)
		:
		base(type, name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		mDirectionTexture = newObject<txUIStaticTexture>("DirectionTexture");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		LayoutTools.ALPHA_WINDOW(mDirectionTexture,0.3f);
		
	}
	public override void onShow(bool immediately, string param)
	{
		LayoutTools.ALPHA_KEYFRAME_WINDOW(mDirectionTexture,GameDefine.ONE_ZERO_ONE,0.3f,1.0f,1.0f,true);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
}


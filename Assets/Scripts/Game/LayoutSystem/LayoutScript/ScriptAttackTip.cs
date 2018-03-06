using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ScriptAttackTip : LayoutScript
{
	protected txNGUIStaticSprite mAttackEffectRoot;
	public ScriptAttackTip(string name, GameLayout layout)
		:
		base(name, layout)
	{
		;
	}
	public override void assignWindow()
	{
		newObject(out mAttackEffectRoot, "AttackedEffectRoot");
	}
	public override void init()
	{
		;
	}
	public override void onReset()
	{
		LayoutTools.ACTIVE_WINDOW(mAttackEffectRoot, false);
	}
	public override void onShow(bool immediately, string param)
	{
		;
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public override void onHide(bool immediately, string param)
	{
		;
	}
	// 导弹锁定玩家
	public void notifyMissileLockPlayer(bool locked)
	{
		LayoutTools.ACTIVE_WINDOW(mAttackEffectRoot, locked);
		if(locked)
		{
			LayoutTools.ALPHA_KEYFRAME_WINDOW(mAttackEffectRoot, GameDefine.ONE_ZERO_ONE_CURVE, 0.4f, 1.0f, 0.7f, true);
		}
		else
		{
			LayoutTools.ALPHA_WINDOW(mAttackEffectRoot, 1.0f);
		}
	}
}


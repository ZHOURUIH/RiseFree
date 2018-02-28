﻿using UnityEngine;
using System.Collections;

public class txNGUIPanel : txUIObject
{
	protected UIPanel mPanel;
	public txNGUIPanel()
	{
		mType = UI_TYPE.UT_NGUI_PANEL;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mPanel = mObject.GetComponent<UIPanel>();
		if (mPanel == null)
		{
			mPanel = mObject.AddComponent<UIPanel>();
		}
	}
	public override void setDepth(int depth)
	{
		// 不调用基类函数
		mPanel.depth = depth;
	}
	public override int getDepth()
	{
		return mPanel.depth;
	}
}
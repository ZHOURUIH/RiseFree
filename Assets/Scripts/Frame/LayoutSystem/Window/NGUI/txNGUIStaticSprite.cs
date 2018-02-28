﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUIStaticSprite : txUIObject
{
	public UISprite mSprite;
	public txNGUIStaticSprite()
	{
		mType = UI_TYPE.UT_NGUI_STATIC_SPRITE;
	}
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		mSprite = mObject.GetComponent<UISprite>();
		if (mSprite == null)
		{
			mSprite = mObject.AddComponent<UISprite>();
		}
	}
	public UIAtlas getAtlas()
	{
		return mSprite.atlas;
	}
	public string getSpriteName()
	{
		return mSprite.spriteName;
	}
	public void setAtlas(UIAtlas atlas)
	{
		mSprite.atlas = atlas;
	}
	public void setSpriteName(string name)
	{
		mSprite.spriteName = name;
	}
	public Vector2 getWindowSize()
	{
		return new Vector2(mSprite.width, mSprite.height);
	}
	public void setWindowSize(Vector2 size)
	{
		mSprite.SetDimensions((int)size.x, (int)size.y);
	}
	public override void setDepth(int depth)
	{
		mSprite.depth = depth;
		base.setDepth(depth);
	}
	public override int getDepth()
	{
		return mSprite.depth;
	}
	public override void setAlpha(float alpha)
	{
		mSprite.alpha = alpha;
	}
	public override float getAlpha()
	{
		return mSprite.alpha;
	}
	public override void setFillPercent(float percent)
	{
		mSprite.fillAmount = percent;
	}
}
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class ItemParamBase
{
	public SCENE_ITEM mType;
}

public class SceneItemBase : MovableObject
{
	protected SCENE_ITEM mType;
	protected bool mSelfControlDestroy;
	public SceneItemBase(string name, SCENE_ITEM type)
		:
		base(name)
	{
		mType = type;
		mSelfControlDestroy = false;
	}
	public virtual void init(ItemParamBase param)
	{
		base.init();
		if(param.mType != mType)
		{
			UnityUtility.logError("item param error!");
		}
	}
	public override void destroy()
	{
		mObjectManager.destroyObject(mObject);
		base.destroy();
	}
	public SCENE_ITEM getItemType() { return mType; }
	public bool getSelfControlDestroy() { return mSelfControlDestroy; }
	// 通知道具需要销毁
	public virtual void notifyDestroy() { }
	public virtual void onEffective(Character player) { }
	//---------------------------------------------------------------------------------------------------------------------
	protected void createObject(string path, Vector3 pos)
	{
		createObject(path, pos, Vector3.zero,Vector3.one);
	}
	protected void createObject(string path, Vector3 pos, Vector3 rot, Vector3 scale)
	{
		setObject(mObjectManager.createObject(mItemManager.getManagerObject(), path));
		setDestroyObject(false);
		mTransform.localPosition = pos;
		mTransform.localEulerAngles = rot;
		mTransform.localScale = scale;
	}
}
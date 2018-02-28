﻿using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class LayoutAsyncInfo
{
	public string			mName;
	public int				mRenderOrder;
	public bool				mIsNGUI;
	public LAYOUT_TYPE		mType;
	public GameLayout		mLayout;
	public GameObject		mLayoutObject;
	public LayoutAsyncDone	mCallback;
}

public class GameLayoutManager : FrameComponent
{
	protected Dictionary<Type, List<LAYOUT_TYPE>>	mScriptMappingList;
	protected Dictionary<LAYOUT_TYPE, Type>			mScriptRegisteList;
	protected Dictionary<LAYOUT_TYPE, string>		mLayoutTypeToName;
	protected Dictionary<string, LAYOUT_TYPE>		mLayoutNameToType;
	protected Dictionary<LAYOUT_TYPE, GameLayout>	mLayoutTypeList;
	protected txUIObject							mNGUIRoot;
	protected txUIObject							mUGUIRoot;
	protected Dictionary<string, LayoutAsyncInfo>	mLayoutAsyncList;
	public GameLayoutManager(string name)
		:
		base(name)
	{
		mScriptMappingList = new Dictionary<Type, List<LAYOUT_TYPE>>();
		mScriptRegisteList = new Dictionary<LAYOUT_TYPE, Type>();
		mLayoutTypeToName = new Dictionary<LAYOUT_TYPE, string>();
		mLayoutNameToType = new Dictionary<string, LAYOUT_TYPE>();
		mLayoutTypeList = new Dictionary<LAYOUT_TYPE, GameLayout>();
		mLayoutAsyncList = new Dictionary<string, LayoutAsyncInfo>();
	}
	public override void init()
	{
		mNGUIRoot = LayoutScript.newUIObject<txUIObject>("NGUIRoot", null, null, UnityUtility.getGameObject(null, "NGUIRoot", true));
		mUGUIRoot = LayoutScript.newUIObject<txUIObject>("UGUIRoot", null, null, UnityUtility.getGameObject(null, "UGUIRoot", true));
	}
	public GameObject getNGUIRootObject()
	{
		return mNGUIRoot.mObject;
	}
	public txUIObject getNGUIRoot()
	{
		return mNGUIRoot;
	}
	public GameObject getUGUIRootObject()
	{
		return mUGUIRoot.mObject;
	}
	public txUIObject getUGUIRoot()
	{
		return mUGUIRoot;
	}
	public override void update(float elapsedTime)
	{
		foreach (var layout in mLayoutTypeList)
		{
			layout.Value.update(elapsedTime);
		}
	}
	public override void destroy()
	{
		foreach(var item in mLayoutTypeList)
		{
			item.Value.destroy();
		}
		mLayoutTypeList.Clear();
		mLayoutTypeToName.Clear();
		mLayoutNameToType.Clear();
		mLayoutAsyncList.Clear();
		mNGUIRoot = null;
		mUGUIRoot = null;
		base.destroy();
	}
	public string getLayoutNameByType(LAYOUT_TYPE type)
	{
		if (mLayoutTypeToName.ContainsKey(type))
		{
			return mLayoutTypeToName[type];
		}
		else
		{
			UnityUtility.logError("can not find LayoutType: " + type);
		}
		return "";
	}
	public LAYOUT_TYPE getLayoutTypeByName(string name)
	{
		if (mLayoutNameToType.ContainsKey(name))
		{
			return mLayoutNameToType[name];
		}
		else
		{
			UnityUtility.logError("can not  find LayoutName:" + name);
		}
		return LAYOUT_TYPE.LT_MAX;
	}
	public GameLayout getGameLayout(LAYOUT_TYPE type)
	{
		if (mLayoutTypeList.ContainsKey(type))
		{
			return mLayoutTypeList[type];
		}
		return null;
	}
	public T getScript<T>(LAYOUT_TYPE type) where T : LayoutScript
	{
		GameLayout layout = getGameLayout(type);
		if (layout != null)
		{
			return layout.getScript() as T;
		}
		return null;
	}
	public int getScriptMappingCount(Type classType)
	{
		return mScriptMappingList[classType].Count;
	}
	public GameLayout createLayout(LAYOUT_TYPE type, int renderOrder, bool async, LayoutAsyncDone callback, bool isNGUI)
	{
		if (mLayoutTypeList.ContainsKey(type))
		{
			if (async && callback != null)
			{
				callback(mLayoutTypeList[type]);
				return null;
			}
			else
			{
				return mLayoutTypeList[type];
			}
		}
		string name = getLayoutNameByType(type);
		string path = isNGUI ? CommonDefine.R_NGUI_PREFAB_PATH : CommonDefine.R_UGUI_PREFAB_PATH;
		GameObject layoutParent = isNGUI ? mNGUIRoot.mObject : mUGUIRoot.mObject;
		// 如果是异步加载则,则先加入列表中
		if (async)
		{
			LayoutAsyncInfo info = new LayoutAsyncInfo();
			info.mName = name;
			info.mType = type;
			info.mRenderOrder = renderOrder;
			info.mLayout = null;
			info.mLayoutObject = null;
			info.mIsNGUI = isNGUI;
			info.mCallback = callback;
			mLayoutAsyncList.Add(info.mName, info);
			bool ret = mResourceManager.loadResourceAsync<GameObject>(path + name, onLayoutPrefabAsyncDone, true);
			if (!ret)
			{
				UnityUtility.logError("can not find layout : " + name);
			}
			return null;
		}
		else
		{
			UnityUtility.instantiatePrefab(layoutParent, path + name);
			GameLayout layout = new GameLayout();
			addLayoutToList(layout, name, type);
			layout.init(type, name, renderOrder, isNGUI);
			return layout;
		}
	}
	public void destroyLayout(LAYOUT_TYPE type)
	{
		GameLayout layout = getGameLayout(type);
		if (layout == null)
		{
			return;
		}
		removeLayoutFromList(layout);
		layout.destroy();
	}
	public LayoutScript createScript(string name, GameLayout layout)
	{
		return UnityUtility.createInstance<LayoutScript>(mScriptRegisteList[layout.getType()], name, layout);
	}
	public List<BoxCollider> getAllLayoutBoxCollider()
	{
		List<BoxCollider> allBoxList = new List<BoxCollider>();
		foreach (var layout in mLayoutTypeList)
		{
			List<BoxCollider> boxList = layout.Value.getAllBoxCollider();
			foreach (var box in boxList)
			{
				allBoxList.Add(box);
			}
		}
		return allBoxList;
	}
	public void registeLayout(Type classType, LAYOUT_TYPE type, string name)
	{
		mLayoutTypeToName.Add(type, name);
		mLayoutNameToType.Add(name, type);
		mScriptRegisteList.Add(type, classType);
		if(!mScriptMappingList.ContainsKey(classType))
		{
			List<LAYOUT_TYPE> list = new List<LAYOUT_TYPE>();
			mScriptMappingList.Add(classType, list);
		}
		mScriptMappingList[classType].Add(type);
	}
	public int getLayoutCount()
	{
		return mLayoutTypeToName.Count;
	}
	//----------------------------------------------------------------------------------------------------------------------------------------------------
	protected void addLayoutToList(GameLayout layout, string name, LAYOUT_TYPE type)
	{
		mLayoutTypeList.Add(type, layout);
	}
	protected void removeLayoutFromList(GameLayout layout)
	{
		if (layout != null)
		{
			mLayoutTypeList.Remove(layout.getType());
		}
	}
	protected void onLayoutPrefabAsyncDone(UnityEngine.Object res, object userData)
	{
		LayoutAsyncInfo info = mLayoutAsyncList[res.name];
		mLayoutAsyncList.Remove(res.name);
		info.mLayoutObject = UnityUtility.instantiatePrefab(null, (GameObject)res);
		info.mLayout = new GameLayout();
		addLayoutToList(info.mLayout, info.mName, info.mType);
		GameObject layoutParent = info.mIsNGUI ? mNGUIRoot.mObject : mUGUIRoot.mObject;
		UnityUtility.setNormalProperty(ref info.mLayoutObject, layoutParent, info.mName, Vector3.one, Vector3.zero, Vector3.zero);
		info.mLayout.init(info.mType, info.mName, info.mRenderOrder, info.mIsNGUI);
		info.mCallback(info.mLayout);
	}
}
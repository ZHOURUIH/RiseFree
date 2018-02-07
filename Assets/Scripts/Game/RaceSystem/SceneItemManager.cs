using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class SceneItemManager : FrameComponent
{
	protected Dictionary<SCENE_ITEM, Type> mItemRegisteList;
	protected Dictionary<SCENE_ITEM, List<SceneItemBase>> mItemList;
	protected GameObject mItemManagerObject;
	protected List<int> mDelayCreateList;		// 创建道具的命令ID列表,用于中断命令
	protected List<int> mDelayDestroyList;		// 强制销毁道具的命令ID列表,用于中断命令
	public SceneItemManager(string name)
		:base(name)
	{
		mItemRegisteList = new Dictionary<SCENE_ITEM, Type>();
		mItemList = new Dictionary<SCENE_ITEM, List<SceneItemBase>>();
		mDelayCreateList = new List<int>();
		mDelayDestroyList = new List<int>();
	}
	public override void init()
	{
		registeItem<SceneItemBox>(SCENE_ITEM.SI_ITEM_BOX);
		registeItem<SceneLandMine>(SCENE_ITEM.SI_LAND_MINE);
		registeItem<SceneMissile>(SCENE_ITEM.SI_MISSILE);
		mItemManagerObject = UnityUtility.getGameObject(mGameFramework.getGameFrameObject(), "ItemManager", true);
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		foreach (var item in mItemList)
		{
			int count = item.Value.Count;
			for(int i = 0; i < count; ++i)
			{
				item.Value[i].update(elapsedTime);
			}
		}
	}
	public T createItem<T>(SCENE_ITEM type, ItemParamBase param) where T : SceneItemBase
	{
		return createItem(type, param) as T;
	}
	// 创建一个道具
	public SceneItemBase createItem(SCENE_ITEM type, ItemParamBase param)
	{
		if(!mItemRegisteList.ContainsKey(type))
		{
			return null;
		}
		SceneItemBase item = UnityUtility.createInstance<SceneItemBase>(mItemRegisteList[type], type);
		item.init(param);
		if (mItemList.ContainsKey(type))
		{
			mItemList[type].Add(item);
		}
		else
		{
			List<SceneItemBase> itemList = new List<SceneItemBase>();
			itemList.Add(item);
			mItemList.Add(type, itemList);
		}
		return item;
	}
	// 延迟创建一个道具
	public void createItemDelay(SCENE_ITEM type, float time, ItemParamBase param)
	{
		CommandSceneItemManagerCreateItem cmd = newCmd(out cmd, true, true);
		cmd.mParam = param;
		cmd.mItemType = type;
		pushDelayCommand(cmd, this, time);
		cmd.addStartCommandCallback(onCreateItemCmd, null);
		mDelayCreateList.Add(cmd.mAssignID);
	}
	// 销毁一个道具,force为真则表示直接销毁,否则通知道具开始销毁
	public void destroyItem(SceneItemBase item, bool force = false)
	{
		if(!item.getSelfControlDestroy() || force)
		{
			item.destroy();
			mItemList[item.getItemType()].Remove(item);
		}
		else
		{
			item.notifyDestroy();
		}
	}
	// 延迟销毁一个道具,销毁时是强制销毁
	public void destroyItemDelay(SceneItemBase item, float delayTime)
	{
		CommandSceneItemManagerDestroyItem cmd = newCmd(out cmd, true, true);
		cmd.mSceneItem = item;
		pushDelayCommand(cmd, this, delayTime);
		cmd.addStartCommandCallback(onDestroyItemCmd, null);
		mDelayDestroyList.Add(cmd.mAssignID);
	}
	// 销毁所有指定类型的道具
	public void destroyAllItem(SCENE_ITEM type = SCENE_ITEM.SI_MAX)
	{
		// 销毁所有道具
		if(type == SCENE_ITEM.SI_MAX)
		{
			foreach(var item in mItemList)
			{
				int count = item.Value.Count;
				for (int i = 0; i < count; ++i)
				{
					(item.Value)[i].destroy();
				}
			}
			mItemList.Clear();
		}
		// 销毁指定类型的道具
		else
		{
			if (!mItemList.ContainsKey(type))
			{
				return;
			}
			int count = mItemList[type].Count;
			for (int i = 0; i < count; ++i)
			{
				mItemList[type][i].destroy();
			}
			mItemList[type].Clear();
		}
	}
	// 通知本局比赛已经结束
	public void notifyTrackFinish()
	{
		// 中断所有创建道具的命令
		int countCreate = mDelayCreateList.Count;
		for(int i = 0; i < countCreate; ++i)
		{
			mCommandSystem.interruptCommand(mDelayCreateList[i]);
		}
		mDelayCreateList.Clear();
		// 中断所有销毁道具的命令
		int countDestroy = mDelayDestroyList.Count;
		for (int i = 0; i < countDestroy; ++i)
		{
			mCommandSystem.interruptCommand(mDelayDestroyList[i]);
		}
		mDelayDestroyList.Clear();
	}
	public GameObject getManagerObject() { return mItemManagerObject; }
	//------------------------------------------------------------------------------------------------
	protected void registeItem<T>(SCENE_ITEM type)where T : SceneItemBase
	{
		mItemRegisteList.Add(type, typeof(T));
	}
	protected void onCreateItemCmd(object user_data, Command cmd)
	{
		mDelayCreateList.Remove(cmd.mAssignID);
	}
	protected void onDestroyItemCmd(object user_data, Command cmd)
	{
		mDelayDestroyList.Remove(cmd.mAssignID);
	}
}
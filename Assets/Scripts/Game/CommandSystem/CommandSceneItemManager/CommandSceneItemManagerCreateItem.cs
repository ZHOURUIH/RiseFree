using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 创建一个道具,只能由道具管理器调用
class CommandSceneItemManagerCreateItem : Command
{
	public ItemParamBase mParam;
	public SCENE_ITEM mItemType;
	public override void init()
	{
		base.init();
		mParam = null;
		mItemType = SCENE_ITEM.SI_MAX;
	}
	public override void execute()
	{
		SceneItemManager sceneItemManager = mReceiver as SceneItemManager;
		sceneItemManager.createItem(mItemType, mParam);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : type : " + mItemType;
	}
}


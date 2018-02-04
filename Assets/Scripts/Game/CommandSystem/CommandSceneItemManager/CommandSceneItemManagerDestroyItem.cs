using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

// 强制销毁一个道具,只能由道具管理器调用
class CommandSceneItemManagerDestroyItem : Command
{
	public SceneItemBase mSceneItem;
	public override void init()
	{
		base.init();
		mSceneItem = null;
	}
	public override void execute()
	{
		SceneItemManager sceneItemManager = mReceiver as SceneItemManager;
		sceneItemManager.destroyItem(mSceneItem, true);
	}
	public override string showDebugInfo()
	{
		SCENE_ITEM type = mSceneItem != null ? mSceneItem.getItemType() : SCENE_ITEM.SI_MAX;
		return this.GetType().ToString() + " : type : " + type;
	}
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class ItemBoxParam : ItemParamBase
{
	public ItemBoxParam()
	{
		mType = SCENE_ITEM.SI_ITEM_BOX;
	}
	public Vector3 mPosition;
}

public class SceneItemBox : SceneItemBase
{
	protected ItemBoxObject mBoxObjectComponent;
	protected string mExplodeParticleName;
	protected string mBoxModelNodeName;
	public SceneItemBox(SCENE_ITEM type)
		:
		base(type)
	{
		mSelfControlDestroy = true;
		mExplodeParticleName = "ItemBox_SOmobileFX_galaxy_2";
		mBoxModelNodeName = "DJ_xiangzi";
	}
	public override void init(ItemParamBase param)
	{
		base.init(param);
		ItemBoxParam boxParam = param as ItemBoxParam;
		// 创建道具箱子模型并添加脚本
		createObject(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.ITEM_BOX, boxParam.mPosition);
		UnityUtility.getGameObject(mObject, mExplodeParticleName, true).SetActive(false);
		mBoxObjectComponent = mObject.AddComponent<ItemBoxObject>();
		mBoxObjectComponent.setItem(this);
		// 播放箱子动画(并且设置播放的速度为0.5f)
		GameObject diXiangzi = UnityUtility.getGameObject(mObject, mBoxModelNodeName, true);
		Animation animation = diXiangzi.GetComponent<Animation>();
		string animName = "idle";
		animation.Play(animName);
		animation[animName].speed = 0.5f;
	}
	// 道具箱子生效的效果
	public override void onEffective(Character player)
	{
		// 角色获得一个随机道具
		CommandCharacterGetItem cmd = newCmd(out cmd);
		cmd.mItemType = generateItemType();
		pushCommand(cmd, player);
		// 开始销毁道具箱子
		mItemManager.destroyItem(this);
	}
	// 通知开始销毁
	public override void notifyDestroy()
	{
		base.notifyDestroy();
		// 播放爆炸特效,隐藏箱子模型
		UnityUtility.getGameObject(mObject, mExplodeParticleName, true).SetActive(true);
		UnityUtility.getGameObject(mObject, mBoxModelNodeName, true).SetActive(false);
		// 1秒后销毁道具
		mItemManager.destroyItemDelay(this, 1.0f);
		// 30秒后在该位置重新克隆一个箱子
		ItemBoxParam param = new ItemBoxParam();
		param.mPosition = mObject.transform.position;
		mItemManager.createItemDelay(mType, GameDefine.CREATE_ITEM_BOX_TIME, param);
	}
	public PLAYER_ITEM generateItemType()
	{
		PLAYER_ITEM[] typeList = new PLAYER_ITEM[] { PLAYER_ITEM.PI_MISSILE, PLAYER_ITEM.PI_SHIELD, PLAYER_ITEM.PI_TURBO, PLAYER_ITEM.PI_LAND_MINE };
		int itemIndex = MathUtility.randomInt(0, typeList.Length - 1);
		return typeList[itemIndex];
	}
}
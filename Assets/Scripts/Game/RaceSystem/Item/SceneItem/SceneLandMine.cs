using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class LandmineParam : ItemParamBase
{
	public LandmineParam()
	{
		mType = SCENE_ITEM.SI_LAND_MINE;
	}
	public Vector3 mPosition;
}

// 地雷
public class SceneLandMine : SceneItemBase
{
	protected LandmineObject mLandMineComponent;
	protected string mExplodeParticleName;
	protected string mLandmineModelName;
	public SceneLandMine(SCENE_ITEM type)
		:
		base("Landmine", type)
	{
		mSelfControlDestroy = true;
		mExplodeParticleName = "Landmine_3D_FireHit_01";
		mLandmineModelName = "DJ_dilei";
	}
	public override void init(ItemParamBase param)
	{
		base.init(param);
		LandmineParam landmineParam = param as LandmineParam;
		// 创建地雷模型并添加地雷脚本
		createObject(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.LANDMINE, landmineParam.mPosition);
		UnityUtility.getGameObject(mObject, mExplodeParticleName, true).SetActive(false);
		UnityUtility.getGameObject(mObject, mLandmineModelName, true).SetActive(true);
		mLandMineComponent = mObject.GetComponent<LandmineObject>();
		if(mLandMineComponent == null)
		{
			mLandMineComponent = mObject.AddComponent<LandmineObject>();
		}
		mLandMineComponent.setItem(this);
	}
	// 地雷生效的效果
	public override void onEffective(Character player)
	{
		// 通知角色被攻击
		CommandCharacterAttacked cmdAttack = newCmd(out cmdAttack);
		cmdAttack.mAttackSource = mType;
		pushCommand(cmdAttack, player);
		if (player.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			GameTools.PLAY_AUDIO_OBJECT(this, SOUND_DEFINE.SD_LANDMINE_EXPLODE);
		}
		// 开始销毁地雷
		mItemManager.destroyItem(this);
	}
	// 通知开始销毁
	public override void notifyDestroy()
	{
		base.notifyDestroy();
		// 播放爆炸特效并隐藏地雷模型
		UnityUtility.getGameObject(mObject, mExplodeParticleName, true).SetActive(true);
		UnityUtility.getGameObject(mObject, mLandmineModelName, true).SetActive(false);
		// 0.7秒后销毁物体
		mItemManager.destroyItemDelay(this, 0.7f);
	}
}
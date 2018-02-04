using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class MissileParam : ItemParamBase
{
	public MissileParam()
	{
		mType = SCENE_ITEM.SI_MISSILE;
	}
	public Vector3 mPosition;
	public MovableObject mTarget;
}

// 导弹
public class SceneMissile : SceneItemBase
{
	protected MissileObject mMissileComponent;
	protected string mExplodeParticleName;
	protected string mExplodeParticleName1;
	protected string mMissileModelName;
	protected MovableObject mTarget;
	protected CharacterOther mCharacterOther;
	public SceneMissile(SCENE_ITEM type)
		:
		base(type)
	{
		mSelfControlDestroy = true;
		mExplodeParticleName = "Missile_M_3D_Hit_02";
		mExplodeParticleName1 = "Missile_M_3D_EarthMagic_02";
		mMissileModelName = "DJ_daodan";
	}
	public override void init(ItemParamBase param)
	{
		base.init(param);
		MissileParam missileParam = param as MissileParam;
		// 创建导弹模型并添加导弹脚本
		createObject(GameDefine.R_SCENE_ITEM_PREFAB_PATH + GameDefine.MISSILE, missileParam.mPosition);
		mMissileComponent = mObject.AddComponent<MissileObject>();
		mMissileComponent.setItem(this);
		ObjectTools.TRACK_TARGET(this, 30.0f, Vector3.up, missileParam.mTarget, null);
		mTarget = missileParam.mTarget;
		UnityUtility.getGameObject(mObject, mExplodeParticleName, true).SetActive(false);
		UnityUtility.getGameObject(mObject, mExplodeParticleName1, true).SetActive(false);
	}
	public override void update(float elapsedTime)
	{
		base.update(elapsedTime);
		//计算导弹的朝向
		Vector3 originRot = Vector3.up;
		Vector3 targetDir = mTarget.getWorldPosition() - getWorldPosition();
		Vector3 normal = MathUtility.normalize(Vector3.Cross(originRot, targetDir));
		float angle = MathUtility.getAngleBetweenVector(originRot, targetDir) * Mathf.Rad2Deg;
		resetRotation();
		rotateAround(normal, angle);
	}
	public override void onEffective(Character player)
	{
		// 通知角色被攻击
		CommandCharacterAttacked cmdAttack = newCmd(out cmdAttack);
		cmdAttack.mAttackSource = mType;
		pushCommand(cmdAttack, player);
		// 开始销毁导弹
		mItemManager.destroyItem(this);
		mCharacterOther = player as CharacterOther;
	}
	// 通知开始销毁
	public override void notifyDestroy()
	{
		base.notifyDestroy();
		// 停止追踪,播放爆炸特效并隐藏导弹模型
		ObjectTools.TRACK_TARGET(this, 10.0f, null, null);
		UnityUtility.getGameObject(mObject, mExplodeParticleName, true).SetActive(true);
		UnityUtility.getGameObject(mObject, mExplodeParticleName1, true).SetActive(true);
		UnityUtility.getGameObject(mObject, mMissileModelName, true).SetActive(false);
		// 0.7秒后销毁物体
		mItemManager.destroyItemDelay(this, 0.7f);
	}
}
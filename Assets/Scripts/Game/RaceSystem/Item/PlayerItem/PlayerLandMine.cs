using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

// 地雷
public class PlayerLandMine : PlayerItemBase
{
	public PlayerLandMine(PLAYER_ITEM type)
		:
		base(type)
	{
		;
	}
	public override void init()
	{
		;
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public override void use(CharacterOther player)
	{
		// 在角色当前位置放置一个地雷
		Vector3 curRot = player.getRotation();
		Vector3 dir = MathUtility.getVectorFromAngle(curRot.y * Mathf.Deg2Rad);
		Vector3 pos = player.getPosition() - dir * 2.0f;
		LandmineParam param = new LandmineParam();
		param.mPosition = pos;
		SceneLandMine landmine = mItemManager.createItem<SceneLandMine>(SCENE_ITEM.SI_LAND_MINE, param);
		GameTools.PLAY_AUDIO_OBJECT(landmine, SOUND_DEFINE.SD_PUT_LANDMINE);
		// 使用后立即移除背包中的道具
		CommandCharacterRemoveItem cmdRemove = newCmd(out cmdRemove);
		cmdRemove.mItem = this;
		pushCommand(cmdRemove, player);
	}
}
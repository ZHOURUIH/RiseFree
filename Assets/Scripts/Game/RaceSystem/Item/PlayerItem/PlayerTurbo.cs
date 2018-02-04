using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

// 加速
public class PlayerTurbo : PlayerItemBase
{
	public PlayerTurbo(PLAYER_ITEM type)
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
		// 添加加速状态
		BOOL result = new BOOL();
		CommandCharacterAddState cmdState = newCmd(out cmdState);
		cmdState.mState = PLAYER_STATE.PS_SPRINT;
		cmdState.mResult = result;
		pushCommand(cmdState, player);
		// 添加状态成功后移除背包中的道具
		if(result.mValue)
		{
			CommandCharacterRemoveItem cmdRemove = newCmd(out cmdRemove);
			cmdRemove.mItem = this;
			pushCommand(cmdRemove, player);
		}
	}
}
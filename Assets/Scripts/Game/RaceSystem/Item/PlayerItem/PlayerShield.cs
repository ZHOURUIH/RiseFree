using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

// 护盾
public class PlayerShield : PlayerItemBase
{
	public PlayerShield(PLAYER_ITEM type)
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
		// 添加护盾状态
		BOOL result = new BOOL();
		CommandCharacterAddState cmdState = newCmd(out cmdState);
		cmdState.mState = PLAYER_STATE.PS_PROTECTED;
		cmdState.mResult = result;
		pushCommand(cmdState, player);
		// 使用成功后移除背包中的道具
		if(result.mValue)
		{
			CommandCharacterRemoveItem cmdRemove = newCmd(out cmdRemove);
			cmdRemove.mItem = this;
			pushCommand(cmdRemove, player);
		}
	}
}
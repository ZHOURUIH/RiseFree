using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

// 导弹
public class PlayerMissile : PlayerItemBase
{
	public PlayerMissile(PLAYER_ITEM type)
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
		// 如果已经在瞄准状态中,则不能使用
		if (player.hasState(PLAYER_STATE.PS_AIM))
		{
			return;
		}
		// 给角色添加瞄准状态
		CommandCharacterAddState cmdCharacterAddState = newCmd(out cmdCharacterAddState);
		cmdCharacterAddState.mState = PLAYER_STATE.PS_AIM;
		pushCommand(cmdCharacterAddState, player);
	}
}
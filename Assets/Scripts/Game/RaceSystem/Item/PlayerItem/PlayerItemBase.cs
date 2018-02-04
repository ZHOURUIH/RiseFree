using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerItemBase : GameBase
{
	protected PLAYER_ITEM mType;
	public PlayerItemBase(PLAYER_ITEM type)
	{
		mType = type;
	}
	public virtual void init()
	{
		;
	}
	public virtual void destroy()
	{
		;
	}
	public virtual void update(float elapsedTime)
	{
		;
	}
	public virtual void use(CharacterOther player) { }
	public PLAYER_ITEM getItemType() { return mType; }
}
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerPack : GameBase
{
	protected static Dictionary<PLAYER_ITEM, Type> mRegisteList;
	protected CharacterOther mPlayer;
	protected PlayerItemBase[] mPackItem;
	protected int mItemCount;
	protected int mSelectedIndex;
	public PlayerPack()
	{
		mPackItem = new PlayerItemBase[GameDefine.PACK_ITEM_COUNT];
		mItemCount = 0;
		mSelectedIndex = 0;
		if (mRegisteList == null)
		{
			mRegisteList = new Dictionary<PLAYER_ITEM, Type>();
		}
	}
	public void init(CharacterOther player)
	{
		mPlayer = player;
		if(mRegisteList.Count == 0)
		{
			registePlayerItem<PlayerMissile>(PLAYER_ITEM.PI_MISSILE);
			registePlayerItem<PlayerShield>(PLAYER_ITEM.PI_SHIELD);
			registePlayerItem<PlayerTurbo>(PLAYER_ITEM.PI_TURBO);
			registePlayerItem<PlayerLandMine>(PLAYER_ITEM.PI_LAND_MINE);
		}
	}
	public void destroy()
	{
		int maxCount = mPackItem.Length;
		for (int i = 0; i < maxCount; ++i)
		{
			if (mPackItem[i] != null)
			{
				mPackItem[i].destroy();
				mPackItem[i] = null;
			}
		}
		mItemCount = 0;
	}
	public int addItem(PLAYER_ITEM type)
	{
		PlayerItemBase item = createPlayerItem(type);
		int maxCount = mPackItem.Length;
		for(int i = 0; i < maxCount; ++i)
		{
			if(mPackItem[i] == null)
			{
				++mItemCount;
				mPackItem[i] = item;
				return i;
			}
		}
		return 0;
	}
	public void useItem(int index)
	{
		if(mPackItem[index] != null)
		{
			mPackItem[index].use(mPlayer);
		}
	}
	public int removeItem(PlayerItemBase item)
	{
		int itemIndex = 0;
		int maxCount = mPackItem.Length;
		for (int i = 0; i < maxCount; ++i)
		{
			if(mPackItem[i] == item)
			{
				--mItemCount;
				mPackItem[i] = null;
				itemIndex = i;
				break;
			}
		}
		return itemIndex;
	}
	public PlayerItemBase getCurItem() { return mPackItem[mSelectedIndex]; }
	public bool isFull() { return getItemCount() >= mPackItem.Length; }
	public int getItemCount() { return mItemCount; }
	public int getSelectedIndex() { return mSelectedIndex; }
	public void setItemIndex(int index){ mSelectedIndex = index;}
	public int getNextItem()
	{
		return (mSelectedIndex + 1) % GameDefine.PACK_ITEM_COUNT;
	}
	//----------------------------------------------------------------------------------------------------------------
	protected PlayerItemBase createPlayerItem(PLAYER_ITEM type)
	{
		return UnityUtility.createInstance<PlayerItemBase>(mRegisteList[type], type);
	}
	protected void registePlayerItem<T>(PLAYER_ITEM type) where T : PlayerItemBase
	{
		mRegisteList.Add(type, typeof(T));
	}
}
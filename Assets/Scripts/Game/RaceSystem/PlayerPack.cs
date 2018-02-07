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
		mSelectedIndex = -1;
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
		int startIndex = mSelectedIndex == -1 ? 0 : mSelectedIndex;
		for (int i = 0; i < maxCount; ++i)
		{
			int index = (startIndex + i) % maxCount;
			if (mPackItem[index] == null)
			{
				++mItemCount;
				mPackItem[index] = item;
				return index;
			}
		}
		return 0;
	}
	public void useItem(int index)
	{
		if(index < 0 || index >= mPackItem.Length)
		{
			return;
		}
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
	public bool canChangeSelection()
	{
		if (mItemCount == 0 || (mItemCount == 1 && getCurItem() != null))
		{
			return false;
		}
		return true;
	}
	public PlayerItemBase getCurItem()
	{
		if(mSelectedIndex == -1)
		{
			return null;
		}
		return mPackItem[mSelectedIndex];
	}
	public int getFirstItemIndex(PLAYER_ITEM item)
	{
		int maxCount = mPackItem.Length;
		for (int i = 0; i < maxCount; ++i)
		{
			if(mPackItem[i] != null && mPackItem[i].getItemType() == item)
			{
				return i;
			}
		}
		return -1;
	}
	public bool isFull() { return getItemCount() >= mPackItem.Length; }
	public int getItemCount() { return mItemCount; }
	public int getSelectedIndex() { return mSelectedIndex; }
	public void setItemIndex(int index){ mSelectedIndex = index;}
	public int getNextNotEmptyIndex()
	{
		if(mSelectedIndex == -1)
		{
			return 0;
		}
		int nextIndex = -1;
		int curIndex = mSelectedIndex;
		int maxCount = mPackItem.Length;
		for (int i = 0; i < maxCount - 1; ++i)
		{
			int index = (curIndex + 1 + i) % maxCount;
			if (mPackItem[index] != null)
			{
				nextIndex = index;
				break;
			}
		}
		return nextIndex;
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
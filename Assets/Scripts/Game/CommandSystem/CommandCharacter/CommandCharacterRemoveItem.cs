using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CommandCharacterRemoveItem : Command
{
	public PlayerItemBase mItem;
	public override void init()
	{
		base.init();
		mItem = null;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterOther player = character as CharacterOther;
		if (player == null)
		{
			return;
		}
		PlayerPack pack = player.getPlayerPack();
		int itemIndex = -1;
		if(mItem != null)
		{
			itemIndex = pack.removeItem(mItem);
		}
		// 通知布局
		if(player.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			mScriptProps.removeProps(itemIndex);
		}
	}
	public override string showDebugInfo()
	{
		string itemType = mItem != null ? mItem.getItemType().ToString() : "";
		return this.GetType().ToString() + " : item : " + itemType;
	}
}


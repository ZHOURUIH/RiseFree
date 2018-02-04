using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
class CommandCharacterGetItem : Command
{
	public PLAYER_ITEM mItemType;
	public override void init()
	{
		base.init();
		mItemType = PLAYER_ITEM.PI_MAX;
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
		if (!pack.isFull())
		{
			 int itemIndex = pack.addItem(mItemType);
			// 如果获取道具的是玩家自己,通知游戏界面
			if (player.isType(CHARACTER_TYPE.CT_MYSELF))
			{
				mScriptProps.addProps(mItemType, itemIndex);
			}
			// 如果获取道具的是游戏中Al(直接使用游戏中的道具)
			else
			{
			  CharacterControllerAI characterControllerAI = player.getFirstActiveComponent<CharacterControllerAI>();
				characterControllerAI.notifyAIGetBoxItem(pack.getCurItem());
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : Item type : " + mItemType;
	}
}
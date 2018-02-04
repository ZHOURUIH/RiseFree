using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CommandCharacterSelectItem : Command
{
	public int mItemIndex;
	public override void init()
	{
		base.init();
		mItemIndex = 0;
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
		pack.setItemIndex(mItemIndex);
		// 通知布局
		if(player.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			mScriptProps.showIndex(mItemIndex);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : item index : " + mItemIndex;
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CommandCharacterUseItem : Command
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
		pack.useItem(mItemIndex);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : item index : " + mItemIndex;
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CommandCharacterSelectItem : Command
{
	public int mIndex;
	public override void init()
	{
		base.init();
		mIndex = -1;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterOther player = character as CharacterOther;
		if (player == null)
		{
			return;
		}
		// 移除瞄准状态
		if(player.hasState(PLAYER_STATE.PS_AIM))
		{
			CommandCharacterRemoveState cmdState = newCmd(out cmdState);
			cmdState.mState = PLAYER_STATE.PS_AIM;
			pushCommand(cmdState, character);
		}

		PlayerPack pack = player.getPlayerPack();
		int nextNotNullIndex = mIndex != -1 ? mIndex : pack.getNextNotEmptyIndex();
		if (nextNotNullIndex == -1)
		{
			return;
		}
		pack.setItemIndex(nextNotNullIndex);
		// 通知布局
		if(player.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			mScriptProps.showIndex(nextNotNullIndex);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : index : " + mIndex;
	}
}
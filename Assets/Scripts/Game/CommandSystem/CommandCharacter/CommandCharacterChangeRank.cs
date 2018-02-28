using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CommandCharacterChangeRank : Command
{
	public int mRank;
	public override void init()
	{
		base.init();
		mRank = 0;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		data.mRank = mRank;
		// 通知布局
		mScriptPlayerRaceInfo.notifyRank(data.mNumber, data.mRank);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : rank : " + mRank;
	}
}
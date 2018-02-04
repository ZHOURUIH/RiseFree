using UnityEngine;
using System.Collections;

public class CommandCharacterAddState : Command
{
	public PLAYER_STATE mState;
	public override void init()
	{
		base.init();
		mState = PLAYER_STATE.PS_MAX;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		if(!character.isType(CHARACTER_TYPE.CT_OTHER))
		{
			return;
		}
		CharacterOther other = character as CharacterOther;
		bool ret = other.getStateMachine().addState(mState);
		if(mResult != null)
		{
			mResult.mValue = ret;
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : state : " + mState;
	}
}
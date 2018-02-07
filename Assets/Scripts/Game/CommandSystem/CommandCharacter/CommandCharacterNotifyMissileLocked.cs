using UnityEngine;
using System.Collections;

// 通知玩家被导弹锁定
public class CommandCharacterNotifyMissileLocked : Command
{
	public bool mLocked;
	public SceneMissile mMissile;
	public override void init()
	{
		base.init();
		mMissile = null;
		mLocked = false;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterOther other = character as CharacterOther;
		if(other == null)
		{
			return;
		}
		other.notifyMissileLocked(mMissile, mLocked);
		if(other.isType(CHARACTER_TYPE.CT_MYSELF) && other.isLockedByMissile())
		{
			mScriptAttackTip.notifyMissileLockPlayer(mLocked);
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : locked : " + mLocked;
	}
}
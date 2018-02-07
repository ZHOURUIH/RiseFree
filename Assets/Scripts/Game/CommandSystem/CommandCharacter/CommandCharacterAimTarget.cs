using UnityEngine;
using System.Collections;

public class CommandCharacterAimTarget : Command
{
	public CharacterOther mTarget;
	public override void init()
	{
		base.init();
		mTarget = null;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		if(!character.isType(CHARACTER_TYPE.CT_OTHER))
		{
			return;
		}
		if (character.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			if (mTarget == null)
			{
				Vector2 screenPos = UnityUtility.worldPosToScreenPos(character.getWorldPosition() + GameDefine.AIM_OFFSET);
				mScriptAiming.setAiming(screenPos, mScriptAiming.getOriginHeight(), false);
			}
			else
			{
				Vector2 screenPos = UnityUtility.worldPosToScreenPos(mTarget.getWorldPosition());
				Vector2 screenPosHead = UnityUtility.worldPosToScreenPos(mTarget.getWorldPosition() + GameDefine.AIM_OFFSET);
				float distance = MathUtility.getLength(screenPosHead - screenPos);
				if (distance < mScriptAiming.getOriginHeight() / 2)
				{
					distance = mScriptAiming.getOriginHeight() / 2;
				}
				mScriptAiming.setAiming((screenPos + screenPosHead) / 2.0f, distance, true);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString();
	}
}
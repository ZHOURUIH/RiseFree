using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterMyself : CharacterOther
{
	public CharacterMyself(CHARACTER_TYPE type, string name)
		:
		base(type, name)
	{
		;
	}
	public override void initComponents()
	{
		base.initComponents();
	}
	public override void update(float elapsedTime)
	{
		mCharacterData.mTurnAngle = mGameInputManager.getStickAngle();
		base.update(elapsedTime);
	}
	public override bool isType(CHARACTER_TYPE type) { return type == CHARACTER_TYPE.CT_MYSELF || base.isType(type); }
}
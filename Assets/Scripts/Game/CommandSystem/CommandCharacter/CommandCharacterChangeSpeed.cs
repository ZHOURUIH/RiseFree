using UnityEngine;
using System.Collections;

public class CommandCharacterChangeSpeed : Command
{
	public float mSpeed = 0.0f;
	public override void init()
	{
		base.init();
		mSpeed = 0.0f;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		CharacterData data = character.getCharacterData();
		data.mLastSpeed = data.mSpeed;
		// 如果速度相同,则直接返回
		if (MathUtility.isFloatEqual(data.mSpeed, mSpeed))
		{
			return;
		}
		data.mSpeed = mSpeed;
		if (mSpeed > data.mMaxSpeed)
		{
			data.mMaxSpeed = mSpeed;
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : speed : " + mSpeed;
	}
}

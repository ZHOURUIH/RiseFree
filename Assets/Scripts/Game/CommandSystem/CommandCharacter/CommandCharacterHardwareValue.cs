using UnityEngine;
using System.Collections;

public class CommandCharacterHardwareSpeed : Command
{
	public float mSpeed;
	public bool mExternalSpeed;	// 是否为外部输入速度,外部输入速度是指由输入设备产生的速度
	public bool mDirectSpeed;	// 是否是直接设置为指定速度,而不是逐渐增加到指定速度
	public override void init()
	{
		base.init();
		mSpeed = 0.0f;
		mExternalSpeed = true;
		mDirectSpeed = false;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		if(!character.isType(CHARACTER_TYPE.CT_OTHER))
		{
			return;
		}
		CharacterOther charOther = character as CharacterOther;
		CharacterSpeedHardware componentSpeed = character.getFirstActiveComponent<CharacterSpeedHardware>();
		if (componentSpeed != null)
		{
			if(!mExternalSpeed || charOther.getProcessExternalSpeed())
			{
				componentSpeed.setHardwareSpeed(mSpeed, mDirectSpeed);
			}
		}
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : speed : " + mSpeed + " , direct speed : " + mDirectSpeed;
	}
}

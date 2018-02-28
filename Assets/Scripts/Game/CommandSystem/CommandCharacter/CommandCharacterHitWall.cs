using UnityEngine;
using System.Collections;

// 角色撞到了墙
public class CommandCharacterHitWall : Command
{
	public float mAngle;	// 速度方向反方向与墙法线的夹角,角度制
	public override void init()
	{
		base.init();
		mAngle = 0.0f;
	}
	public override void execute()
	{
		Character character = mReceiver as Character;
		MathUtility.clamp(ref mAngle, 0.0f, 45.0f);
		CommandCharacterChangeSpeed cmdSpeed = newCmd(out cmdSpeed,false);
		cmdSpeed.mSpeed = Mathf.Sin(Mathf.Abs(mAngle * Mathf.Deg2Rad)) * character.getCharacterData().mSpeed;
		pushCommand(cmdSpeed, character);
		// 同时需要降低硬件速度组件中从当前速度加速到目标速度的快慢
		CharacterSpeedHardware speedHardware = character.getFirstComponent<CharacterSpeedHardware>();
		speedHardware.setAcceleration(0.0f);
	}
	public override string showDebugInfo()
	{
		return this.GetType().ToString() + " : angle : " + mAngle;
	}
}

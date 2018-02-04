using UnityEngine;

// 冲刺状态
public class SprintState : PlayerState
{
	private GameObject mTurbo;
	protected float mLastTargetSpeed;
	public SprintState(PLAYER_STATE type)
		:
		base(type)
	{
		// 该状态只持续4秒
		mStateTime = 4.0f;
		mEnableStatus.mProcessExternalSpeed = false;
	}
	public override void enter()
	{
		mTurbo = UnityUtility.instantiatePrefab(mPlayer.getObject(), GameDefine.R_PARTICLE_PREFAB_PATH + GameDefine.TURBO);
		mTurbo.transform.localPosition = new Vector3(0.5f, -3.0f, 2.0f);
		// 保存当前速度,将速度提升到60km/h
		CharacterSpeedHardware component = mPlayer.getFirstComponent<CharacterSpeedHardware>();
		mLastTargetSpeed = component.getTargetSpeed();
		CommandCharacterHardwareSpeed cmd = newCmd(out cmd);
		cmd.mSpeed = MathUtility.KMHtoMS(60.0f);
		cmd.mExternalSpeed = false;
		pushCommand(cmd, mPlayer);
		// 提升加速度,加速度会自动恢复到正常值
		component.setAcceleration(50.0f);
	}
	public override void leave()
	{
		UnityUtility.destroyGameObject(mTurbo);
		// 恢复之前的速度
		CommandCharacterHardwareSpeed cmd = newCmd(out cmd);
		cmd.mSpeed = mLastTargetSpeed;
		cmd.mExternalSpeed = false;
		pushCommand(cmd, mPlayer);
	}
}
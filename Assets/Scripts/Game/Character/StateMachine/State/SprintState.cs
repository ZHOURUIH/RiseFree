using UnityEngine;

// 冲刺状态
public class SprintState : PlayerState
{
	private GameObject mTurbo;
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
		float speed = mPlayer.getCharacterData().mSpeed;
		CommandCharacterHardwareSpeed cmd = newCmd(out cmd);
		cmd.mSpeed = speed + GameUtility.getSprintIncreaseSpeed(speed);
		cmd.mExternalSpeed = false;
		pushCommand(cmd, mPlayer);
		// 提升加速度,加速度会自动恢复到正常值
		CharacterSpeedHardware component = mPlayer.getFirstComponent<CharacterSpeedHardware>();
		component.setAcceleration(50.0f);
		if(mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_SPRINT);
		}
	}
	public override void leave()
	{
		UnityUtility.destroyGameObject(mTurbo);
	}
}
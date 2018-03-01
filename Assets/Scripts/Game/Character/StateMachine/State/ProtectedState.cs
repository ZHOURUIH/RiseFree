using UnityEngine;
using System.Collections;

// 护盾状态
public class ProtectedState : PlayerState
{
	protected GameObject mShield;
	public ProtectedState(PLAYER_STATE type)
		:
		base(type)
	{
		mStateTime = 4.0f;
	}
	public override void enter()
	{
		mShield = UnityUtility.instantiatePrefab(mPlayer.getObject(), GameDefine.R_PARTICLE_PREFAB_PATH + GameDefine.SHIELD);
		mShield.transform.localPosition = new Vector3(0.0f, 1.0f, 0.0f);
		if (mPlayer.isType(CHARACTER_TYPE.CT_MYSELF))
		{
			GameTools.PLAY_AUDIO_UI(mScriptGlobalAudio.getAudioWindow(), SOUND_DEFINE.SD_SHIELD_OPEN);
		}	
	}
	public override void leave()
	{
		UnityUtility.destroyGameObject(mShield);
	}
}

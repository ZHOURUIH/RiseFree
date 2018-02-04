using UnityEngine;
using System.Collections;

public class SceneRegister : GameBase
{
	public void registeAllScene()
	{
		registeScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		registeScene<SnowMountain>(GameDefine.SNOW_MOUNTAIN);
		registeScene<PrimaryTrack>(GameDefine.PRIMARY_TRACK);
		registeScene<Desert>(GameDefine.DESERT);
		registeScene<AncientCity>(GameDefine.ANCIENT_CITY);
	}
	//------------------------------------------------------------------------------
	protected void registeScene<T>(string name) where T : SceneInstance
	{
		mSceneSystem.registeScene<T>(name);
	}
}
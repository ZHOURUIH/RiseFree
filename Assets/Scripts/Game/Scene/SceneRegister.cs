using UnityEngine;
using System.Collections;

public class SceneRegister : GameBase
{
	public static void registeAllScene()
	{
		registeScene<RoleDisplay>(GameDefine.ROLE_DISPLAY);
		registeScene<SnowMountain>(GameDefine.SNOW_MOUNTAIN);
		registeScene<PrimaryTrack>(GameDefine.PRIMARY_TRACK);
		registeScene<Desert>(GameDefine.DESERT);
		registeScene<AncientCity>(GameDefine.ANCIENT_CITY);
	}
	//------------------------------------------------------------------------------
	protected static void registeScene<T>(string name) where T : SceneInstance
	{
		mSceneSystem.registeScene(typeof(T), name);
	}
}
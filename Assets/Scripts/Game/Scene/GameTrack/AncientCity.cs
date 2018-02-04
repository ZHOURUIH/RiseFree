using UnityEngine;

public class AncientCity : GameTrackBase
{
	public GameObject mCamera;
	public GameObject mWater;
	public AncientCity(string name)
		:base(name)
	{
		mCircleCount = 2;
	}
	public override void init()
	{
		base.init();
	}
	public override void destroy()
	{
		base.destroy();
	}
	//---------------------------------------------------------------------------------------------
	protected override void findGameObject()
	{
		base.findGameObject();
		mCamera = UnityUtility.getGameObject(mRoot, "Camera");
		mWater = UnityUtility.getGameObject(mRoot, "WaterProDaytime");
	}
	protected override void initGameObject()
	{
		base.initGameObject();
		// 禁用当前场景的摄像机
		mCamera.SetActive(false);
		mWater.SetActive(true);
	}
}
using UnityEngine;

public class SnowMountain : GameTrackBase
{
	public GameObject mCamera;
	public SnowMountain(string name)
		:base(name)
	{
		mCircleCount = 3;
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
	}
	protected override void initGameObject()
	{
		base.initGameObject();
		// 禁用当前场景的摄像机
		mCamera.SetActive(false);
	}
}
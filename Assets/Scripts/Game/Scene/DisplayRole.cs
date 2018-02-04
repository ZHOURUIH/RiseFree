using UnityEngine;

public class RoleDisplay : SceneInstance
{
	public GameObject mGround;
	public GameObject mCameraPositionObject0;
	public GameObject mCameraPositionObject1;
	public GameObject mRolePositionObject0;
	public GameObject mRolePositionObject1;
	public GameObject mRolePositionObject2;
	public Transform mCameraTransform0;
	public Transform mCameraTransform1;
	public Vector3 mRolePosition0;
	public Vector3 mRolePosition1;
	public Vector3 mRolePosition2;
	public RoleDisplay(string name)
		:base(name)
	{
		;
	}
	public override void init()
	{
		base.init();
	}
	public override void destroy()
	{
		base.destroy();
	}
	//------------------------------------------------------------------------------------------------------
	protected override void findGameObject()
	{
		mGround = UnityUtility.getGameObject(mRoot, "Ground");
		mCameraPositionObject0 = UnityUtility.getGameObject(mRoot, "CameraPosition0");
		mCameraPositionObject1 = UnityUtility.getGameObject(mRoot, "CameraPosition1");
		mRolePositionObject0 = UnityUtility.getGameObject(mRoot, "RolePosition0");
		mRolePositionObject1 = UnityUtility.getGameObject(mRoot, "RolePosition1");
		mRolePositionObject2 = UnityUtility.getGameObject(mRoot, "RolePosition2");
	}
	protected override void initGameObject()
	{
		mGround.AddComponent<MoveGround>();
		mCameraPositionObject0.SetActive(false);
		mCameraPositionObject1.SetActive(false);
		mCameraTransform0 = mCameraPositionObject0.transform;
		mCameraTransform1 = mCameraPositionObject1.transform;
		mRolePosition0 = mRolePositionObject0.transform.position;
		mRolePosition1 = mRolePositionObject1.transform.position;
		mRolePosition2 = mRolePositionObject2.transform.position;
	}
}
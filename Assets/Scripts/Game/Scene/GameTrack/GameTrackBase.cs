using UnityEngine;
using System.Collections.Generic;

public class GameTrackBase : SceneInstance
{
	public List<List<Vector3>> mItemBoxPoints;
	public GameObject mDaoJu;
	public GameObject mLuDian;
	public GameObject mStartPoint;
	public GameObject mJumpPoint;
	public List<Vector3> mStartPointList;
	public List<Vector3> mJumpPointList;
	public int mCircleCount;
	public int mDifficultyStar;
	public GameTrackBase(string name)
		:base(name)
	{
		mItemBoxPoints = new List<List<Vector3>>();
		mStartPointList = new List<Vector3>();
		mJumpPointList = new List<Vector3>();
	}
	public override void init()
	{
		base.init();
		TrackInfo info = mRaceSystem.getTrackInfo(mName);
		mCircleCount = info.mCircleCount;
		mDifficultyStar = info.mDifficultyStar;
	}
	public override void destroy()
	{
		// 销毁所有道具
		mItemManager.destroyAllItem();
		mItemManager.notifyTrackFinish();
		// 清空路点
		mWayPointManager.clear();
		mStartPointList.Clear();
		mJumpPointList.Clear();
		base.destroy();
	}
	//---------------------------------------------------------------------------------------------
	protected override void findGameObject()
	{
		mDaoJu = UnityUtility.getGameObject(mRoot, "daoju");
		mLuDian = UnityUtility.getGameObject(mRoot, "ludian");
		mStartPoint = UnityUtility.getGameObject(mRoot, "StartPoint");
		mJumpPoint = UnityUtility.getGameObject(mRoot, "JumpPoint");
	}
	protected override void initGameObject()
	{
		// 初始化并创建所有道具箱子
		initItemBoxPoints(mDaoJu);
		createAllItemBox();
		// 初始化路点管理器,起始点,跳跃点
		mWayPointManager.initPoints(mLuDian);
		initStartPoint(mStartPoint);
		initJumpPoint(mJumpPoint);
	}
	protected void initItemBoxPoints(GameObject pointsRoot)
	{
		mItemBoxPoints.Clear();
		if(pointsRoot == null)
		{
			return;
		}
		Transform rootTrans = pointsRoot.transform;
		int count0 = rootTrans.childCount;
		for (int i = 0; i < count0; ++i)
		{
			List<Vector3> pointsList = new List<Vector3>();
			Transform groupTrans = rootTrans.GetChild(i);
			int count1 = groupTrans.childCount;
			for (int j = 0; j < count1; ++j)
			{
				pointsList.Add(groupTrans.GetChild(j).position);
			}
			mItemBoxPoints.Add(pointsList);
		}
	}
	protected void createAllItemBox()
	{
		ItemBoxParam param = new ItemBoxParam();
		int count0 = mItemBoxPoints.Count;
		for (int i = 0; i < count0; ++i)
		{
			int count1 = mItemBoxPoints[i].Count;
			for (int j = 0; j < count1; ++j)
			{
				param.mPosition = mItemBoxPoints[i][j];
				mItemManager.createItem(SCENE_ITEM.SI_ITEM_BOX, param);
			}
		}
	}
	protected void initStartPoint(GameObject pointsRoot)
	{
		mStartPointList.Clear();
		if (pointsRoot == null)
		{
			return;
		}
		Transform rootTrans = pointsRoot.transform;
		int count0 = rootTrans.childCount;
		for (int i = 0; i < count0; ++i)
		{
			Transform child = rootTrans.GetChild(i);
			mStartPointList.Add(child.position);
		}
	}
	protected void initJumpPoint(GameObject pointsRoot)
	{
		mJumpPointList.Clear();
		if(pointsRoot == null)
		{
			return;
		}
		Transform rootTrans = pointsRoot.transform;
		int count0 = rootTrans.childCount;
		for (int i = 0; i < count0; ++i)
		{
			Transform child = rootTrans.GetChild(i);
			mJumpPointList.Add(child.position);
		}
	}
}
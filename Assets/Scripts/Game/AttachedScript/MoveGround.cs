using UnityEngine;
using System.Collections;

public class MoveGround : MonoBehaviour 
{
	protected Transform mLeftGround;
	protected Transform mRightGround;
	protected Transform[] mGround;
	protected Vector3 mLeftPos;
	protected Vector3 mRightPos;
	protected float mSpeed = 2.0f;
	protected int mGroundCount = 4;
	protected float mSingleLength = 42.0f;
	void Awake()
	{
		mGround = new Transform[mGroundCount];
		mLeftGround = UnityUtility.getGameObject(null, "LeftGround", true).GetComponent<Transform>();
		mRightGround = UnityUtility.getGameObject(null, "RightGround", true).GetComponent<Transform>();
		for (int i = 0; i < mGroundCount; ++i)
		{
			mGround[i] = UnityUtility.getGameObject(null, "Ground" + i, true).GetComponent<Transform>();
		}
		mLeftPos = mLeftGround.position;
		mRightPos = mRightGround.position;
	}
	void Update() 
	{
		for (int i = 0; i < mGroundCount; ++i)
		{
			mGround[i].transform.Translate(Vector3.up * mSpeed * Time.deltaTime);
			Vector3 pos = mGround[i].transform.position;
			if(pos.z <= mLeftPos.z)
			{
				int targetIndex = 0;
				if(i == 0)
				{
					targetIndex = 1;
				}
				else if(i == 1)
				{
					targetIndex = 0;
				}
				else if(i == 2)
				{
					targetIndex = 3;
				}
				else if(i == 3)
				{
					targetIndex = 2;
				}
				pos.z = mGround[targetIndex].transform.position.z + mSingleLength;
				mGround[i].transform.position = pos;
			}
		}
	}
}

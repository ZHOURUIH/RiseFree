using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using UnityEngine;

public class TrackInfo
{
	public string mName;
	public int mCircleCount;
	public int mDifficultyStar;
	public TrackInfo(string name, int circle, int star)
	{
		mName = name;
		mCircleCount = circle;
		MathUtility.clamp(ref mCircleCount, 1, GameDefine.MAX_CIRCLE_COUNT);
		mDifficultyStar = star;
	}
}

public class CollidePair
{
	public CharacterOther mPlayer0;
	public CharacterOther mPlayer1;
	public CollidePair(CharacterOther player0, CharacterOther player1)
	{
		mPlayer0 = player0;
		mPlayer1 = player1;
	}
	public bool isPair(CharacterOther player0, CharacterOther player1)
	{
		return (mPlayer0 == player0 && mPlayer1 == player1) || (mPlayer0 == player1 && mPlayer1 == player0);
	}
}

public class RankInfo
{
	public float mTotalDistance;
	public float mRunTime;
	public RankInfo(CharacterOther player)
	{
		mTotalDistance = player.getCharacterData().mTotalDistance;
		mRunTime = player.getCharacterData().mRunTime;
	}
}

public class RankCompare : IComparer<RankInfo>
{
	int IComparer<RankInfo>.Compare(RankInfo value0, RankInfo value1)
	{
		// 距离越短,排名越低
		// 距离一致,则时间越长排名越低
		if (value0.mTotalDistance < value1.mTotalDistance || value0.mRunTime > value1.mRunTime)
		{
			return -1;
		}
		else if(value0.mTotalDistance > value1.mTotalDistance || value0.mRunTime < value1.mRunTime)
		{
			return 1;
		}
		else
		{
			return 0;
		}
	}
}

public class RaceSystem : FrameComponent
{
	protected float mSystemTime;
	protected int mTrackIndex;
	protected List<TrackInfo> mTrackInfoList;
	protected List<CollidePair> mCollidePairList;
	public RaceSystem(string name)
		:
		base(name)
	{
		mTrackInfoList = new List<TrackInfo>();
		mSystemTime = 0.0f;
		mCollidePairList = new List<CollidePair>();
	}
	public override void init()
	{
		mTrackInfoList.Add(new TrackInfo(GameDefine.PRIMARY_TRACK, 3, 1));
		mTrackInfoList.Add(new TrackInfo(GameDefine.SNOW_MOUNTAIN, 3, 2));
		mTrackInfoList.Add(new TrackInfo(GameDefine.DESERT, 3, 3));
		mTrackInfoList.Add(new TrackInfo(GameDefine.ANCIENT_CITY, 2, 4));
	}
	public override void destroy()
	{
		base.destroy();
	}
	public override void update(float elapsedTime)
	{
		// 在游戏中才会计时和计算名称
		if (mGameSceneManager.getCurScene().atProcedure(PROCEDURE_TYPE.PT_MAIN_GAMING))
		{
			// 系统计时
			int oldTime = (int)mSystemTime;
			mSystemTime += elapsedTime;
			if ((int)mSystemTime != oldTime)
			{
				mScriptTopTime.setTime((int)mSystemTime);
			}
			// 计算名次,先判断里程,如果里程一致则判断时间
			SortedDictionary<RankInfo, List<CharacterOther>> distanceList = new SortedDictionary<RankInfo, List<CharacterOther>>(new RankCompare());
			SortedDictionary<int, CharacterOther> playerList = mRoleSystem.getAllPlayer();
			foreach(var item in playerList)
			{
				RankInfo rankInfo = new RankInfo(item.Value);
				if (!distanceList.ContainsKey(rankInfo))
				{
					distanceList.Add(rankInfo, new List<CharacterOther>());
				}
				distanceList[rankInfo].Add(item.Value);
			}
			int rank = 0;
			int playerCount = playerList.Count;
			foreach (var item in distanceList)
			{
				int listCount = item.Value.Count;
				for (int i = 0; i < listCount; ++i)
				{
					int curRank = playerCount - 1 - rank;
					if (curRank != item.Value[i].getCharacterData().mRank)
					{
						CommandCharacterChangeRank cmdRank = newCmd(out cmdRank);
						cmdRank.mRank = curRank;
						pushCommand(cmdRank, item.Value[i]);
					}
					++rank;
				}
			}
			int count = mCollidePairList.Count;
			if (count > 0)
			{
				// 计算每个碰撞对的碰撞效果
				for(int i = 0; i < count; ++i)
				{
					CollidePair pair = mCollidePairList[i];
					CharacterData data0 = pair.mPlayer0.getCharacterData();
					CharacterData data1 = pair.mPlayer1.getCharacterData();
					Vector3 playerSpeed0 = MathUtility.rotateVector3(Vector3.forward * data0.mSpeed, data0.mSpeedRotation.y * Mathf.Deg2Rad);
					Vector3 playerSpeed1 = MathUtility.rotateVector3(Vector3.forward * data1.mSpeed, data1.mSpeedRotation.y * Mathf.Deg2Rad);
					// 将速度分解为沿角色连线方向和连线的法线方向
					Vector3 line = pair.mPlayer0.getPosition() - pair.mPlayer1.getPosition();
					float lineYaw = MathUtility.getVectorYaw(line);
					playerSpeed0 = MathUtility.rotateVector3(playerSpeed0, -lineYaw);
					playerSpeed1 = MathUtility.rotateVector3(playerSpeed1, -lineYaw);
					// 将沿连线方向的速度交换
					MathUtility.swap(ref playerSpeed0.z, ref playerSpeed1.z);
					playerSpeed0 = MathUtility.rotateVector3(playerSpeed0, lineYaw);
					playerSpeed1 = MathUtility.rotateVector3(playerSpeed1, lineYaw);
					data0.mSpeedRotation.y = MathUtility.getVectorYaw(playerSpeed0) * Mathf.Rad2Deg;
					data1.mSpeedRotation.y = MathUtility.getVectorYaw(playerSpeed1) * Mathf.Rad2Deg;
					data0.mSpeed = MathUtility.getLength(playerSpeed0);
					data1.mSpeed = MathUtility.getLength(playerSpeed1);
				}
				mCollidePairList.Clear();
			}
		}
	}
	public void addCollidePair(CharacterOther player0, CharacterOther player1)
	{
		bool hasPair = false;
		int count = mCollidePairList.Count;
		for(int i = 0; i < count; ++i)
		{
			if(mCollidePairList[i].isPair(player0, player1))
			{
				hasPair = true;
				break;
			}
		}
		if(!hasPair)
		{
			mCollidePairList.Add(new CollidePair(player0, player1));
		}
	}
	public void notifyGameStart()
	{
		mSystemTime = 0.0f;
	}
	public void notifyGameFinish()
	{
		mCollidePairList.Clear();
	}
	public void setTrackIndex(int track){mTrackIndex = track;}
	public int getTrackIndex(){return mTrackIndex;}
	public int getTrackCount(){return mTrackInfoList.Count;}
	public int getLastTrackIndex()
	{
		return (mTrackIndex - 1 + getTrackCount()) % getTrackCount();
	}
	public int getNextTrackIndex()
	{
		return (mTrackIndex + 1) % getTrackCount();
	}
	public GameTrackBase getCurGameTrack()
	{
		return mSceneSystem.getScene<GameTrackBase>(getTrackName());
	}
	public string getTrackName()
	{
		return mTrackInfoList[mTrackIndex].mName;
	}
	public int getTrackDifficultyStar(int index)
	{
		return mTrackInfoList[index].mDifficultyStar;
	}
	public TrackInfo getTrackInfo(string name)
	{
		int count = mTrackInfoList.Count;
		for(int i = 0; i < count; ++i)
		{
			if(mTrackInfoList[i].mName == name)
			{
				return mTrackInfoList[i];
			}
		}
		return null;
	}
}
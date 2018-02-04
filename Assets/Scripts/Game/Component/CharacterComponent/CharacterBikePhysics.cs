using UnityEngine;
using System;
using System.Collections;

// 调整单车来适应当前地形的位置和旋转,以及碰撞,跳跃逻辑
// 运动方向,当前位置计算
public class CharacterBikePhysics : GameComponent
{
	protected CharacterOther mCharacter;
	protected CharacterData mData;
	protected Rigidbody mRigidbody;
	protected Transform[] mWheelCenter;    // 车轮的中心点,第0个是前轮,第1个是后轮
	protected float mLastTurnAngle;
	public CharacterBikePhysics(Type type, string name)
		:
	   base(type, name)
	{
		mWheelCenter = new Transform[2];
	}
	public override void init(ComponentOwner owner)
	{
		base.init(owner);
		mCharacter = owner as CharacterOther;
		if(mCharacter == null)
		{
			UnityUtility.logError("CharacterBikePhysics can only attach to CharacterOther!");
		}
		mData = mCharacter.getCharacterData();
	}
	public void notifyModelInited()
	{
		mRigidbody = mCharacter.getRigidBody();
		GameObject charNode = mCharacter.getObject();
		mWheelCenter[0] = UnityUtility.getGameObject(charNode, "FrontWheelCenter", true).transform;
		mWheelCenter[1] = UnityUtility.getGameObject(charNode, "BackWheelCenter", true).transform;
	}
	public override void setActive(bool active)
	{
		base.setActive(active);
		// 激活单车物理组件时,需要更新保存的移动方向
		if(active)
		{
			mData.mSpeedRotation = mCharacter.getRotation();
		}
	}
	public override void update(float elapsedTime)
	{
		// 转向判断
		float turnAngle = 0.0f;
		if (mCharacter.getProcessTurn())
		{
			turnAngle = mData.mTurnAngle;
			// 只有转向角度增加时才会判断转弯
			if (Mathf.Abs(turnAngle) > Mathf.Abs(mLastTurnAngle) && Mathf.Abs(turnAngle) >= GameDefine.TURN_ANGLE)
			{
				CommandCharacterTurn cmdTurn = newCmd(out cmdTurn, false);
				cmdTurn.mAngle = turnAngle;
				pushCommand(cmdTurn, mCharacter);
			}
			mLastTurnAngle = turnAngle;
		}
		// 方向
		if (!MathUtility.isFloatZero(turnAngle))
		{
			float angleDelta = turnAngle * elapsedTime;
			// 一帧的旋转角度不能太大
			MathUtility.clamp(ref angleDelta, -GameDefine.MAX_REFLECT_ANGLE, GameDefine.MAX_REFLECT_ANGLE);
			mData.mSpeedRotation.y += angleDelta;
		}
		// 根据运动方向调整角色朝向
		mCharacter.setYaw(mData.mSpeedRotation.y);
		// 位置
		Vector3 moveDir = MathUtility.getVectorFromAngle(mData.mSpeedRotation.y * Mathf.Deg2Rad);
		Vector3 lastPosition = mCharacter.getPosition();
		if (!MathUtility.isFloatZero(mData.mSpeed) || !MathUtility.isFloatZero(mData.mVerticalSpeed))
		{
			Vector3 delta = (moveDir * mData.mSpeed + Vector3.up * mData.mVerticalSpeed) * elapsedTime;
			mCharacter.setPosition(lastPosition + delta);
		}

		// 计算与墙面的碰撞
		// 从运动之前的位置向运动方向发射一条射线
		Ray dirRay = new Ray(lastPosition, moveDir);
#if UNITY_EDITOR
		Debug.DrawLine(dirRay.origin, dirRay.origin + dirRay.direction * 10.0f, Color.red);
#endif
		RaycastHit dirRet;
		// 检测前方是否有墙
		if (Physics.Raycast(dirRay, out dirRet, 1000.0f, GameUtility.mWallLayer))
		{
			float wallDis = MathUtility.getLength(dirRet.point - lastPosition);
			// 减去上一次的位置与当前位置的差值后,如果小于单车前半部分的长度则认为是碰到了墙,计算出反弹方向
			if (wallDis - MathUtility.getLength(lastPosition - mCharacter.getPosition()) < GameDefine.BIKE_FRONT_LENGTH)
			{
				Vector3 newDir = MathUtility.getReflection(moveDir, dirRet.normal);
				float reflectAngle = MathUtility.getAngleBetweenVector(newDir, dirRet.normal) * Mathf.Rad2Deg;
				// 反射角需要限定到一定范围
				if (reflectAngle < GameDefine.MIN_REFLECT_ANGLE || reflectAngle > GameDefine.MAX_REFLECT_ANGLE)
				{
					MathUtility.clamp(ref reflectAngle, GameDefine.MIN_REFLECT_ANGLE, GameDefine.MAX_REFLECT_ANGLE);
					// 如果法线与反射方向相同,也就是垂直撞墙,则反射角默认为正
					// 否则通过叉乘的结果来判断反射角的符号
					if (!MathUtility.isVectorZero(MathUtility.normalize(newDir) - MathUtility.normalize(dirRet.normal)))
					{
						Vector3 cross = MathUtility.normalize(Vector3.Cross(newDir, dirRet.normal));
						if (cross.y > 0.0f)
						{
							reflectAngle = -reflectAngle;
						}
					}
					Quaternion quat = new Quaternion();
					quat.eulerAngles = new Vector3(0.0f, reflectAngle, 0.0f);
					newDir = MathUtility.rotateVector3(dirRet.normal, quat);
				}
				mData.mSpeedRotation.y = MathUtility.getVectorYaw(newDir) * Mathf.Rad2Deg;
				// 此处仍然同步设置角色的旋转,暂时保证速度方向与角色的旋转值一致
				mCharacter.setYaw(mData.mSpeedRotation.y);
				// 根据碰撞的角度,计算出速度损失量
				CommandCharacterHitWall cmdHitWall = newCmd(out cmdHitWall);
				cmdHitWall.mAngle = reflectAngle;
				pushCommand(cmdHitWall, mCharacter);
			}
		}

		// 调整自身位置
		correctTransform(elapsedTime);
		base.update(elapsedTime);
	}
	// 调整当前位置,自由落体时着地判断
	public void correctTransform(float elapsedTime = 0.0f)
	{
		// 从两个车轮中心向下进行射线检测,判断与地面的交点
		Ray frontRay = new Ray(mWheelCenter[0].position + Vector3.up, Vector3.down);
		Ray backRay = new Ray(mWheelCenter[1].position + Vector3.up, Vector3.down);
		RaycastHit frontHit;
		RaycastHit backHit;
#if UNITY_EDITOR
		Debug.DrawLine(frontRay.origin, frontRay.origin + frontRay.direction * 10.0f, Color.red);
		Debug.DrawLine(backRay.origin, backRay.origin + backRay.direction * 10.0f, Color.red);
#endif
		bool frontRet = Physics.Raycast(frontRay, out frontHit, 1000.0f, GameUtility.mGroundLayer);
		bool backRet = Physics.Raycast(backRay, out backHit, 1000.0f, GameUtility.mGroundLayer);
		if (!frontRet)
		{
			UnityUtility.logError("front wheel is not on ground, pos : " + StringUtility.vector3ToString(mCharacter.getPosition()));
			return;
		}
		if (!backRet)
		{
			UnityUtility.logError("back wheel is not on ground, pos : " + StringUtility.vector3ToString(mCharacter.getPosition()));
			return;
		}

		// 跳跃状态下
		if (mCharacter.hasState(PLAYER_STATE.PS_JUMP))
		{
			mData.mVerticalSpeed -= GameDefine.GRAVITY * elapsedTime;
			// 下落过程中,根据速度计算车身的俯仰角偏移
			if (mData.mVerticalSpeed <= 0.0f)
			{
				float pitch = mData.mVerticalSpeed / 5.0f * GameDefine.FALLING_PITCH;
				MathUtility.clamp(ref pitch, 0.0f, GameDefine.FALLING_PITCH);
				mCharacter.setPitch(pitch * Mathf.Rad2Deg);

				bool land = false;
				// 如果有车轮已经接触到地面了,则认为落到了地面上
				if (frontHit.point.y >= mWheelCenter[0].position.y + GameDefine.WHEEL_RADIUS
					|| backHit.point.y >= mWheelCenter[1].position.y + GameDefine.WHEEL_RADIUS)
				{
					land = true;
				}
				// 如果有车轮到地面的距离小于车轮半径加一个误差值,则认为是落到了地面上
				else
				{
					float minDis = Mathf.Min(MathUtility.getLength(mWheelCenter[0].position - frontHit.point),
											MathUtility.getLength(mWheelCenter[1].position - backHit.point));
					if (minDis < GameDefine.WHEEL_RADIUS + GameDefine.LAND_OFFSET)
					{
						land = true;
					}
				}
				if (land)
				{
					CommandCharacterLandGround cmdLand = newCmd(out cmdLand);
					cmdLand.mVerticalSpeed = mData.mVerticalSpeed;
					pushCommand(cmdLand, mCharacter);
					// 落地后将竖直方向上的速度设置为0
					mData.mVerticalSpeed = 0.0f;
				}
			}
		}
		// 在地面上
		else
		{
			// 根据与地面的交点计算角色中心位置
			Vector3 center = MathUtility.lerp(frontHit.point, backHit.point, GameDefine.BIKE_CENTER_PERCENT);
			mCharacter.setPositionY(center.y);
			// 根据地面交点计算角色俯仰角
			mCharacter.setPitch(MathUtility.getVectorPitch(frontHit.point - backHit.point) * Mathf.Rad2Deg);
		}
	}
	//------------------------------------------------------------------------------------------------------------------------------
	protected override void setBaseType() { mBaseType = typeof(CharacterBikePhysics); }
	protected override bool isType(Type type) { return type == typeof(CharacterBikePhysics); }
}

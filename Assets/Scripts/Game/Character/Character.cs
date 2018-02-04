using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Character : MovableObject
{
	protected CHARACTER_TYPE	mCharacterType;// 角色类型
	protected CharacterData		mCharacterData; //玩家数据
	protected CharacterComponentModel mAvatar;
	protected Rigidbody mRigidBody;
	public Character(CHARACTER_TYPE type, string name)
		:
		base(name)
	{
		mCharacterType = type;
		mCharacterData = new CharacterData();
	}
	public override void init()
	{
		base.init();
		mCharacterData.mName = mName;
	}
	public void setID(int id)
	{
		mCharacterData.mGUID = id;
	}
	public override void initComponents()
	{
		base.initComponents();
		mAvatar = addComponent<CharacterComponentModel>("Model", true);
	}
	public override void update(float elapsedTime)
	{
		// 先更新自己的所有组件
		base.update(elapsedTime);
	}
	public override void fixedUpdate(float elapsedTime)
	{
		base.fixedUpdate(elapsedTime);
	}
	public virtual void initModel(string modelPath, string animationControllerPath = "")
	{
		if (modelPath != "")
		{
			// 模型节点也就是角色节点,并且将节点挂到角色管理器下
			GameObject model = mModelManager.createModel(modelPath, mName);
			setObject(model, true);
			// 将外部节点设置为角色节点后,角色在销毁时就不能自动销毁节点,否则会出错
			mDestroyObject = false;
			setParent(mCharacterManager.getManagerNode());
			mAvatar.setModel(model, modelPath, true);
			mRigidBody = model.GetComponent<Rigidbody>();
		}
		if (animationControllerPath != "")
		{
			mAvatar.mAnimator.runtimeAnimatorController = mResourceManager.loadResource<RuntimeAnimatorController>(animationControllerPath, true);
		}
	}
	public virtual void notifyComponentChanged(GameComponent component) {}
	public CharacterData getCharacterData() { return mCharacterData; }
	public CHARACTER_TYPE getType() { return mCharacterType; }
	public virtual bool isType(CHARACTER_TYPE type) { return false; }
	public CharacterComponentModel getAvatar() { return mAvatar; }
	public Animation getAnimation() { return mAvatar.mAnimation; }
	public Rigidbody getRigidBody() { return mRigidBody; }
}

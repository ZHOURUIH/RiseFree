using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using UnityEditor;

public class AnchorMenu
{
	public const string mAutoAnchorMenuName = "Auto Anchor";
	[MenuItem(mAutoAnchorMenuName + "/Advance Anchor")]
	public static void advanceAnchor()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// ��ѡ������������ͬһ�����ڵ���
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for (int i = 1; i < count; ++i)
		{
			if (parent != Selection.transforms[i].parent)
			{
				UnityUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 1; i < count; ++i)
		{
			addAdvanceAnchor(Selection.gameObjects[i]);
		}
	}
	[MenuItem(mAutoAnchorMenuName + "/Scale Anchor")]
	public static void scaleAnchor()
	{
		if (Selection.gameObjects.Length <= 0)
		{
			return;
		}
		// ��ѡ������������ͬһ�����ڵ���
		Transform parent = Selection.transforms[0].parent;
		int count = Selection.gameObjects.Length;
		for(int i = 1; i < count; ++i)
		{
			if(parent != Selection.transforms[i].parent)
			{
				UnityUtility.logError("objects must have the same parent!");
				return;
			}
		}
		for (int i = 1; i < count; ++i)
		{
			addScaleAnchor(Selection.gameObjects[i]);
		}
	}
	[MenuItem(mAutoAnchorMenuName + "/ScaleAnchor/AutoRelativePos")]
	static void Calculation()
	{
		if (Selection.activeGameObject == null)
		{
			return;
		}
		ScaleAnchor anchor = Selection.activeGameObject.GetComponent<ScaleAnchor>();
		if(anchor == null)
		{
			return;
		}
		var obj = Selection.activeGameObject;
		Vector2 thisPos = obj.transform.localPosition;
		// ��ȡ�����������
		UIRect parentRect = CustomAnchor.findParentRect(obj);
		Vector2 parentSize = CustomAnchor.getRectSize(parentRect);
		// ����
		anchor.mHorizontalRelativePos = thisPos.x / parentSize.x * 2;
		anchor.mVerticalRelativePos = thisPos.y / parentSize.y * 2;
		// ���ó��Զ��巽ʽ
		anchor.mPadding = PADDING_STYLE.PS_CUSTOM_VALUE;
	}
	// ��� PADDING_STYLE ����ֵ
	[MenuItem(mAutoAnchorMenuName + "/ScaleAnchor/DefaultRelativePos")]
	static void ClaerCalculation()
	{
		if(Selection.activeGameObject == null)
		{
			return;
		}
		ScaleAnchor Anchor = Selection.activeGameObject.GetComponent<ScaleAnchor>();
		Anchor.mHorizontalRelativePos = 0.0f;
		Anchor.mVerticalRelativePos = 0.0f;
		Anchor.mPadding = PADDING_STYLE.PS_NONE;
	}
	//-------------------------------------------------------------------------------------------------------------------
	public static void addAdvanceAnchor(GameObject obj)
	{
		// �������Լ���Anchor
		UIWidget widget = CustomAnchor.getGameObjectWidget(obj);
		if(widget != null)
		{
			CustomAnchor anchor = obj.AddComponent<CustomAnchor>();
			anchor._mAnchorMode = ANCHOR_MODE.AM_NEAR_PARENT_SIDE;
		}
		// �������ӽڵ��Anchor
		int childCount = obj.transform.childCount;
		for(int i = 0; i < childCount; ++i)
		{
			addAdvanceAnchor(obj.transform.GetChild(i).gameObject);
		}
	}
	public static void addScaleAnchor(GameObject obj)
	{
		// �������Լ���Anchor
		if (obj.GetComponent<ScaleAnchor>() == null)
		{
			obj.AddComponent<ScaleAnchor>();
		}
		// �������ӽڵ��Anchor
		int childCount = obj.transform.childCount;
		for (int i = 0; i < childCount; ++i)
		{
			addScaleAnchor(obj.transform.GetChild(i).gameObject);
		}
	}
}
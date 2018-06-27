using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ReloadTexture
{
	[MenuItem("Reload/Texture")]
	static public void reloadDitherTexture()
	{
		Dither.generateDitherList();
		List<string> reloadList = Dither.getDitherList();
		string prePath = CommonDefine.A_RESOURCE_PATH + "Texture/TextureAnim/";
		int pathCount = reloadList.Count;
		for(int i = 0; i < pathCount; ++i)
		{
			reloadPath(prePath + reloadList[i] + "/");
		}
		Dither.clearDitherList();
	}
	//----------------------------------------------------------------------------------------------------------------------------
	static protected void reloadPath(string path)
	{
		List<string> files = new List<string>();
		FileUtility.findFiles(path, ref files, ".png");
		int fileCount = files.Count;
		for(int i = 0; i < fileCount; ++i)
		{
			reloadTexture(CommonDefine.P_ASSETS_PATH + files[i]);
		}
	}
	static protected void reloadTexture(string name)
	{
		// ���µ���ͼƬ,����DitherTexture�еĴ�����
		AssetDatabase.ImportAsset(name);
	}
}

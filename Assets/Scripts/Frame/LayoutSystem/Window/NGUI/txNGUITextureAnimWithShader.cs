﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class txNGUITextureAnimCriticalMask : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderCriticalMask>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimCriticalMaskFadeOutLinearDodge : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderCriticalMaskFadeOutLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimFeather : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderFeather>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimGrey : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderGrey>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimLumOffset : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderLumOffset>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimLumOffsetLinearDodge : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderLumOffsetLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimLinearDodge : txNGUIStaticTexture
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimHSLOffset : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderHSLOffset>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimHSLOffsetLinearDodge : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderHSLOffsetLinearDodge>();
	}
}
//---------------------------------------------------------------------------------------------------------------------------
public class txNGUITextureAnimMaskCut : txNGUITextureAnim
{
	public override void init(GameLayout layout, GameObject go, txUIObject parent)
	{
		base.init(layout, go, parent);
		setWindowShader<WindowShaderMaskCut>();
	}
}
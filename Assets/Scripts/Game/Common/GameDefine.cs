using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// 游戏枚举定义-----------------------------------------------------------------------------------------------
// 界面布局定义
public enum LAYOUT_TYPE
{
	LT_LOGO,
	LT_START_VIDEO,
	LT_STAND_BY,
	LT_SELECT_ROLE,
	LT_VOLUME_SETTING,
	LT_BUTTOM_PROMPT,
	LT_LOADING,
	LT_RETURN,
	LT_SELECT_TRACK,
	LT_CONFIRM_SELECTION,
	LT_TOP_TIME,
	LT_TRACK,
	LT_PROPS,
	LT_AIMING,
	LT_COUNT_DOWN,
	LT_SETTLEMENT,
	LT_CIRCLE_TIP,
	LT_END_COUNT_DOWN,
	LT_DIRECTION_TIPS,
	LT_PLAYER_RACE_INFO,
	LT_ATTACK_TIP,
	LT_DEBUG_INFO,
	LT_GLOBAL_AUDIO,
	LT_MAX,
}
// 所有的音效定义
public enum SOUND_DEFINE
{
	SD_LOGO_VIDEO,
	SD_CIRCLE_TIP,
	SD_CLICK_BUTTON,
	SD_FINISH,
	SD_FINISH_COUNT_DOWN,
	SD_FIRE_MISSILE,
	SD_GET_ITEM,
	SD_HIT_WALL,
	SD_LANDMINE_EXPLODE,
	SD_MENU_BACKGROUND,
	SD_MISSILE_HIT,
	SD_PUT_LANDMINE,
	SD_RACE_BACKGROUND0,
	SD_RACE_BACKGROUND1,
	SD_RACE_BACKGROUND2,
	SD_RACE_BACKGROUND3,
	SD_SELECTION_CHANGE,
	SD_SHIELD_OPEN,
	SD_SPRINT,
	SD_TIP_SHOW,
	SD_UNFINISH,
	SD_WRONG_DIRECTION,
	SD_MAX,
};
// 数据库表格类型
public enum DATA_TYPE
{
	DT_GAME_SOUND,
	DT_MAX,
};

// 场景的类型
public enum GAME_SCENE_TYPE
{
	GST_LOGO,
	GST_START,
	GST_MAIN,
	GST_MAX,
};
// 游戏场景流程类型
public enum PROCEDURE_TYPE
{
	PT_NONE,

	PT_LOGO_MIN,
	PT_LOGO_REGISTER_CHECK,
	PT_LOGO_LOGO,
	PT_LOGO_START_VIDEO,
	PT_LOGO_EXIT,
	PT_LOGO_MAX,

	PT_START_MIN,
	PT_START_LOADING,
	PT_START_STAND_BY,
	PT_START_SELECT_ROLE,
	PT_START_SETTING,
	PT_START_SELECT_TRACK,
	PT_START_CONFIRM_SELECTION,
	PT_START_EXIT,
	PT_START_MAX,

	PT_MAIN_MIN,
	PT_MAIN_LOADING,
	PT_MAIN_READY,
	PT_MAIN_GAMING,
	PT_MAIN_GAMING_FINISH,
	PT_MAIN_FINISH,
	PT_MAIN_SETTLEMENT,
	PT_MAIN_EXIT,
	PT_MAIN_MAX,
};
// 游戏中的公共变量定义
public enum GAME_DEFINE_FLOAT
{
	GDF_NONE,
	// 应用程序配置参数
	GDF_APPLICATION_MIN,
	GDF_FULL_SCREEN,				// 是否全屏,0为窗口模式,1为全屏,2为无边框窗口
	GDF_SCREEN_WIDTH,				// 分辨率的宽
	GDF_SCREEN_HEIGHT,              // 分辨率的高
	GDF_ADAPT_SCREEN,               // 屏幕自适应的方式,0为基于锚点的自适应,可以根据不同分辨率调整布局排列,1为简单拉伸,2为多屏拼接后复制显示
	GDF_SCREEN_COUNT,               // 显示屏数量,用于多屏横向组合为高分辨率
	GDF_USE_FIXED_TIME,				// 是否将每帧的时间固定下来
	GDF_FIXED_TIME,					// 每帧的固定时间,单位秒
	GDF_APPLICATION_MAX,

	// 框架配置参数
	GDF_FRAME_MIN,
	GDF_SOCKET_PORT,                // socket端口
	GDF_BROADCAST_PORT,             // 广播端口
	GDF_LOAD_RESOURCES,             // 游戏加载资源的路径,0代表在Resources中读取,1代表从AssetBundle中读取
	GDF_LOG_LEVEL,                  // 日志输出等级
	GDF_ENABLE_KEYBOARD,            // 是否响应键盘按键
	GDF_FRAME_MAX,

	// 游戏配置参数
	GDF_GAME_MIN,
	GDF_VOLUME,                     // 游戏整体音量
	GDF_KEYBOARD_ENABLE,			// 是否在已连接设备时也可以使用键盘操作
	GDF_TURN_THRESHOLD,				// 转向阈值,当转向角度大于该值时认为左转或者右转一次
	GDF_TURN_ANGLE_OFFSET,			// 转向角度的校正值,在使用时会将硬件角度减去该值作为最后的转向角度
	GDF_TURN_SENSITIVE,				// 骑行过程中转向的灵敏度
	GDF_READ_RPM,					// 是否读取转速作为速度值,不为0表示读取转速,为0表示读取功率
	GDF_NORMAL_FRICTION,			// 平地的正常阻力
	GDF_MIN_UPHILL_ANGLE,           // 最小上坡角度,符号为负表示向上
	GDF_MAX_UPHILL_ANGLE,           // 最大上坡角度,符号为负表示向上
	GDF_MIN_DOWNHILL_ANGLE,         // 最小下坡角度,符号为正表示向下
	GDF_MAX_DOWNHILL_ANGLE,         // 最大下坡角度,符号为正表示向下
	GDF_MIN_UPHILL_FRICTION,        // 最小上坡角度对应的阻力值
	GDF_MAX_UPHILL_FRICTION,        // 最大上坡角度对应的阻力值
	GDF_MIN_DOWNHILL_FRICTION,      // 最小下坡角度对应的阻力值
	GDF_MAX_DOWNHILL_FRICTION,      // 最大下坡角度对应的阻力值
	GDF_AI_BASE_SPEED,				// AI的基础速度
	GDF_GAME_MAX,
};
public enum GAME_DEFINE_STRING
{
	GDS_NONE,
	// 应用程序配置参数
	GDS_APPLICATION_MIN,
	GDS_APPLICATION_MAX,
	// 框架配置参数
	GDS_FRAME_MIN,
	GDS_FRAME_MAX,
	// 游戏配置参数
	GDS_GAME_MIN,
	GDS_REGISTE_CODE,					// 程序注册码
	GDS_GAME_MAX,
};
// 作为客户端时接收以及发送的类型
public enum SOCKET_PACKET
{
	SP_SPEED_DATA_RET,
	SP_FRICTION,
	SP_FRICTION_RET,
	SP_MAX,
};
// 串口消息包类型
public enum COM_PACKET
{
	CP_FIT_DATA,
	CP_FRICTION,
	CP_MAX,
}
public enum STATE_GROUP
{
	SG_SELECT,
	SG_GAME,
	SG_RIDE,
	SG_BUFF,
}
// 角色状态类型
public enum PLAYER_STATE
{
	PS_AIM,
	PS_ATTACKED,
	PS_FINISH,
	PS_JUMP,
	PS_GAMING,
	PS_RIDING,
	PS_PROTECTED,
	PS_READY,
	PS_SPRINT,
	PS_IDLE,
	PS_ON_SELECT_ROLE,
	PS_SELECTED_ROLE,
	PS_UN_SELECT_ROLE,
	PT_WRONG_DIRECTION,
	PS_MAX,
}
// 场景中道具类型
public enum SCENE_ITEM
{
	SI_ITEM_BOX,
	SI_MISSILE,
	SI_LAND_MINE,
	SI_MAX,
}
// 角色道具类型
public enum PLAYER_ITEM
{
	PI_MISSILE,
	PI_SHIELD,
	PI_TURBO,
	PI_LAND_MINE,
	PI_MAX,
}

// 数据解析结果
public enum PARSE_RESULT
{
	PR_SUCCESS,     // 解析成功
	PR_ERROR,       // 内容错误
	PR_NOT_ENOUGH,  // 数据不足
};
public enum DEVICE_CONNENT
{
	DC_NONE,
	DC_PROCEED,
	DC_SUCCESS,
	DC_CLOSE,
	DC_MAX,
}

// 游戏常量定义-------------------------------------------------------------------------------------------------------------
public class GameDefine : CommonDefine
{
	// 路径定义
	//-----------------------------------------------------------------------------------------------------------------
	public const string CHARACTER = "Character";
	public const string SCENE_ITEM = "SceneItem";
	public const string PREFAB = "Prefab";
	public const string ITEM_BOX = "ItemBox";
	public const string LANDMINE = "Landmine";
	public const string MISSILE = "Missile";
	public const string SHIELD = "Shield";
	public const string TURBO = "Turbo";
	public const string R_SCENE_ITEM_PATH = R_MODEL_PATH + SCENE_ITEM + "/";
	public const string R_SCENE_ITEM_PREFAB_PATH = R_SCENE_ITEM_PATH + PREFAB + "/";
	public const string R_MODEL_CHARACTER_PATH = R_MODEL_PATH + CHARACTER + "/";
	public const string R_CHARACTER_PREFAB_PATH = R_MODEL_CHARACTER_PATH + PREFAB + "/";
	public const string R_PARTICLE_PREFAB_PATH = R_PARTICLE_PATH + PREFAB + "/";
	// 常量定义
	//-----------------------------------------------------------------------------------------------------------------
	public const string COMPANY_NAME = "北京踏行天际科技发展有限公司";
	// 当前游戏名
	public const string GAME_NAME = "电能动力4.0";
	// 注册密钥
	public const string REGISTE_KEY = "电能动力4.3";
	// 场景名
	public const string ROLE_DISPLAY = "RoleDisplay";
	public const string SNOW_MOUNTAIN = "SnowMountain";
	public const string PRIMARY_TRACK = "PrimaryTrack";
	public const string DESERT = "Desert";
	public const string ANCIENT_CITY = "AncientCity";
	// 层名
	public const string LAYER_GROUND = "Ground";
	public const string LAYER_WALL = "Wall";
	public const string LAYER_JUMP_POINT = "JumpPoint";
	public const string LAYER_CHARACTER = "Character";
	// 角色动作定义
	public const string ANIM_STANDING = "Standing";
	public const string ANIM_STARTING = "Starting";
	public const string ANIM_RIDE = "Ride";
	public const string ANIM_TURN_RIGHT = "TurnRight";
	public const string ANIM_TURN_RIGHT_SHARP = "TurnRightSharp";
	public const string ANIM_TURN_LEFT = "TurnLeft";
	public const string ANIM_TURN_LEFT_SHARP = "TurnLeftSharp";
	public const string ANIM_PRE_JUMP = "PreJump";
	public const string ANIM_START_JUMP = "StartJump";
	public const string ANIM_JUMP_UP = "JumpUp";
	public const string ANIM_JUMP_LOOP = "JumpLoop";
	public const string ANIM_JUMP_DOWN = "JumpDown";    // 此动作暂时不播放
	public const string ANIM_LANDING = "Landing";
	public const string ANIM_FORCE_LANDING = "ForceLanding";
	public const string ANIM_SPEED_UP = "SpeedUp";
	public const string ANIM_FALL_DOWN = "FallDown";
	public const string ANIM_SPEED_UP_SHARP = "SpeedUpSharp";
	public const string ANIM_SHAKE_BIKE = "ShakeBike";
	public const string QUEUE_SUFFIX = " - Queued Clone";
	// 角色模型的数量
	public const int ROLE_COUNT = 4;
	// 赛道的数量
	public const int TRACK_COUNT = 4;
	// AI的最大数量
	public const int MAX_AI_COUNT = 4;
	// 最大的赛道圈数
	public const int MAX_CIRCLE_COUNT = 4;
	// 角色背包中物品的最大数量
	public const int PACK_ITEM_COUNT = 3;
	// 导弹的飞行速度
	public const float MISSILE_SPEED = 40.0f;
	// 强着地的速度
	public const float FORCE_LAND_SPEED = 10.0f;
	// 判断角色是否加速的差值
	public const float SPEED_UP_DELTA = 0.5f;
	// 判断角色是否急加速的差值		
	public const float SPEED_UP_FAST_DELTA = 1.0f;
	// 摇车速度
	public const float SHAKE_BIKE_SPEED = 8.0f;
	// 达到转弯所需要的转向角
	public const float TURN_ANGLE = 15.0f;
	// 达到急转弯所需要的转向角
	public const float TURN_SHARP_ANGLE = 45.0f;
	// 起跳时的竖直方向上的初速度
	public const float JUMP_SPEED = 5.0f;
	// 重力加速度
	public const float GRAVITY = 9.8f;
	// 下落过程中车身俯仰角的最大值
	public const float FALLING_PITCH = 15.0f;
	// 单车最前端到模型中心的长度,由模型决定,男女角色目前为一致的
	public const float BIKE_FRONT_LENGTH = 0.83f;
	// 单车的车轮半径
	public const float WHEEL_RADIUS = 0.33f;
	// 撞墙后反弹的最大和最小反射角度,反射角是反射方向与法线的夹角
	public const float MAX_REFLECT_ANGLE = 80.0f;
	public const float MIN_REFLECT_ANGLE = 70.0f;
	// 判断落地时的高度最大误差范围
	public const float LAND_OFFSET = 0.2f;
	// 角色中心在前车轮接触点和后接触点之间的位置
	public const float BIKE_CENTER_PERCENT = 0.47f;
	// 道具箱子刷新的时间
	public const float CREATE_ITEM_BOX_TIME = 30.0f;
	// 导弹的最大发射距离
	public const float MAX_LAUNCH_MISSILE_DISTANCE = 100.0f;
	// 反向时间,强制刷新角色的方向和位置
	public const float WRONG_DIRECTION_TIME = 4.0f;
	// 方向时间,显示方向的错误提示
	public const float WRONG_DIRECTION_TIPS_TIME = 2.0f;
	// 显示的里程或速度数值系数,实际里程或速度乘以该系数为显示的值
	public const float DISPLAY_MILEAGE_SCALE = 0.33f;
	// 瞄准准星的偏移量
	public static Vector3 AIM_OFFSET = new Vector3(0.0f, 2.0f, 0.0f);
	public static Vector3 CAMERA_RELATIVE = new Vector3(0.0f, 1.8f, -6.7f);
	public static Vector3 CAMERA_LOOKAT_OFFSET = new Vector3(0.0f, 0.7f, 0.0f);
	public static string[] ROLE_MODEL_NAME = new string[ROLE_COUNT] { "Role0", "Role1", "Role2", "Role3" };
	public static Color[] PLAYER_TRAIL_COLOR = new Color[GameDefine.ROLE_COUNT] 
	{
		new Color(0.0f, 0.7f, 1.0f),
		new Color(1.0f, 0.3f, 0.5f),
		new Color(0.0f, 1.0f, 0.0f),
		new Color(1.0f, 0.2f, 0.0f),
	};
	public const byte REPORT_OUT = 0x01;
	public const byte REPORT_IN = 0x02;
}
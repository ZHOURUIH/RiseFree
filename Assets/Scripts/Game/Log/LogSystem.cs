using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public enum LOG_TYPE
{
	LOG_RISE_FREE_PROCEDURE = 1000,     // 流程跳转      
	LOG_RISE_FREE_USER_OPERATION,       // 用户操作
	LOG_RISE_FREE_OTHER,                // 其他
	LOG_RISE_FREE_HTTP_TIME_OUT,
	LOG_RISE_FREE_GAME_ERROR,
}

public enum LOG_STATE
{
	LS_UNUPLOAD,
	LS_UPLOADING,
	LS_UPLOADED,
}

public class LogData
{
	public LOG_TYPE mType;
	public DateTime mTime;
	public string mInfo;
	public Guid mGuid;
	public LOG_STATE mState;
}

public class LogSystem : FrameComponent, IFrameLogSystem
{
	protected Dictionary<string, LogData> mLogSendList;     // 需要发送的日志列表
	protected List<LogData> mLogBufferList;   // 临时存放日志的列表
	protected CustomThread mSendThread;
	protected ThreadLock mBufferLock;
	protected ThreadLock mSqlLiteLock;
	protected ThreadLock mSendLock;
	protected SQLite mSQLite;
	protected string mTableName = "Log";
	protected string mGymID;
	public LogSystem(string name)
		: base(name)
	{
		mSQLite = new SQLite("Game.data");
		mLogSendList = new Dictionary<string, LogData>();
		mLogBufferList = new List<LogData>();
		mBufferLock = new ThreadLock();
		mSqlLiteLock = new ThreadLock();
		mSendLock = new ThreadLock();
		mSendThread = new CustomThread("SendLog");
	}
	public override void init()
	{
		try
		{
			mSQLite.init();
			mSQLite.createTable(mTableName, "GymID varchar(64), LogType varchar(64), Time varchar(32), LogInfo varchar(256), GUID varchar(64), uploaded integer");
			mGymID = mRegisterTool.generateRegisteCode(mRegisterTool.generateRequestCode(), GameDefine.REGISTE_KEY);
		}
		catch (Exception e)
		{
			UnityUtility.logError("初始化日志系统失败! " + e.Message);
		}
		mSendThread.start(sendLog, 50);
	}
	public override void destroy()
	{
		base.destroy();
		if (mSQLite != null)
		{
			mSQLite.destroy();
			mSQLite = null;
		}
		mBufferLock.unlock();
		mSqlLiteLock.unlock();
		mSendThread.destroy();
		UnityUtility.logInfo("完成退出日志");
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void logUserOperation(string info)
	{
		log(info, LOG_TYPE.LOG_RISE_FREE_USER_OPERATION);
	}
	public void logProcedure(string info)
	{
		log(info, LOG_TYPE.LOG_RISE_FREE_PROCEDURE);
	}
	public void logOther(string info)
	{
		log(info, LOG_TYPE.LOG_RISE_FREE_OTHER);
	}
	public void logHttpOverTime(string info)
	{
		log(info, LOG_TYPE.LOG_RISE_FREE_HTTP_TIME_OUT);
	}
	public void logGameError(string info)
	{
		log(info, LOG_TYPE.LOG_RISE_FREE_GAME_ERROR);
	}
	//-------------------------------------------------------------------------------------------------------------------------------------
	protected void log(string info, LOG_TYPE type)
	{
		LogData data = new LogData();
		data.mType = type;
		data.mTime = DateTime.Now;
		data.mInfo = info;
		data.mGuid = Guid.NewGuid();
		data.mState = LOG_STATE.LS_UNUPLOAD;
		mBufferLock.waitForUnlock();
		mLogBufferList.Add(data);
		mBufferLock.unlock();
	}
	protected bool sendLog()
	{
		// 将日志缓存同步到发送列表中
		mSendLock.waitForUnlock();
		mBufferLock.waitForUnlock();
		int count = mLogBufferList.Count;
		if (count > 0)
		{
			for (int i = 0; i < count; ++i)
			{
				LogData data = mLogBufferList[i];
				mLogSendList.Add(data.mGuid.ToString(), data);
			}
			mLogBufferList.Clear();
		}
		mBufferLock.unlock();

		Dictionary<string, LogData> tempList = new Dictionary<string, LogData>(mLogSendList);
		mSendLock.unlock();
		foreach (var item in tempList)
		{
			// 找到未上传的数据
			if(item.Value.mState == LOG_STATE.LS_UNUPLOAD)
			{
				LogData data = item.Value;
				// 设置为正在上传状态
				data.mState = LOG_STATE.LS_UPLOADING;
				mSqlLiteLock.waitForUnlock();
				mSQLite.insertData(mTableName, new object[] { mGymID, data.mType.ToString(), data.mTime.ToString("G"), data.mInfo, data.mGuid.ToString(), 0 });
				mSqlLiteLock.unlock();

				// 将日志上传服务器,并且记录到本地数据库
				string uploadData = "";
				prepareData(data, ref uploadData);
				HttpUtility.httpWebRequestPost("http://app1.taxingtianji.com/wechat/php/gameLog.php?", uploadData, onDataUploadResult, data.mGuid.ToString());
			}
		}
		return true;
	}
	protected void prepareData(LogData logData, ref string str)
	{
		if (logData == null)
		{
			return;
		}
		str = string.Empty;
		int type = (int)logData.mType;
		StringUtility.jsonStartStruct(ref str, 0, true);
		StringUtility.jsonAddPair(ref str, "GymID", mGameConfig.getStringParam(GAME_DEFINE_STRING.GDS_REGISTE_CODE), 1, true);
		StringUtility.jsonAddPair(ref str, "logtype", StringUtility.intToString(type), 1, true);
		StringUtility.jsonAddPair(ref str, "date", logData.mTime.ToString("G"), 1, true);
		StringUtility.jsonAddPair(ref str, "info", logData.mInfo, 1, true);
		StringUtility.jsonEndStruct(ref str, 0, true);
		StringUtility.removeLastComma(ref str);
	}
	protected void onDataUploadResult(LitJson.JsonData data, object userData)
	{
		if (data == null || data.ToJson() == "")
		{
			return;
		}
		string guid = userData as string;
		string result = data["result"].ToString();
		if (result == "fail")
		{
			mSqlLiteLock.waitForUnlock();
			mSQLite.updateData(mTableName, new string[] { "uploaded" }, new object[] { 0 }, new string[] { "guid = '" + guid + "'" });
			mSqlLiteLock.unlock();
			// 上传失败,设置为未上传状态
			mSendLock.waitForUnlock();
			mLogSendList[guid].mState = LOG_STATE.LS_UNUPLOAD;
			mSendLock.unlock();
		}
		else if (result == "success")
		{
			mSqlLiteLock.waitForUnlock();
			mSQLite.updateData(mTableName, new string[] { "uploaded" }, new object[] { 1 }, new string[] { "guid = '" + guid + "'" });
			mSqlLiteLock.unlock();
			// 上传成功,移除该条信息
			mSendLock.waitForUnlock();
			mLogSendList.Remove(guid);
			mSendLock.unlock();
		}
	}
}
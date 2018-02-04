using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public enum LOG_TYPE
{
	LOG_RISE_FREE_PROCEDURE = 1000,		// 流程跳转      
	LOG_RISE_FREE_USER_OPERATION,		// 用户操作
	LOG_RISE_FREE_OTHER,				// 其他
}

public class LogData
{
	public LOG_TYPE mType;
	public DateTime mTime;
	public string mInfo;
	public Guid mGuid;
}

public class LogSystem : FrameComponent
{
	protected List<LogData> mLogSendList;     // 需要发送的日志列表
	protected List<LogData> mLogBufferList;   // 临时存放日志的列表
	protected Thread mSendThread;
	protected ThreadLock mBufferLock;
	protected ThreadLock mSqlLiteLock;
	protected bool mRunning = false;
	protected bool mFinish = true;
	protected SQLite mSQLite;
	protected string mTableName = "Log";
	protected string mGymID;
	public LogSystem(string name)
		:base(name)
	{
		mSQLite = new SQLite("Game.data");
		mLogSendList = new List<LogData>();
		mLogBufferList = new List<LogData>();
		mBufferLock = new ThreadLock();
		mSqlLiteLock = new ThreadLock();
	}
	public override void init()
	{
		mRunning = true;
		mFinish = true;
		try
		{
			mSQLite.init();
			mSQLite.createTable(mTableName, "GymID varchar(64), LogType varchar(64), Time varchar(32), LogInfo varchar(256), GUID varchar(64), uploaded integer");
			mGymID = mRegisterTool.generateRegisteCode(mRegisterTool.generateRequestCode(), GameDefine.REGISTE_KEY);
			mSendThread = new Thread(sendLog);
			mSendThread.Start();
		}
		catch(Exception e)
		{
			UnityUtility.logError("初始化日志系统失败! " + e.Message);
			mFinish = true;
		}
	}
	public override void destroy()
	{
		base.destroy();
		if(mSQLite != null)
		{
			mSQLite.destroy();
			mSQLite = null;
		}
		mBufferLock.unlock();
		mSqlLiteLock.unlock();
		mRunning = false;
		while (!mFinish) { }
		if(mSendThread != null)
		{
			mSendThread.Abort();
			mSendThread = null;
		}
		UnityUtility.logInfo("日志系统退出完毕!");
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
	//-------------------------------------------------------------------------------------------------------------------------------------
	protected void log(string info, LOG_TYPE type)
	{
		LogData data = new LogData();
		data.mType = type;
		data.mTime = DateTime.Now;
		data.mInfo = info;
		data.mGuid = Guid.NewGuid();
		mBufferLock.waitForUnlock();
		mLogBufferList.Add(data);
		mBufferLock.unlock();
		if (type.ToString().Length >= 64 || data.mTime.ToString("G").Length >= 32 || info.Length >= 256 || data.mGuid.ToString().Length >= 64)
		{
			return;
		}
		mSqlLiteLock.waitForUnlock();
		mSQLite.insertData(mTableName, new object[] { mGymID, type.ToString(), data.mTime.ToString("G"), info, data.mGuid.ToString(), 0 });
		mSqlLiteLock.unlock();
	}
	protected void sendLog()
	{
		string uploadData = "";
		mFinish = false;
		while (mRunning)
		{
			try
			{
				// 将日志缓存同步到发送列表中
				mBufferLock.waitForUnlock();

				if (mLogBufferList.Count > 0)
				{
					for (int i = 0; i < mLogBufferList.Count; i++)
					{
						LogData data = mLogBufferList[i];
						mLogSendList.Add(data);
					}
					mLogBufferList.Clear();
				}
				mBufferLock.unlock();

				if (mLogSendList.Count > 0)
				{
					// 将日志上传服务器,并且记录到本地数据库
					prepareData(mLogSendList[0], ref uploadData);
					PluginUtility.httpWebRequestPost("http://app1.taxingtianji.com/wechat/php/gameLog.php?", uploadData, onDataUploadResult, mLogSendList[0].mGuid.ToString());
					mLogSendList.RemoveAt(0);
				}
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获日志异常 : " + e.Message + ", stack : " + e.StackTrace, LOG_LEVEL.LL_FORCE);
			}
		}
		mFinish = true;
		UnityUtility.logInfo("退出日志线程!");
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
		}
		else if (result == "success")
		{
			mSqlLiteLock.waitForUnlock();
			mSQLite.updateData(mTableName, new string[] { "uploaded" }, new object[] { 1 }, new string[] { "guid = '" + guid + "'" });
			mSqlLiteLock.unlock();
		}
	}
}
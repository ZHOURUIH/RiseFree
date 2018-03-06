using Mono.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

public enum LOG_TYPE
{
	LOG_4_5_PROCEDURE,          // 流程跳转
	LOG_4_5_USER_OPERATION,     // 用户操作
	LOG_4_5_OTHER,              // 其他
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

public class LogSystem : FrameComponent
{
	protected Dictionary<string, LogData> mLogSendList;     // 需要发送的日志列表
	protected List<LogData> mLogBufferList;   // 临时存放日志的列表
	protected Thread mSendThread;
	protected ThreadLock mBufferLock;
	protected ThreadLock mSqlLiteLock;
	protected ThreadLock mSendLock;
	protected bool mRunning = false;
	protected bool mFinish = true;
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
		catch (Exception e)
		{
			UnityUtility.logError("初始化日志系统失败! " + e.Message);
			mFinish = true;
		}
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
		mRunning = false;
		while (!mFinish) { }
		if (mSendThread != null)
		{
			mSendThread.Abort();
			mSendThread = null;
		}
		UnityUtility.logInfo("完成退出日志");
	}
	public override void update(float elapsedTime)
	{
		;
	}
	public void logUserOperation(string info)
	{
		log(info, LOG_TYPE.LOG_4_5_USER_OPERATION);
	}
	public void logProcedure(string info)
	{
		log(info, LOG_TYPE.LOG_4_5_PROCEDURE);
	}
	public void logOther(string info)
	{
		log(info, LOG_TYPE.LOG_4_5_OTHER);
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
	protected void sendLog()
	{
		string uploadData = "";
		mFinish = false;
		while (mRunning)
		{
			try
			{
				// 将日志缓存同步到发送列表中
				mSendLock.waitForUnlock();
				mBufferLock.waitForUnlock();
				if (mLogBufferList.Count > 0)
				{
					for (int i = 0; i < mLogBufferList.Count; i++)
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
					// 找到第一个未上传的数据
					if(item.Value.mState == LOG_STATE.LS_UNUPLOAD)
					{
						LogData data = item.Value;
						// 设置为正在上传状态
						data.mState = LOG_STATE.LS_UPLOADING;
						if (data.mType.ToString().Length >= 64 || data.mTime.ToString("G").Length >= 32 || data.mInfo.Length >= 256 || data.mGuid.ToString().Length >= 64)
						{
							return;
						}
						mSqlLiteLock.waitForUnlock();
						mSQLite.insertData(mTableName, new object[] { mGymID, data.mType.ToString(), data.mTime.ToString("G"), data.mInfo, data.mGuid.ToString(), 0 });
						mSqlLiteLock.unlock();

						// 将日志上传服务器,并且记录到本地数据库
						prepareData(data, ref uploadData);
						PluginUtility.httpWebRequestPost("http://app1.taxingtianji.com/wechat/php/gameLog.php?", uploadData, onDataUploadResult, data.mGuid.ToString());
					}
				}
			}
			catch (Exception e)
			{
				UnityUtility.logInfo("捕获日志异常!" + e.Message + ", " + e.StackTrace);
			}
		}
		mFinish = true;
		UnityUtility.logInfo("日志线程退出完成!");
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
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

// 내부적으로 쓰레드를 돌며 로그 정보를 관리한다.
// 네트워크 연결된 상태이면 축적된 로그(파일로그 포함)를 모두 DB서버로 보낸다.
// 네트워크 연결이 끊긴 상태이면 로컬 파일로 저장한다.
public class LogManager
{
    public Func<bool> IsNetworkAlive;
    public Action<string> LogWriterConsole;
    public Func<string, bool> LogStringWriterDB;
    public Func<byte[], bool> LogBytesWriterDB;

    private Thread mThread = null;
    private bool mRunFlag = false;
    private string mFileLogPath = null;
    private ConcurrentQueue<string> mQueue = new ConcurrentQueue<string>();

    public void Initialize(string logPath)
    {
        if (mThread != null)
            return;

        if (IsNetworkAlive == null)
            IsNetworkAlive = () => { return false; };

        if (LogStringWriterDB == null)
            LogStringWriterDB = (data) => { return false; };

        if (LogBytesWriterDB == null)
            LogBytesWriterDB = (data) => { return false; };

        mFileLogPath = logPath + "/Log/";
        DirectoryInfo di = new DirectoryInfo(mFileLogPath);
        if (di.Exists == false)
            di.Create();

        //mRunFlag = true;
        //mThread = new Thread(new ThreadStart(Run));
        //mThread.Start();
    }
    public void UnInitialize()
    {
        string[] logs = FlushQueue();
        if (logs.Length > 0)
        {
            WriteLogsToFile(logs);
        }
    }
    public void AddLog(string msg)
    {
        mQueue.Enqueue(msg);
    }
    void ProcessToFlushLog()
    {
        if (IsNetworkAlive())
            WriteFilesToDB();

        string[] logs = FlushQueue();
        if (logs.Length <= 0)
            return;

        if (IsNetworkAlive())
        {
            if (!WriteLogsToDB(logs))
                WriteLogsToFile(logs);
        }
        else
        {
            WriteLogsToFile(logs);
        }
    }


    void Run()
    {
        while (mRunFlag)
        {
            Thread.Sleep(1000);

            ProcessToFlushLog();
        }
    }
    void WriteLogsToFile(string[] logs)
    {
        try
        {
            string filename = DateTime.Now.ToString("yyMMdd") + ".txt";
            string path = mFileLogPath + filename;
            using (var stream = new FileStream(path, FileMode.Append))
            {
                byte[] data = LogStringToByte(logs);
                stream.Write(data, 0, data.Length);
            }
        }
        catch
        {
            LogWriterConsole?.Invoke("Failed WriteLogsToFile");
        }
    }
    bool WriteLogsToDB(string[] logs)
    {
        foreach (string log in logs)
        {
            if (!IsNetworkAlive())
                return false;

            if (!LogStringWriterDB.Invoke(log))
                return false;
        }
        return true;
    }
    void WriteFilesToDB()
    {
        string[] filePaths = Directory.GetFiles(mFileLogPath);
        foreach (string file in filePaths)
        {
            if (!IsNetworkAlive())
                break;

            byte[] filedata = File.ReadAllBytes(file);
            if (LogBytesWriterDB.Invoke(filedata))
                File.Delete(file);
        }
    }
    string[] FlushQueue()
    {
        List<string> logs = new List<string>();
        string msg = "";
        while (mQueue.TryDequeue(out msg))
            logs.Add(msg);

        return logs.ToArray();
    }
    byte[] LogStringToByte(string[] logs)
    {
        string log = String.Join<string>("\r\n", logs) + "\r\n";
        return Encoding.UTF8.GetBytes(log);
    }
    string[] LogFileToString(byte[] bytes)
    {
        string log = Encoding.UTF8.GetString(bytes);
        return log.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
    }

}

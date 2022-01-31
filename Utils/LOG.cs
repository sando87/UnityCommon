using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class LOG
{
    static private readonly char PathSpliter = Path.DirectorySeparatorChar;
    static public Action<string> EventLogSimple { get; set; } = DefaultPrinter;
    static public Action<LogDetail> EventLogDetail { get; set; } = null;
    static private void DefaultPrinter(string message)
    {
        UnityEngine.Debug.Log(message);
    }
    //특정 코드 영역이 타는지 간단히 확인하고 싶을 때 사용(파일이름,함수이름,라인번호 정보를 콘솔창에 남긴다.)
    //How to Use : LOG.trace();
    static public void trace(string val = "",
        [CallerFilePath] string file = null,
        [CallerMemberName] string caller = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (EventLogDetail != null)
        {
            LogDetail log = new LogDetail();
            log.logLevel = "trace";
            log.fileName = ParseFilename(file);
            log.funcName = caller;
            log.lineNumber = lineNumber.ToString();
            log.message = val;
            EventLogDetail?.Invoke(log);
        }
        string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val + "]";
        EventLogSimple?.Invoke(message);
    }
    //기본 trace에 추가로 사용자가 남기고 싶은 정보도 함께 콘솔창에 남긴다.
    //How to Use : LOG.trace("user custom message"); or LOG.trace(123);
    static public void trace<T>(T val,
        [CallerFilePath] string file = null,
        [CallerMemberName] string caller = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (EventLogDetail != null)
        {
            LogDetail log = new LogDetail();
            log.logLevel = "trace";
            log.fileName = ParseFilename(file);
            log.funcName = caller;
            log.lineNumber = lineNumber.ToString();
            log.message = val.ToString();
            EventLogDetail?.Invoke(log);
        }
        string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val.ToString() + "]";
        EventLogSimple?.Invoke(message);
    }
    //에러 수준 로그를 띄어주고 반드시 수정해야 해야 할때 사용
    static public void warn(string val = "",
        [CallerFilePath] string file = null,
        [CallerMemberName] string caller = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (EventLogDetail != null)
        {
            LogDetail log = new LogDetail();
            log.logLevel = "warn";
            log.fileName = ParseFilename(file);
            log.funcName = caller;
            log.lineNumber = lineNumber.ToString();
            log.message = val;
            EventLogDetail?.Invoke(log);
        }
        string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val + "]";
        EventLogSimple?.Invoke(message);
    }
    //개발자를 위한 디버깅용 로그에 사용(조건이 맞으면 로그 출력) - 출시상태에서는 삭제 필요
    static public void warn(bool isError, string val = "",
        [CallerFilePath] string file = null,
        [CallerMemberName] string caller = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        if (!isError) return;
        if (EventLogDetail != null)
        {
            LogDetail log = new LogDetail();
            log.logLevel = "warn";
            log.fileName = ParseFilename(file);
            log.funcName = caller;
            log.lineNumber = lineNumber.ToString();
            log.message = val;
            EventLogDetail?.Invoke(log);
        }
        string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val + "]";
        EventLogSimple?.Invoke(message);
    }
    //호출 스택을 출력해줌
    static public void callStack(string val = "",
        [CallerFilePath] string file = null,
        [CallerMemberName] string caller = null,
        [CallerLineNumber] int lineNumber = 0)
    {
        LogDetail log = new LogDetail();
        string message = "## [" + ParseFilename(file) + "] [" + caller + "] [" + lineNumber + "] [" + val + "] [" + log.stackTrace + "]";
        EventLogSimple?.Invoke(message);
    }
    static public string ParseFilename(string fullPath)
    {
        int idx = fullPath.LastIndexOf(PathSpliter);
        if (0 <= idx && idx < fullPath.Length)
        {
            return fullPath.Substring(idx);
        }
        return fullPath;
    }
}

public class LogDetail
{
    public string time; //로그 남기는 당시 시간
    public string threadID; //쓰레드 ID
    public string logLevel; //로그레벨
    public string fileName; //호출당시 파일이름
    public string funcName; //호출당시 함수이름
    public string lineNumber; //호출당시 코드 라인 번호
    public string message;  //사용자 로그 메시지
    public string stackTrace; //호출스택
    public override string ToString()
    {
        return time + "," + threadID + "," + logLevel + "," + fileName + "," + funcName + "," + lineNumber + "," + message + "," + stackTrace;
    }
    public LogDetail()
    {
        time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        threadID = Thread.CurrentThread.ManagedThreadId.ToString();
        stackTrace = "";
        StackFrame[] frames = new StackTrace().GetFrames();
        int cnt = Mathf.Min(frames.Length, 7);
        for (int i = 0; i < cnt; ++i)
        {
            stackTrace += (frames[i].GetMethod().Name + ">");
        }
    }
}

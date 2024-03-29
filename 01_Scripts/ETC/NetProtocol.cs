﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public enum NetCMD
{
    Undef, AddUser, UpdateUser, EditName, DelUser, AddLog, AddLogFile, GetScores, 
    SearchOpponent, StopMatching, PVP, EndPVP, HeartCheck
}
public enum PVPCommand
{
    Undef, StartGame, Click, Swipe, Destroy, Create, FlushAttacks, EndGame, DropPause, DropResume, ChangeSkill, BreakIce, CloseProducts, UseItem, SyncTimer,
    SkillBomb, SkillIce, SkillIceRes, SkillShield, SkillScoreBuff, SkillChangeProducts, SkillCloud, SkillUpsideDown, SkillRemoveBadEffects
}
public enum ProductColor
{
    None, Blue, Green, Orange, Purple, Red, Yellow
};
public enum SwipeDirection
{
    UP, DOWN, LEFT, RIGHT
};
public enum ProductSkill
{
    Nothing, Horizontal, Vertical, Bomb, SameColor, Hammer
};
public enum PurchaseItemType
{
    None, ExtendLimit, RemoveIce, MakeSkill1, MakeCombo, MakeSkill2, Meteor
}

public interface ByteSerializer
{
    byte[] Serialize();
    void Deserialize(byte[] bytes);
}

public class NetProtocol
{
    public const int ClientSessionKeepTime = 3;
    public const int HeartCheckInterval = 30;
    public const int DeadSessionMaxTime = HeartCheckInterval * 5;
    public const int ServerMatchingInterval = 1;
    public const int ServerMonitoringInterval = 300;

    public const UInt32 MAGIC = 0x12345678;
    public const int recvBufSize = 1024 * 64;
    static public int HeadSize()
    {
        return Marshal.SizeOf(typeof(Header));
    }
    static public bool IsValid(byte[] msg, int offset = 0)
    {
        if (offset + 4 > msg.Length)
            return false;
        return NetProtocol.MAGIC == BitConverter.ToInt32(msg, offset);
    }
    static public int Length(byte[] msg, int offset = 0)
    {
        if (offset + 16 + 4 > msg.Length)
            return -1;
        return BitConverter.ToInt32(msg, offset + 16);
    }
    static public byte[] ToArray(Header msg, byte[] body)
    {
        List<byte> buf = new List<byte>();
        if (body == null)
        {
            msg.Length = HeadSize();
            buf.AddRange(MyUtils.Serialize(msg));
        }
        else
        {
            msg.Length = HeadSize() + body.Length;
            buf.AddRange(MyUtils.Serialize(msg));
            buf.AddRange(body);
        }

        return buf.ToArray();
    }
    static public Header ToMessage(byte[] buf, out byte[] body)
    {
        int headSize = HeadSize();
        Header msg = MyUtils.Deserialize<Header>(ref buf);
        int bodyLen = buf.Length - headSize;
        if (bodyLen > 0)
        {
            body = new byte[bodyLen];
            Array.Copy(buf, headSize, body, 0, bodyLen);
        }
        else
            body = null;

        return msg;
    }
    static public List<byte[]> SplitBuffer(byte[] buffer)
    {
        List<byte[]> messages = new List<byte[]>();

        int headerSize = HeadSize();
        if (buffer.Length < headerSize)
            return messages;

        int offset = 0;
        while (true)
        {
            if (!IsValid(buffer, offset))
                break;

            int len = Length(buffer, offset);
            if (len < 0 || offset + len > buffer.Length)
                break;

            byte[] msg = new byte[len];
            Array.Copy(buffer, offset, msg, 0, len);
            messages.Add(msg);
            offset += len;
        }

        return messages;
    }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class Header
{
    public UInt32 Magic = NetProtocol.MAGIC;
    public NetCMD Cmd = NetCMD.Undef;
    public Int64 RequestID = -1;
    public int Length = 0;
    public int Ack = 0;
    public int UserPk = -1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
    public string message = "";
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class UserInfo
{
    public int userPk = -1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string userName = "";
    public int score = 0;
    public int win = 0;
    public int lose = 0;
    public int total = 0;
    public float rankingRate = 1;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
    public string deviceName = "";
    public bool IsBot { get { return deviceName.Contains("bot"); } }
    public int NetworkLatency { get; set; } = -1;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public PurchaseItemType[] PvpItems = new PurchaseItemType[3];
    public MatchingLevel maxLeague = MatchingLevel.Bronze;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public struct ProductInfo
{
    public ProductColor prvColor;
    public ProductColor nextColor;
    public ProductSkill skill;
    public int idxX;
    public int idxY;
    public int prvInstID;
    public int nextInstID;
    public ProductInfo(ProductColor prvColor, ProductColor nextColor, ProductSkill skill, int idxX, int idxY, int prvInstID, int nextInstID)
    { this.prvColor = prvColor; this.nextColor = nextColor; this.skill = skill; this.idxX = idxX; this.idxY = idxY; this.prvInstID = prvInstID; this.nextInstID = nextInstID; }
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class LogInfo
{
    public int userPk;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string message;
    public LogInfo(string msg) { userPk = -1; message = msg; }
    public LogInfo() { userPk = -1; message = ""; }
}

public class LogFile : ByteSerializer
{
    public int userPk;
    public byte[] data;

    public byte[] Serialize()
    {
        List<byte> rets = new List<byte>();
        rets.AddRange(BitConverter.GetBytes(userPk));
        rets.AddRange(data);
        return rets.ToArray();
    }

    public void Deserialize(byte[] bytes)
    {
        userPk = BitConverter.ToInt32(bytes, 0);
        data = new byte[bytes.Length - 4];
        Array.Copy(bytes, 4, data, 0, data.Length);
    }
}

public enum MatchingState { None, Idle, TryMatching, FoundOpp, FoundOppAck, Matched }
public enum MatchingLevel { None, Bronze, Silver, Gold, Master, All }
public enum MatchingFriend { None, Make, Join }

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class SearchOpponentInfo
{
    public MatchingState State;
    public UserInfo MyUserInfo;
    public UserInfo OppUserInfo;
    public MatchingLevel Level;
    public MatchingFriend WithFriend = MatchingFriend.None;
    public int RoomNumber = -1;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
public class EndPVP
{
    public PVPCommand cmd;
    public int oppUserPk;
    public bool success;
    public UserInfo userInfo;
}

public class PVPInfo : ByteSerializer
{
    public PVPCommand cmd;
    public int oppUserPk;
    public int XCount;
    public int YCount;
    public int combo;
    public int remainTime;
    public float colorCount;
    public bool withLaserEffect;
    public bool success;
    public bool oppDisconnected = false;
    public ProductSkill skill;
    public SwipeDirection dir;
    public PurchaseItemType item;

    public ProductInfo[] pros { get; set; }
    public int ArrayCount { get { return pros == null ? 0 : pros.Length; } }

    public byte[] Serialize()
    {
        List<byte> rets = new List<byte>();
        rets.AddRange(BitConverter.GetBytes((int)cmd));
        rets.AddRange(BitConverter.GetBytes(oppUserPk));
        rets.AddRange(BitConverter.GetBytes(XCount));
        rets.AddRange(BitConverter.GetBytes(YCount));
        rets.AddRange(BitConverter.GetBytes(combo));
        rets.AddRange(BitConverter.GetBytes(remainTime));
        rets.AddRange(BitConverter.GetBytes(colorCount));
        rets.AddRange(BitConverter.GetBytes(withLaserEffect));
        rets.AddRange(BitConverter.GetBytes(success));
        rets.AddRange(BitConverter.GetBytes(oppDisconnected));
        rets.AddRange(BitConverter.GetBytes((int)skill));
        rets.AddRange(BitConverter.GetBytes((int)dir));
        rets.AddRange(BitConverter.GetBytes((int)item));
        //rets.AddRange(Utils.Serialize(userInfo));

        if (pros != null)
        {
            foreach (ProductInfo info in pros)
                rets.AddRange(MyUtils.Serialize(info));
        }
        return rets.ToArray();
    }

    public void Deserialize(byte[] bytes)
    {
        try
        {
            int off = 0;
            cmd = (PVPCommand)BitConverter.ToInt32(bytes, off); off += 4;
            oppUserPk = BitConverter.ToInt32(bytes, off); off += 4;
            XCount = BitConverter.ToInt32(bytes, off); off += 4;
            YCount = BitConverter.ToInt32(bytes, off); off += 4;
            combo = BitConverter.ToInt32(bytes, off); off += 4;
            remainTime = BitConverter.ToInt32(bytes, off); off += 4;
            colorCount = BitConverter.ToSingle(bytes, off); off += 4;
            withLaserEffect = BitConverter.ToBoolean(bytes, off); off += 1;
            success = BitConverter.ToBoolean(bytes, off); off += 1;
            oppDisconnected = BitConverter.ToBoolean(bytes, off); off += 1;
            skill = (ProductSkill)BitConverter.ToInt32(bytes, off); off += 4;
            dir = (SwipeDirection)BitConverter.ToInt32(bytes, off); off += 4;
            item = (PurchaseItemType)BitConverter.ToInt32(bytes, off); off += 4;

            //userInfo = Utils.Deserialize<UserInfo>(ref bytes, off); off += Utils.Sizeof<UserInfo>();

            List<ProductInfo> infos = new List<ProductInfo>();
            int proSize = MyUtils.Sizeof<ProductInfo>();
            for (; off < bytes.Length; off += proSize)
                infos.Add(MyUtils.Deserialize<ProductInfo>(ref bytes, off));

            pros = infos.ToArray();
        }
        catch(Exception ex)
        {
            LOG.warn(ex.Message);
        }
    }
}
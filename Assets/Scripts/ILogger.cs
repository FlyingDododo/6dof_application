using UnityEngine;

// 日志记录接口，用于记录系统事件和数据包
public interface ILogger
{
    // 记录一般消息
    void LogMessage(string message);
    
    // 记录发送的数据包
    void LogSentPacket(byte[] data, string targetIP, int targetPort);
    
    // 记录收到的数据包
    void LogReceivedPacket(byte[] data, string sourceIP, int sourcePort);
    
    // 记录错误信息
    void LogError(string errorMessage);
} 
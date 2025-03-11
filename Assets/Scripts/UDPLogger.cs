using UnityEngine;
using System;
using System.IO;
using System.Text;

// 确保UDPLogger实现了ILogger接口
public class UDPLogger : MonoBehaviour, ILogger
{
    // 是否记录日志
    public bool enableLogging = true;
    
    // 日志文件路径
    private string logFilePath;
    
    void Start()
    {
        // 创建日志文件夹
        string logDirectory = Path.Combine(Application.persistentDataPath, "ChairLogs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }
        
        // 创建日志文件
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        logFilePath = Path.Combine(logDirectory, "UDPLog_" + timestamp + ".txt");
        
        Debug.Log("UDP日志记录器启动，日志路径: " + logFilePath);
        
        // 记录初始日志
        if (enableLogging)
        {
            LogMessage("日志开始记录 - " + DateTime.Now.ToString());
            LogMessage("-----------------------------");
        }
    }
    
    // 记录发送的数据包
    public void LogSentPacket(byte[] data, string targetIP, int targetPort)
    {
        if (!enableLogging) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] 发送数据包到 " + targetIP + ":" + targetPort);
        sb.AppendLine("数据内容 (十六进制): " + BitConverter.ToString(data).Replace("-", " "));
        
        // 尝试解析数据内容
        sb.AppendLine("解析内容:");
        if (data.Length >= 4 && data[0] == 0xEB && data[1] == 0x90)
        {
            sb.AppendLine("头部: EB 90 " + data[2].ToString("X2") + " " + data[3].ToString("X2"));
            
            if (data.Length >= 28) // 确保数据长度足够
            {
                float pitch = BitConverter.ToSingle(data, 4);
                float roll = BitConverter.ToSingle(data, 8);
                float yaw = BitConverter.ToSingle(data, 12);
                float sway = BitConverter.ToSingle(data, 16);
                float surge = BitConverter.ToSingle(data, 20);
                float heave = BitConverter.ToSingle(data, 24);
                
                sb.AppendLine(string.Format("俯仰(Pitch): {0:F2}, 横滚(Roll): {1:F2}, 航向(Yaw): {2:F2}", pitch, roll, yaw));
                sb.AppendLine(string.Format("横向(Sway): {0:F2}, 纵向(Surge): {1:F2}, 垂向(Heave): {2:F2}", sway, surge, heave));
            }
        }
        
        sb.AppendLine("-----------------------------");
        
        LogMessage(sb.ToString());
    }
    
    // 记录接收的数据包
    public void LogReceivedPacket(byte[] data, string sourceIP, int sourcePort)
    {
        if (!enableLogging) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] 接收数据包，来源 " + sourceIP + ":" + sourcePort);
        sb.AppendLine("数据内容 (十六进制): " + BitConverter.ToString(data).Replace("-", " "));
        sb.AppendLine("-----------------------------");
        
        LogMessage(sb.ToString());
    }
    
    // 记录错误信息
    public void LogError(string errorMessage)
    {
        if (!enableLogging) return;
        
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "] 错误: " + errorMessage);
        sb.AppendLine("-----------------------------");
        
        LogMessage(sb.ToString());
    }
    
    // 写入日志文件
    public void LogMessage(string message)
    {
        try
        {
            if (string.IsNullOrEmpty(logFilePath))
            {
                Debug.LogWarning("日志文件路径未初始化");
                return;
            }
            
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.Write(message);
            }
        }
        catch (Exception e)
        {
            Debug.LogError("写入日志文件失败: " + e.Message);
        }
    }
    
    void OnDestroy()
    {
        if (enableLogging)
        {
            LogMessage("日志记录结束 - " + DateTime.Now.ToString());
        }
    }
} 
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LogDisplay : MonoBehaviour
{
    public Text logText;
    public int maxLines = 20;
    
    private Queue<string> logLines = new Queue<string>();
    
    void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }
    
    void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }
    
    void HandleLog(string logString, string stackTrace, LogType type)
    {
        string coloredText;
        
        // 根据日志类型添加颜色
        switch (type)
        {
            case LogType.Error:
            case LogType.Exception:
                coloredText = "<color=red>" + logString + "</color>";
                break;
            case LogType.Warning:
                coloredText = "<color=yellow>" + logString + "</color>";
                break;
            default:
                coloredText = "<color=white>" + logString + "</color>";
                break;
        }
        
        // 添加时间戳
        string timestamp = System.DateTime.Now.ToString("HH:mm:ss");
        string logEntry = "[" + timestamp + "] " + coloredText;
        
        // 添加到队列
        logLines.Enqueue(logEntry);
        
        // 保持日志行数在最大限制内
        while (logLines.Count > maxLines)
        {
            logLines.Dequeue();
        }
        
        // 更新UI文本
        UpdateLogText();
    }
    
    void UpdateLogText()
    {
        if (logText == null) return;
        
        // 合并所有日志行
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        foreach (string line in logLines)
        {
            sb.AppendLine(line);
        }
        
        // 更新UI文本
        logText.text = sb.ToString();
    }
    
    // 清除日志
    public void ClearLogs()
    {
        logLines.Clear();
        UpdateLogText();
    }
} 
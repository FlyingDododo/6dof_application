using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

// 使用外部定义的ILogger接口

public class ChairController : MonoBehaviour, IChairController
{
    // UI引用
    public InputField ipInput;
    public InputField portInput;
    public Button connectButton;
    public Text statusText;
    
    // 状态指示器
    private Image statusIndicator;
    
    // 立方体引用（用于可视化）
    public Transform visualCube;
    
    // 日志记录器 - 通过MonoBehaviour引用避免直接依赖
    [SerializeField]
    private MonoBehaviour loggerObject;
    private ILogger logger 
    {
        get { return loggerObject as ILogger; }
    }
    
    // UDP通信
    private UdpClient udpClient;
    private string chairIP = "192.168.0.15"; // 根据测试程序截图设置默认IP
    private int chairPort = 20000; // 根据测试程序截图设置默认端口
    private bool isConnected = false;
    
    // 通信选项
    public bool useAltProtocol = false; // 如果为true，使用EB 90 02开头的协议
    public float sendInterval = 0.05f; // 发送间隔，默认20Hz
    private float sendTimer = 0f;
    
    // 座椅参数
    private float pitch = 0f; // 俯仰
    private float roll = 0f;  // 横滚
    private float yaw = 0f;   // 航向
    private float sway = 0f;  // 横向
    private float surge = 0f; // 纵向
    private float heave = 0f; // 垂向
    
    // 安全限制
    private float maxPitch = 15f;
    private float maxRoll = 15f;
    private float maxYaw = 15f;
    private float maxSway = 15f;
    private float maxSurge = 15f;
    private float maxHeave = 15f;
    
    void Start()
    {
        // 设置UI事件监听
        if (connectButton != null)
            connectButton.onClick.AddListener(ToggleConnection);
        
        // 初始化UI
        if (ipInput != null)
            ipInput.text = chairIP;
        if (portInput != null)
            portInput.text = chairPort.ToString();
        
        if (statusText != null)
            statusText.text = "未连接";
            
        // 获取状态指示器
        statusIndicator = GameObject.Find("StatusIndicator")?.GetComponent<Image>();
        if (statusIndicator != null)
            statusIndicator.color = Color.red;
            
        Debug.Log("座椅控制器初始化完成");
    }
    
    void Update()
    {
        // 更新可视化立方体的旋转（用于显示姿态）
        if (visualCube != null)
        {
            // 应用六自由度运动
            // 1. 首先设置旋转（俯仰、横滚、航向）
            Quaternion targetRotation = Quaternion.Euler(pitch, yaw, roll);
            visualCube.rotation = Quaternion.Slerp(visualCube.rotation, targetRotation, Time.deltaTime * 10f);
            
            // 2. 应用平移（考虑当前旋转方向）
            Vector3 basePosition = new Vector3(0, 1, 7); // 基础位置
            Vector3 translation = new Vector3(sway / 50f, heave / 50f, surge / 50f); // 缩小平移幅度
            
            // 应用平移（从当前位置平滑过渡）
            Vector3 targetPosition = basePosition + translation;
            visualCube.position = Vector3.Lerp(visualCube.position, targetPosition, Time.deltaTime * 5f);
        }
        
        // 如果已连接，按固定间隔发送数据
        if (isConnected)
        {
            sendTimer += Time.deltaTime;
            if (sendTimer >= sendInterval)
            {
                SendChairCommand();
                sendTimer = 0f;
            }
        }
    }
    
    public void ToggleConnection()
    {
        if (!isConnected)
        {
            Debug.Log("尝试连接座椅...");
            
            // 从UI获取IP和端口
            if (ipInput != null)
                chairIP = ipInput.text;
            if (portInput != null)
                int.TryParse(portInput.text, out chairPort);
            
            try
            {
                // 初始化UDP客户端
                udpClient = new UdpClient();
                
                // 更新UI
                if (connectButton != null)
                    connectButton.GetComponentInChildren<Text>().text = "断开";
                if (statusText != null)
                    statusText.text = "已连接到 " + chairIP + ":" + chairPort;
                    
                // 更新状态指示器
                if (statusIndicator != null)
                    statusIndicator.color = Color.green;
                
                isConnected = true;
                Debug.Log("已连接到: " + chairIP + ":" + chairPort);
                
                // 日志记录
                LogMessage("已连接到: " + chairIP + ":" + chairPort);
                
                // 发送初始数据包
                SendChairCommand();
            }
            catch (Exception e)
            {
                Debug.LogError("连接错误: " + e.Message);
                if (statusText != null)
                    statusText.text = "连接失败: " + e.Message;
                    
                // 更新状态指示器为错误状态
                if (statusIndicator != null)
                    statusIndicator.color = new Color(1.0f, 0.5f, 0); // 橙色表示错误
                    
                // 日志记录
                LogError("连接失败: " + e.Message);
            }
        }
        else
        {
            // 断开连接
            if (udpClient != null)
            {
                try {
                    // 发送零位置数据包
                    ResetAllMotion();
                    SendChairCommand();
                    
                    // 关闭UDP客户端
                    udpClient.Close();
                    udpClient = null;
                }
                catch (Exception e) {
                    Debug.LogError("断开连接错误: " + e.Message);
                }
            }
            
            // 更新UI
            if (connectButton != null)
                connectButton.GetComponentInChildren<Text>().text = "连接";
            if (statusText != null)
                statusText.text = "未连接";
                
            // 更新状态指示器
            if (statusIndicator != null)
                statusIndicator.color = Color.red;
            
            isConnected = false;
            Debug.Log("已断开连接");
            
            // 日志记录
            LogMessage("已断开连接");
        }
    }
    
    // 发送协议数据到座椅
    private void SendChairCommand()
    {
        try
        {
            byte[] data;
            
            if (useAltProtocol)
            {
                // 使用EB 90 02开头的协议（123字节）
                data = CreateEB9002Packet();
            }
            else
            {
                // 使用EB 90 01开头的协议（32字节）
                data = CreateEB9001Packet();
            }
            
            // 发送数据到座椅控制器
            udpClient.Send(data, data.Length, chairIP, chairPort);
            
            // 记录数据发送
            LogSentPacket(data, chairIP, chairPort);
        }
        catch (Exception e)
        {
            Debug.LogError("发送数据错误: " + e.Message);
            if (statusText != null)
                statusText.text = "发送错误: " + e.Message;
                
            LogError("发送数据错误: " + e.Message);
            
            // 断开连接
            ToggleConnection();
        }
    }
    
    // 创建EB 90 01协议数据包（32字节）
    private byte[] CreateEB9001Packet()
    {
        byte[] data = new byte[32];
        
        // 数据包头 EB 90 01 0A
        data[0] = 0xEB;
        data[1] = 0x90;
        data[2] = 0x01;
        data[3] = 0x0A;
        
        // 转换六个自由度参数到相应字节位置
        Buffer.BlockCopy(BitConverter.GetBytes(pitch), 0, data, 4, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(roll), 0, data, 8, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(yaw), 0, data, 12, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(sway), 0, data, 16, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(surge), 0, data, 20, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(heave), 0, data, 24, 4);
        
        // 最后4字节可能是校验和或其他数据，暂时保持为0
        data[28] = 0;
        data[29] = 0;
        data[30] = 0;
        data[31] = 0;
        
        return data;
    }
    
    // 创建EB 90 02协议数据包（根据日志观察是123字节）
    private byte[] CreateEB9002Packet()
    {
        byte[] data = new byte[123];
        
        // 数据包头 EB 90 02 7B
        data[0] = 0xEB;
        data[1] = 0x90;
        data[2] = 0x02;
        data[3] = 0x7B;
        
        // 根据日志文件中的数据模式填充剩余数据
        // 由于不确定具体格式，这里填充一些标准值
        
        // 在某些位置插入六个自由度参数
        // 这里的偏移量需要根据实际协议分析调整
        int offset = 16; // 假设的偏移量
        Buffer.BlockCopy(BitConverter.GetBytes(pitch), 0, data, offset, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(roll), 0, data, offset + 4, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(yaw), 0, data, offset + 8, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(sway), 0, data, offset + 12, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(surge), 0, data, offset + 16, 4);
        Buffer.BlockCopy(BitConverter.GetBytes(heave), 0, data, offset + 20, 4);
        
        // 设置一些默认值，模拟日志中看到的模式
        for (int i = 0; i < data.Length; i++)
        {
            if (i >= 40 && i < 52 && (i - 40) % 4 == 3)
            {
                data[i] = 0x3F; // 模拟一些浮点数1.0f的模式
            }
            else if (i >= 40 && i < 52 && (i - 40) % 4 == 2)
            {
                data[i] = 0x80; // 模拟一些浮点数模式
            }
        }
        
        // 设置结尾标记，根据日志中的模式
        data[data.Length - 4] = 0x0A;
        data[data.Length - 3] = 0x0A;
        data[data.Length - 2] = 0x0A;
        data[data.Length - 1] = 0xFF;
        
        return data;
    }
    
    // 安全的日志记录方法
    private void LogMessage(string message)
    {
        if (logger != null)
            logger.LogMessage(message);
        else
            Debug.Log(message);
    }
    
    private void LogSentPacket(byte[] data, string targetIP, int targetPort)
    {
        if (logger != null)
            logger.LogSentPacket(data, targetIP, targetPort);
        else
            Debug.Log("发送数据包: " + BitConverter.ToString(data).Replace("-", " "));
    }
    
    private void LogError(string errorMessage)
    {
        if (logger != null)
            logger.LogError(errorMessage);
        else
            Debug.LogError(errorMessage);
    }
    
    // 获取和设置座椅参数的公共方法（带安全限制）
    public float GetPitch() { return pitch; }
    public float GetRoll() { return roll; }
    public float GetYaw() { return yaw; }
    public float GetSway() { return sway; }
    public float GetSurge() { return surge; }
    public float GetHeave() { return heave; }
    
    public void SetPitch(float value) { pitch = Mathf.Clamp(value, -maxPitch, maxPitch); }
    public void SetRoll(float value) { roll = Mathf.Clamp(value, -maxRoll, maxRoll); }
    public void SetYaw(float value) { yaw = Mathf.Clamp(value, -maxYaw, maxYaw); }
    public void SetSway(float value) { sway = Mathf.Clamp(value, -maxSway, maxSway); }
    public void SetSurge(float value) { surge = Mathf.Clamp(value, -maxSurge, maxSurge); }
    public void SetHeave(float value) { heave = Mathf.Clamp(value, -maxHeave, maxHeave); }
    
    // 用于滑块的值变化监听（0-1值转换到-max到max）
    public void SetPitchFromSlider(float value) { SetPitch((value * 2 - 1) * maxPitch); }
    public void SetRollFromSlider(float value) { SetRoll((value * 2 - 1) * maxRoll); }
    public void SetYawFromSlider(float value) { SetYaw((value * 2 - 1) * maxYaw); }
    public void SetSwayFromSlider(float value) { SetSway((value * 2 - 1) * maxSway); }
    public void SetSurgeFromSlider(float value) { SetSurge((value * 2 - 1) * maxSurge); }
    public void SetHeaveFromSlider(float value) { SetHeave((value * 2 - 1) * maxHeave); }
    
    // 预设动作
    public void PresetForward()
    {
        SetPitch(-5f);
        SetRoll(0f);
        SetSurge(10f);
        
        Debug.Log("执行预设动作：前进");
        LogMessage("执行预设动作：前进");
    }
    
    public void PresetBackward()
    {
        SetPitch(5f);
        SetRoll(0f);
        SetSurge(-10f);
        
        Debug.Log("执行预设动作：后退");
        LogMessage("执行预设动作：后退");
    }
    
    public void PresetLeftTurn()
    {
        SetYaw(5f);
        SetRoll(5f);
        
        Debug.Log("执行预设动作：左转");
        LogMessage("执行预设动作：左转");
    }
    
    public void PresetRightTurn()
    {
        SetYaw(-5f);
        SetRoll(-5f);
        
        Debug.Log("执行预设动作：右转");
        LogMessage("执行预设动作：右转");
    }
    
    public void ResetAllMotion()
    {
        SetPitch(0f);
        SetRoll(0f);
        SetYaw(0f);
        SetSway(0f);
        SetSurge(0f);
        SetHeave(0f);
        
        Debug.Log("重置所有动作");
        LogMessage("重置所有动作");
    }
    
    // 切换协议类型
    public void ToggleProtocolType()
    {
        useAltProtocol = !useAltProtocol;
        Debug.Log("切换协议类型: " + (useAltProtocol ? "EB 90 02" : "EB 90 01"));
        LogMessage("切换协议类型: " + (useAltProtocol ? "EB 90 02" : "EB 90 01"));
    }
    
    // 设置日志记录器的公共方法
    public void SetLogger(MonoBehaviour loggerObj)
    {
        if (loggerObj is ILogger)
        {
            loggerObject = loggerObj;
            Debug.Log("日志记录器设置成功");
        }
        else
        {
            Debug.LogError("提供的对象不是ILogger类型");
        }
    }
    
    void OnDestroy()
    {
        if (udpClient != null)
        {
            udpClient.Close();
        }
    }
} 
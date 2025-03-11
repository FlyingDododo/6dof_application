using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class SceneSetup : MonoBehaviour
{
    // 预制体引用，可以在Unity编辑器中拖拽赋值
    public GameObject controlPanelPrefab;
    public GameObject visualCubePrefab;
    
    void Start()
    {
        Debug.Log("场景设置初始化");
        
        // 创建基本场景
        SetupBasicScene();
    }
    
    void SetupBasicScene()
    {
        // 首先创建Canvas确保所有UI组件正确显示
        CreateCanvas();
        
        // 创建可视化立方体
        CreateVisualCube();
        
        // 创建姿态控制面板
        CreateMotionPanel();
        
        // 创建连接控制面板
        CreateConnectionPanel();
        
        // 创建日志显示面板
        CreateLogPanel();
        
        // 创建控制组件
        CreateControlComponents();
    }
    
    // 创建并确保Canvas存在
    Canvas CreateCanvas()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            
            // 添加CanvasScaler并设置合适的缩放模式
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f; // 平衡宽高适配
            
            canvasObj.AddComponent<GraphicRaycaster>();
            
            // 添加EventSystem
            if (FindObjectOfType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }
        }
        return canvas;
    }
    
    void CreateVisualCube()
    {
        if (visualCubePrefab != null)
        {
            Instantiate(visualCubePrefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            // 创建座椅模型容器
            GameObject chairModel = new GameObject("ChairModel");
            chairModel.transform.position = new Vector3(0, 1, 7);
            
            // 创建一个更明显的座椅形状立方体
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "VisualCube";
            cube.transform.SetParent(chairModel.transform, false);
            cube.transform.localPosition = Vector3.zero;
            cube.transform.localScale = new Vector3(2f, 0.5f, 3f); // 更像座椅的比例
            
            // 添加更好的材质
            MeshRenderer renderer = cube.GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                Material mat = new Material(Shader.Find("Standard"));
                mat.color = new Color(0.3f, 0.3f, 0.35f); // 深灰色，更像座椅
                renderer.material = mat;
            }
            
            // 创建座椅靠背
            GameObject backrest = GameObject.CreatePrimitive(PrimitiveType.Cube);
            backrest.name = "Backrest";
            backrest.transform.SetParent(chairModel.transform, false);
            backrest.transform.localPosition = new Vector3(0, 0.5f, -1.0f);
            backrest.transform.localScale = new Vector3(2f, 1.0f, 0.5f);
            
            // 设置靠背材质
            MeshRenderer backRenderer = backrest.GetComponent<MeshRenderer>();
            if (backRenderer != null)
            {
                Material backMat = new Material(Shader.Find("Standard"));
                backMat.color = new Color(0.25f, 0.25f, 0.3f); // 深灰色，略深于座椅
                backRenderer.material = backMat;
            }
            
            // 添加座椅扶手
            CreateArmrest(chairModel.transform, true); // 左扶手
            CreateArmrest(chairModel.transform, false); // 右扶手
        }
    }
    
    // 创建座椅扶手
    void CreateArmrest(Transform parent, bool isLeft)
    {
        GameObject armrest = GameObject.CreatePrimitive(PrimitiveType.Cube);
        armrest.name = isLeft ? "LeftArmrest" : "RightArmrest";
        armrest.transform.SetParent(parent, false);
        
        // 扶手位置和尺寸
        float xOffset = isLeft ? -1.1f : 1.1f;
        armrest.transform.localPosition = new Vector3(xOffset, 0.25f, -0.3f);
        armrest.transform.localScale = new Vector3(0.2f, 0.2f, 2.0f);
        
        // 设置扶手材质
        MeshRenderer armRenderer = armrest.GetComponent<MeshRenderer>();
        if (armRenderer != null)
        {
            Material armMat = new Material(Shader.Find("Standard"));
            armMat.color = new Color(0.2f, 0.2f, 0.25f); // 更深的灰色
            armRenderer.material = armMat;
        }
    }
    
    // 姿态控制面板
    void CreateMotionPanel()
    {
        // 获取Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // 创建控制面板
        GameObject motionPanel = new GameObject("MotionPanel");
        motionPanel.transform.SetParent(canvas.transform, false);
        
        // 将姿态控制面板放在左侧
        RectTransform motionPanelRect = motionPanel.AddComponent<RectTransform>();
        motionPanelRect.anchorMin = new Vector2(0, 0.5f);
        motionPanelRect.anchorMax = new Vector2(0, 0.5f);
        motionPanelRect.pivot = new Vector2(0, 0.5f);
        motionPanelRect.sizeDelta = new Vector2(400, 500);
        motionPanelRect.anchoredPosition = new Vector2(20, 0);
        
        // 添加背景
        Image motionPanelImage = motionPanel.AddComponent<Image>();
        motionPanelImage.color = new Color(0.15f, 0.15f, 0.2f, 0.9f);
        
        // 创建标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(motionPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        titleRect.anchoredPosition = new Vector2(0, 0);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 18;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "姿态控制";
        
        // 创建六个滑块及其标签
        string[] axisNames = { "俯仰 (Pitch)", "横滚 (Roll)", "航向 (Yaw)", "横向 (Sway)", "纵向 (Surge)", "垂向 (Heave)" };
        
        // 创建两列滑块
        float sliderWidth = 180;
        float spacing = 20;
        float col1X = spacing + sliderWidth/2;
        float col2X = col1X + sliderWidth + spacing;
        
        float[] yPositions = { -70, -170, -270 };
        
        for (int i = 0; i < 6; i++)
        {
            float xPos = (i % 2 == 0) ? col1X : col2X;
            float yPos = yPositions[i/2];
            
            CreateMotionSlider(
                motionPanel.transform, 
                axisNames[i], 
                i, 
                new Vector2(xPos, yPos), 
                new Vector2(sliderWidth, 30)
            );
        }
        
        // 创建控制按钮
        GameObject resetButton = CreateButton(motionPanel.transform, "ResetButton", new Vector2(100, -370), "重置所有");
        resetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(120, 35);
        
        // 创建预设按钮
        string[] presetNames = { "前进", "后退", "左转", "右转" };
        float buttonWidth = 85;
        float buttonY = -420;
        
        float startX = spacing + buttonWidth/2;
        float buttonSpacing = (motionPanelRect.sizeDelta.x - 2*spacing - presetNames.Length * buttonWidth) / (presetNames.Length - 1);
        
        for (int i = 0; i < presetNames.Length; i++)
        {
            float xPos = startX + i * (buttonWidth + buttonSpacing);
            GameObject presetButton = CreateButton(
                motionPanel.transform, 
                "Preset" + i + "Button", 
                new Vector2(xPos, buttonY), 
                presetNames[i]
            );
            presetButton.GetComponent<RectTransform>().sizeDelta = new Vector2(buttonWidth, 35);
        }
    }
    
    // 姿态控制滑块
    GameObject CreateMotionSlider(Transform parent, string name, int axisIndex, Vector2 position, Vector2 size)
    {
        // 主滑块容器
        GameObject container = new GameObject("Slider_" + axisIndex);
        container.transform.SetParent(parent, false);
        
        RectTransform containerRect = container.AddComponent<RectTransform>();
        containerRect.anchorMin = new Vector2(0, 1);
        containerRect.anchorMax = new Vector2(0, 1);
        containerRect.pivot = new Vector2(0.5f, 0.5f);
        containerRect.sizeDelta = new Vector2(size.x, size.y * 3);
        containerRect.anchoredPosition = position;
        
        // 创建标签
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(container.transform, false);
        
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.anchorMin = new Vector2(0, 1);
        labelRect.anchorMax = new Vector2(1, 1);
        labelRect.pivot = new Vector2(0.5f, 1);
        labelRect.sizeDelta = new Vector2(0, 20);
        labelRect.anchoredPosition = new Vector2(0, 10);
        
        Text labelText = labelObj.AddComponent<Text>();
        labelText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        labelText.fontSize = 14;
        labelText.color = Color.white;
        labelText.alignment = TextAnchor.MiddleCenter;
        labelText.text = name;
        
        // 创建值显示
        GameObject valueObj = new GameObject("Value");
        valueObj.transform.SetParent(container.transform, false);
        
        RectTransform valueRect = valueObj.AddComponent<RectTransform>();
        valueRect.anchorMin = new Vector2(0, 1);
        valueRect.anchorMax = new Vector2(1, 1);
        valueRect.pivot = new Vector2(0.5f, 1);
        valueRect.sizeDelta = new Vector2(0, 20);
        valueRect.anchoredPosition = new Vector2(0, -12);
        
        Text valueText = valueObj.AddComponent<Text>();
        valueText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        valueText.fontSize = 14;
        valueText.color = Color.yellow;
        valueText.alignment = TextAnchor.MiddleCenter;
        valueText.text = "0.0";
        
        // 创建滑块
        GameObject sliderObj = new GameObject("Slider");
        sliderObj.transform.SetParent(container.transform, false);
        
        RectTransform sliderRect = sliderObj.AddComponent<RectTransform>();
        sliderRect.anchorMin = new Vector2(0, 1);
        sliderRect.anchorMax = new Vector2(1, 1);
        sliderRect.pivot = new Vector2(0.5f, 1);
        sliderRect.sizeDelta = new Vector2(0, 30);
        sliderRect.anchoredPosition = new Vector2(0, -35);
        
        // 创建滑块背景
        GameObject background = new GameObject("Background");
        background.transform.SetParent(sliderObj.transform, false);
        
        RectTransform bgRect = background.AddComponent<RectTransform>();
        bgRect.anchorMin = Vector2.zero;
        bgRect.anchorMax = Vector2.one;
        bgRect.sizeDelta = Vector2.zero;
        
        Image bgImage = background.AddComponent<Image>();
        bgImage.color = new Color(0.1f, 0.1f, 0.1f, 1f);
        
        // 创建滑块填充
        GameObject fill = new GameObject("Fill");
        fill.transform.SetParent(sliderObj.transform, false);
        
        RectTransform fillRect = fill.AddComponent<RectTransform>();
        fillRect.anchorMin = new Vector2(0, 0);
        fillRect.anchorMax = new Vector2(0.5f, 1);
        fillRect.sizeDelta = Vector2.zero;
        
        Image fillImage = fill.AddComponent<Image>();
        fillImage.color = new Color(0.2f, 0.6f, 0.8f, 1f);
        
        // 创建滑块把手
        GameObject handle = new GameObject("Handle");
        handle.transform.SetParent(sliderObj.transform, false);
        
        RectTransform handleRect = handle.AddComponent<RectTransform>();
        handleRect.anchorMin = new Vector2(0.5f, 0);
        handleRect.anchorMax = new Vector2(0.5f, 1);
        handleRect.sizeDelta = new Vector2(15, 0);
        handleRect.anchoredPosition = Vector2.zero;
        
        Image handleImage = handle.AddComponent<Image>();
        handleImage.color = new Color(0.9f, 0.9f, 0.9f, 1f);
        
        // 添加滑块组件
        Slider slider = sliderObj.AddComponent<Slider>();
        slider.fillRect = fillRect;
        slider.handleRect = handleRect;
        slider.targetGraphic = handleImage;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.value = 0.5f; // 默认居中
        
        // 添加事件监听
        int index = axisIndex; // 捕获当前索引值
        slider.onValueChanged.AddListener((float value) => {
            valueText.text = ((value * 2 - 1) * 15).ToString("F1");
            
            // 找到ChairController并设置相应的值
            ChairController controller = FindObjectOfType<ChairController>();
            if (controller != null)
            {
                switch (index)
                {
                    case 0: controller.SetPitchFromSlider(value); break;
                    case 1: controller.SetRollFromSlider(value); break;
                    case 2: controller.SetYawFromSlider(value); break;
                    case 3: controller.SetSwayFromSlider(value); break;
                    case 4: controller.SetSurgeFromSlider(value); break;
                    case 5: controller.SetHeaveFromSlider(value); break;
                }
            }
        });
        
        return container;
    }
    
    // 连接控制面板
    void CreateConnectionPanel()
    {
        if (controlPanelPrefab != null)
        {
            Instantiate(controlPanelPrefab);
            return;
        }
        
        // 获取Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // 创建连接控制面板，放在右下角
        GameObject panel = new GameObject("ConnectionPanel");
        panel.transform.SetParent(canvas.transform, false);
        
        // 创建带有背景的面板
        RectTransform panelRect = panel.AddComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(1, 0);
        panelRect.anchorMax = new Vector2(1, 0);
        panelRect.pivot = new Vector2(1, 0);
        panelRect.sizeDelta = new Vector2(250, 180);
        panelRect.anchoredPosition = new Vector2(-20, 20);
        
        // 添加面板背景
        Image panelImage = panel.AddComponent<Image>();
        panelImage.color = new Color(0.15f, 0.15f, 0.15f, 0.9f);
        
        // 创建面板标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(panel.transform, false);
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.pivot = new Vector2(0.5f, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        titleRect.anchoredPosition = new Vector2(0, 0);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 16;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "连接控制";
        
        // 添加基本的输入框和按钮
        CreateInputField(panel.transform, "IPInput", new Vector2(-125, -45), "IP地址", "192.168.0.15");
        CreateInputField(panel.transform, "PortInput", new Vector2(-125, -95), "端口", "20000");
        
        // 创建连接按钮
        GameObject connectBtn = CreateButton(panel.transform, "ConnectButton", new Vector2(-125, -140), "连接");
        
        // 创建连接状态指示器
        GameObject statusIndicator = new GameObject("StatusIndicator");
        statusIndicator.transform.SetParent(panel.transform, false);
        
        RectTransform statusRect = statusIndicator.AddComponent<RectTransform>();
        statusRect.anchorMin = new Vector2(1, 1);
        statusRect.anchorMax = new Vector2(1, 1);
        statusRect.pivot = new Vector2(0.5f, 0.5f);
        statusRect.sizeDelta = new Vector2(16, 16);
        statusRect.anchoredPosition = new Vector2(-15, -15);
        
        Image statusImage = statusIndicator.AddComponent<Image>();
        statusImage.color = Color.red; // 默认红色表示未连接
        
        // 创建状态文本
        GameObject statusTextObj = new GameObject("StatusText");
        statusTextObj.transform.SetParent(panel.transform, false);
        
        RectTransform statusTextRect = statusTextObj.AddComponent<RectTransform>();
        statusTextRect.anchorMin = new Vector2(0, 0);
        statusTextRect.anchorMax = new Vector2(1, 0);
        statusTextRect.pivot = new Vector2(0.5f, 0);
        statusTextRect.sizeDelta = new Vector2(0, 20);
        statusTextRect.anchoredPosition = new Vector2(0, 5);
        
        Text statusText = statusTextObj.AddComponent<Text>();
        statusText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        statusText.fontSize = 14;
        statusText.color = Color.white;
        statusText.alignment = TextAnchor.MiddleCenter;
        statusText.text = "未连接";
    }
    
    GameObject CreateInputField(Transform parent, string name, Vector2 position, string placeholder, string defaultText)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = obj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(200, 40);
        rectTransform.anchorMin = new Vector2(0.5f, 1);
        rectTransform.anchorMax = new Vector2(0.5f, 1);
        rectTransform.pivot = new Vector2(0.5f, 1);
        rectTransform.anchoredPosition = position;
        
        Image image = obj.AddComponent<Image>();
        image.color = new Color(1, 1, 1, 0.7f);
        
        InputField inputField = obj.AddComponent<InputField>();
        
        // 创建文本组件
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = new Vector2(10, 6);
        textRectTransform.offsetMax = new Vector2(-10, -7);
        
        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 16;
        text.color = Color.black;
        text.alignment = TextAnchor.MiddleLeft;
        text.text = defaultText;
        
        // 创建占位符
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(obj.transform, false);
        RectTransform placeholderRectTransform = placeholderObj.AddComponent<RectTransform>();
        placeholderRectTransform.anchorMin = Vector2.zero;
        placeholderRectTransform.anchorMax = Vector2.one;
        placeholderRectTransform.offsetMin = new Vector2(10, 6);
        placeholderRectTransform.offsetMax = new Vector2(-10, -7);
        
        Text placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        placeholderText.fontSize = 16;
        placeholderText.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        placeholderText.alignment = TextAnchor.MiddleLeft;
        placeholderText.text = placeholder;
        
        // 设置InputField组件
        inputField.textComponent = text;
        inputField.placeholder = placeholderText;
        inputField.text = defaultText;
        
        // 添加标签
        GameObject labelObj = new GameObject("Label");
        labelObj.transform.SetParent(parent, false);
        RectTransform labelRect = labelObj.AddComponent<RectTransform>();
        labelRect.sizeDelta = new Vector2(80, 30);
        labelRect.anchorMin = new Vector2(0.5f, 1);
        labelRect.anchorMax = new Vector2(0.5f, 1);
        labelRect.pivot = new Vector2(0.5f, 1);
        labelRect.anchoredPosition = new Vector2(-140, position.y + 5);
        
        Text label = labelObj.AddComponent<Text>();
        label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        label.fontSize = 16;
        label.color = Color.white;
        label.alignment = TextAnchor.MiddleRight;
        label.text = placeholder + ":";
        
        return obj;
    }
    
    GameObject CreateButton(Transform parent, string name, Vector2 position, string buttonText)
    {
        GameObject obj = new GameObject(name);
        obj.transform.SetParent(parent, false);
        
        RectTransform rectTransform = obj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(160, 40);
        rectTransform.anchorMin = new Vector2(0.5f, 1);
        rectTransform.anchorMax = new Vector2(0.5f, 1);
        rectTransform.pivot = new Vector2(0.5f, 1);
        rectTransform.anchoredPosition = position;
        
        Image image = obj.AddComponent<Image>();
        image.color = new Color(0.2f, 0.6f, 0.8f, 1.0f);
        
        Button button = obj.AddComponent<Button>();
        button.targetGraphic = image;
        ColorBlock colors = button.colors;
        colors.highlightedColor = new Color(0.3f, 0.7f, 0.9f, 1.0f);
        colors.pressedColor = new Color(0.1f, 0.5f, 0.7f, 1.0f);
        button.colors = colors;
        
        // 创建文本
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(obj.transform, false);
        RectTransform textRectTransform = textObj.AddComponent<RectTransform>();
        textRectTransform.anchorMin = Vector2.zero;
        textRectTransform.anchorMax = Vector2.one;
        textRectTransform.offsetMin = Vector2.zero;
        textRectTransform.offsetMax = Vector2.zero;
        
        Text text = textObj.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 18;
        text.color = Color.white;
        text.alignment = TextAnchor.MiddleCenter;
        text.text = buttonText;
        
        return obj;
    }
    
    // 日志显示面板
    void CreateLogPanel()
    {
        // 获取Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;
        
        // 创建日志面板
        GameObject logPanel = new GameObject("LogPanel");
        logPanel.transform.SetParent(canvas.transform, false);
        
        // 将日志面板放在右侧中部
        RectTransform logPanelRect = logPanel.AddComponent<RectTransform>();
        logPanelRect.anchorMin = new Vector2(1, 0.5f);
        logPanelRect.anchorMax = new Vector2(1, 0.5f);
        logPanelRect.pivot = new Vector2(1, 0.5f);
        logPanelRect.sizeDelta = new Vector2(400, 300);
        logPanelRect.anchoredPosition = new Vector2(-20, 150);
        
        // 添加背景
        Image logPanelImage = logPanel.AddComponent<Image>();
        logPanelImage.color = new Color(0, 0, 0, 0.8f);
        
        // 创建标题
        GameObject titleObj = new GameObject("Title");
        titleObj.transform.SetParent(logPanel.transform, false);
        
        RectTransform titleRect = titleObj.AddComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0, 1);
        titleRect.anchorMax = new Vector2(1, 1);
        titleRect.sizeDelta = new Vector2(0, 30);
        titleRect.anchoredPosition = new Vector2(0, 0);
        
        Text titleText = titleObj.AddComponent<Text>();
        titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        titleText.fontSize = 16;
        titleText.color = Color.white;
        titleText.alignment = TextAnchor.MiddleCenter;
        titleText.text = "系统日志";
        
        // 创建日志文本区域
        GameObject logTextObj = new GameObject("LogText");
        logTextObj.transform.SetParent(logPanel.transform, false);
        
        RectTransform logTextRect = logTextObj.AddComponent<RectTransform>();
        logTextRect.anchorMin = new Vector2(0, 0);
        logTextRect.anchorMax = new Vector2(1, 1);
        logTextRect.offsetMin = new Vector2(10, 5);
        logTextRect.offsetMax = new Vector2(-10, -30);
        
        Text logText = logTextObj.AddComponent<Text>();
        logText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        logText.fontSize = 14;
        logText.color = Color.white;
        logText.alignment = TextAnchor.LowerLeft;
        logText.text = "系统初始化...\n";
        logText.supportRichText = true;
        
        // 添加日志显示组件
        LogDisplay logDisplay = logPanel.AddComponent<LogDisplay>();
        logDisplay.logText = logText;
        
        // 创建清除按钮
        GameObject clearButtonObj = new GameObject("ClearButton");
        clearButtonObj.transform.SetParent(logPanel.transform, false);
        
        RectTransform clearButtonRect = clearButtonObj.AddComponent<RectTransform>();
        clearButtonRect.anchorMin = new Vector2(1, 1);
        clearButtonRect.anchorMax = new Vector2(1, 1);
        clearButtonRect.sizeDelta = new Vector2(80, 25);
        clearButtonRect.anchoredPosition = new Vector2(-45, -15);
        
        Image clearButtonImage = clearButtonObj.AddComponent<Image>();
        clearButtonImage.color = new Color(0.3f, 0.3f, 0.3f, 1);
        
        Button clearButton = clearButtonObj.AddComponent<Button>();
        clearButton.targetGraphic = clearButtonImage;
        
        GameObject buttonTextObj = new GameObject("Text");
        buttonTextObj.transform.SetParent(clearButtonObj.transform, false);
        
        RectTransform buttonTextRect = buttonTextObj.AddComponent<RectTransform>();
        buttonTextRect.anchorMin = Vector2.zero;
        buttonTextRect.anchorMax = Vector2.one;
        buttonTextRect.offsetMin = Vector2.zero;
        buttonTextRect.offsetMax = Vector2.zero;
        
        Text buttonText = buttonTextObj.AddComponent<Text>();
        buttonText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        buttonText.fontSize = 14;
        buttonText.color = Color.white;
        buttonText.alignment = TextAnchor.MiddleCenter;
        buttonText.text = "清除";
        
        // 设置清除按钮事件
        clearButton.onClick.AddListener(() => {
            if (logDisplay != null)
                logDisplay.ClearLogs();
        });
    }
    
    void CreateControlComponents()
    {
        // 创建日志记录器
        GameObject loggerObj = new GameObject("UDPLogger");
        UDPLogger logger = loggerObj.AddComponent<UDPLogger>();
        
        // 创建座椅控制器
        GameObject controllerObj = new GameObject("ChairController");
        ChairController controller = controllerObj.AddComponent<ChairController>();
        
        // 查找UI组件并设置引用 - 注意面板名称已更改为ConnectionPanel
        InputField ipInput = GameObject.Find("IPInput")?.GetComponent<InputField>();
        InputField portInput = GameObject.Find("PortInput")?.GetComponent<InputField>();
        Button connectButton = GameObject.Find("ConnectButton")?.GetComponent<Button>();
        Text statusText = GameObject.Find("StatusText")?.GetComponent<Text>();
        
        if (controller != null)
        {
            controller.ipInput = ipInput;
            controller.portInput = portInput;
            controller.connectButton = connectButton;
            controller.statusText = statusText;
            
            // 查找可视化立方体 - 现在在ChairModel下
            Transform visualCube = GameObject.Find("ChairModel")?.transform;
            if (visualCube != null)
            {
                controller.visualCube = visualCube;
            }
            
            // 设置日志记录器
            controller.SetLogger(logger);
            
            // 设置重置按钮事件
            Button resetButton = GameObject.Find("ResetButton")?.GetComponent<Button>();
            if (resetButton != null)
            {
                resetButton.onClick.AddListener(controller.ResetAllMotion);
            }
            
            // 设置预设按钮事件
            Button presetForwardButton = GameObject.Find("Preset0Button")?.GetComponent<Button>();
            if (presetForwardButton != null)
                presetForwardButton.onClick.AddListener(controller.PresetForward);
                
            Button presetBackwardButton = GameObject.Find("Preset1Button")?.GetComponent<Button>();
            if (presetBackwardButton != null)
                presetBackwardButton.onClick.AddListener(controller.PresetBackward);
                
            Button presetLeftButton = GameObject.Find("Preset2Button")?.GetComponent<Button>();
            if (presetLeftButton != null)
                presetLeftButton.onClick.AddListener(controller.PresetLeftTurn);
                
            Button presetRightButton = GameObject.Find("Preset3Button")?.GetComponent<Button>();
            if (presetRightButton != null)
                presetRightButton.onClick.AddListener(controller.PresetRightTurn);
        }
    }
} 
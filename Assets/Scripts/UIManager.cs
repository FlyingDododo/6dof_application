using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // 引用ChairController - 使用接口以避免直接依赖
    [SerializeField]
    private MonoBehaviour chairControllerObj;
    private IChairController chairController
    {
        get { return chairControllerObj as IChairController; }
    }
    
    // UI面板
    public GameObject connectionPanel;
    public GameObject controlPanel;
    
    // 滑块引用
    public Slider pitchSlider;
    public Slider rollSlider;
    public Slider yawSlider;
    public Slider swaySlider;
    public Slider surgeSlider;
    public Slider heaveSlider;
    
    // 滑块值显示
    public Text pitchValueText;
    public Text rollValueText;
    public Text yawValueText;
    public Text swayValueText;
    public Text surgeValueText;
    public Text heaveValueText;
    
    // 连接UI引用
    public InputField ipInput;
    public InputField portInput;
    public Button connectButton;
    public Text statusText;
    
    void Start()
    {
        Debug.Log("UIManager初始化");
        
        // 初始化滑块事件
        SetupSliderEvents();
    }
    
    void SetupSliderEvents() 
    {
        // 初始化滑块事件
        if (pitchSlider != null)
            pitchSlider.onValueChanged.AddListener(UpdatePitchValue);
        if (rollSlider != null)
            rollSlider.onValueChanged.AddListener(UpdateRollValue);
        if (yawSlider != null)
            yawSlider.onValueChanged.AddListener(UpdateYawValue);
        if (swaySlider != null)
            swaySlider.onValueChanged.AddListener(UpdateSwayValue);
        if (surgeSlider != null)
            surgeSlider.onValueChanged.AddListener(UpdateSurgeValue);
        if (heaveSlider != null)
            heaveSlider.onValueChanged.AddListener(UpdateHeaveValue);
        
        // 设置初始值显示
        UpdateAllValueTexts();
    }
    
    // 更新滑块值文本并传递给ChairController
    void UpdatePitchValue(float value)
    {
        float mappedValue = (value * 2 - 1) * 15; // 映射到-15到15范围
        if (pitchValueText != null)
            pitchValueText.text = mappedValue.ToString("F1");
        if (chairController != null)
            chairController.SetPitchFromSlider(value);
    }
    
    void UpdateRollValue(float value)
    {
        float mappedValue = (value * 2 - 1) * 15;
        if (rollValueText != null)
            rollValueText.text = mappedValue.ToString("F1");
        if (chairController != null)
            chairController.SetRollFromSlider(value);
    }
    
    void UpdateYawValue(float value)
    {
        float mappedValue = (value * 2 - 1) * 15;
        if (yawValueText != null)
            yawValueText.text = mappedValue.ToString("F1");
        if (chairController != null)
            chairController.SetYawFromSlider(value);
    }
    
    void UpdateSwayValue(float value)
    {
        float mappedValue = (value * 2 - 1) * 15;
        if (swayValueText != null)
            swayValueText.text = mappedValue.ToString("F1");
        if (chairController != null)
            chairController.SetSwayFromSlider(value);
    }
    
    void UpdateSurgeValue(float value)
    {
        float mappedValue = (value * 2 - 1) * 15;
        if (surgeValueText != null)
            surgeValueText.text = mappedValue.ToString("F1");
        if (chairController != null)
            chairController.SetSurgeFromSlider(value);
    }
    
    void UpdateHeaveValue(float value)
    {
        float mappedValue = (value * 2 - 1) * 15;
        if (heaveValueText != null)
            heaveValueText.text = mappedValue.ToString("F1");
        if (chairController != null)
            chairController.SetHeaveFromSlider(value);
    }
    
    // 更新所有值显示
    void UpdateAllValueTexts()
    {
        if (pitchSlider != null) UpdatePitchValue(pitchSlider.value);
        if (rollSlider != null) UpdateRollValue(rollSlider.value);
        if (yawSlider != null) UpdateYawValue(yawSlider.value);
        if (swaySlider != null) UpdateSwayValue(swaySlider.value);
        if (surgeSlider != null) UpdateSurgeValue(surgeSlider.value);
        if (heaveSlider != null) UpdateHeaveValue(heaveSlider.value);
    }
    
    // 重置所有滑块
    public void ResetAllSliders()
    {
        if (pitchSlider != null) pitchSlider.value = 0.5f;
        if (rollSlider != null) rollSlider.value = 0.5f;
        if (yawSlider != null) yawSlider.value = 0.5f;
        if (swaySlider != null) swaySlider.value = 0.5f;
        if (surgeSlider != null) surgeSlider.value = 0.5f;
        if (heaveSlider != null) heaveSlider.value = 0.5f;
        
        if (chairController != null)
            chairController.ResetAllMotion();
    }
    
    // 预设动作按钮
    public void OnForwardButtonClick()
    {
        if (chairController != null)
            chairController.PresetForward();
        // 暂时注释掉，避免编译错误
        // UpdateSlidersFromController();
    }
    
    public void OnBackwardButtonClick()
    {
        if (chairController != null)
            chairController.PresetBackward();
        // UpdateSlidersFromController();
    }
    
    public void OnLeftTurnButtonClick()
    {
        if (chairController != null)
            chairController.PresetLeftTurn();
        // UpdateSlidersFromController();
    }
    
    public void OnRightTurnButtonClick()
    {
        if (chairController != null)
            chairController.PresetRightTurn();
        // UpdateSlidersFromController();
    }
    
    // 这个方法需要访问ChairController的私有字段，暂时注释掉
    /*
    private void UpdateSlidersFromController()
    {
        // 实际项目中需要ChairController提供GetXXX方法或公开字段
        // 这里仅作为示例
    }
    */
} 
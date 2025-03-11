using UnityEngine;

// 定义座椅控制器接口，用于解耦UIManager和ChairController
public interface IChairController
{
    // 设置座椅姿态参数
    void SetPitchFromSlider(float value);
    void SetRollFromSlider(float value);
    void SetYawFromSlider(float value);
    void SetSwayFromSlider(float value);
    void SetSurgeFromSlider(float value);
    void SetHeaveFromSlider(float value);
    
    // 预设动作
    void PresetForward();
    void PresetBackward();
    void PresetLeftTurn();
    void PresetRightTurn();
    
    // 重置所有动作
    void ResetAllMotion();
} 
using UnityEngine;

// 使物体始终面向摄像机的简单脚本
public class LookAtCamera : MonoBehaviour
{
    private Camera targetCamera;
    
    void Start()
    {
        // 获取主摄像机
        targetCamera = Camera.main;
    }
    
    void LateUpdate()
    {
        if (targetCamera != null)
        {
            // 使物体始终面向摄像机
            transform.LookAt(targetCamera.transform);
            
            // 翻转Y轴，确保文本方向正确
            transform.rotation = Quaternion.LookRotation(-transform.forward);
        }
    }
} 
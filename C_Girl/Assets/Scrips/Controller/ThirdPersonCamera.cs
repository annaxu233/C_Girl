using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("目标设置")]
    public Transform target;  // 需要跟随的物体

    [Header("相机设置")]
    public float distance = 5f;    // 相机距离目标的距离
    public float height = 2f;      // 相机高度偏移
    public float smoothSpeed = 10f; // 相机平滑跟随速度

    void LateUpdate()
    {
        if (target == null) return;

        // 计算相机目标位置
        Vector3 targetPosition = target.position - target.forward * distance + Vector3.up * height;

        // 平滑移动相机到目标位置
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // 让相机始终看向目标
        transform.LookAt(target.position + Vector3.up * (height * 0.5f));
    }
}

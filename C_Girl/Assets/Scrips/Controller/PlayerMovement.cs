using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("移动设置")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;

    private CharacterController controller;
    private Vector3 moveDirection;

    void Start()
    {
        // 获取角色控制器组件
        controller = GetComponent<CharacterController>();

        // 锁定鼠标到屏幕中心
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // 获取WSAD输入
        float horizontal = Input.GetAxis("Horizontal"); // A/D键
        float vertical = Input.GetAxis("Vertical");     // W/S键

        // 计算移动方向
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // 忽略Y轴方向，防止上下移动
        forward.y = 0f;
        right.y = 0f;

        // 归一化方向向量，确保斜向移动速度相同
        forward.Normalize();
        right.Normalize();

        // 计算最终移动方向
        moveDirection = (forward * vertical + right * horizontal).normalized;

        // 移动角色
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // 旋转角色面向移动方向
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToMove : MonoBehaviour
{
    // Start is called before the first frame update
    // 移动速度
    public float moveSpeed = 5f;
    // 旋转速度（使物体面向移动方向）
    public float rotationSpeed = 10f;
    // 地面层（用于射线检测过滤）
    public LayerMask groundLayer;

    private Rigidbody rb;
    //rb就是对应跟着鼠标走的物体
    private Vector3 targetPosition;
    private bool isMoving = false;
    //vector3暂时还没想明白

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // 初始目标位置设为物体当前位置
        targetPosition = transform.position;
    }

    void Update()
    {
        // 检测鼠标左键点击
        if (Input.GetMouseButtonDown(0))
        {
            // 创建从主摄像机到鼠标点击位置的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // 射线检测，只检测地面层
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // 设置目标位置为射线击中地面的点
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        // 如果需要移动，更新物体位置和旋转
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        // 计算物体到目标位置的方向
        Vector3 direction = (targetPosition - transform.position).normalized;
        // 忽略Y轴，确保在同一平面移动
        direction.y = 0;

        // 如果物体到达目标位置附近，停止移动
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            rb.velocity = Vector3.zero;
            return;
        }

        // 移动物体
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        // 使物体面向移动方向
        if (direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

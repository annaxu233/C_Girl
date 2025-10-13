using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    [Header("Ŀ������")]
    public Transform target;  // ��Ҫ���������

    [Header("�������")]
    public float distance = 5f;    // �������Ŀ��ľ���
    public float height = 2f;      // ����߶�ƫ��
    public float smoothSpeed = 10f; // ���ƽ�������ٶ�

    void LateUpdate()
    {
        if (target == null) return;

        // �������Ŀ��λ��
        Vector3 targetPosition = target.position - target.forward * distance + Vector3.up * height;

        // ƽ���ƶ������Ŀ��λ��
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);

        // �����ʼ�տ���Ŀ��
        transform.LookAt(target.position + Vector3.up * (height * 0.5f));
    }
}

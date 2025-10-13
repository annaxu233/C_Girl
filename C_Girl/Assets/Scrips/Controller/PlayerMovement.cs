using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("�ƶ�����")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 15f;

    private CharacterController controller;
    private Vector3 moveDirection;

    void Start()
    {
        // ��ȡ��ɫ���������
        controller = GetComponent<CharacterController>();

        // ������굽��Ļ����
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // ��ȡWSAD����
        float horizontal = Input.GetAxis("Horizontal"); // A/D��
        float vertical = Input.GetAxis("Vertical");     // W/S��

        // �����ƶ�����
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right = Camera.main.transform.right;

        // ����Y�᷽�򣬷�ֹ�����ƶ�
        forward.y = 0f;
        right.y = 0f;

        // ��һ������������ȷ��б���ƶ��ٶ���ͬ
        forward.Normalize();
        right.Normalize();

        // ���������ƶ�����
        moveDirection = (forward * vertical + right * horizontal).normalized;

        // �ƶ���ɫ
        if (moveDirection.magnitude >= 0.1f)
        {
            controller.Move(moveDirection * moveSpeed * Time.deltaTime);

            // ��ת��ɫ�����ƶ�����
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}


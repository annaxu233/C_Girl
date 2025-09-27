using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToMove : MonoBehaviour
{
    // Start is called before the first frame update
    // �ƶ��ٶ�
    public float moveSpeed = 5f;
    // ��ת�ٶȣ�ʹ���������ƶ�����
    public float rotationSpeed = 10f;
    // ����㣨�������߼����ˣ�
    public LayerMask groundLayer;

    private Rigidbody rb;
    //rb���Ƕ�Ӧ��������ߵ�����
    private Vector3 targetPosition;
    private bool isMoving = false;
    //vector3��ʱ��û������

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // ��ʼĿ��λ����Ϊ���嵱ǰλ��
        targetPosition = transform.position;
    }

    void Update()
    {
        // ������������
        if (Input.GetMouseButtonDown(0))
        {
            // ��������������������λ�õ�����
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // ���߼�⣬ֻ�������
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
            {
                // ����Ŀ��λ��Ϊ���߻��е���ĵ�
                targetPosition = hit.point;
                isMoving = true;
            }
        }

        // �����Ҫ�ƶ�����������λ�ú���ת
        if (isMoving)
        {
            MoveToTarget();
        }
    }

    void MoveToTarget()
    {
        // �������嵽Ŀ��λ�õķ���
        Vector3 direction = (targetPosition - transform.position).normalized;
        // ����Y�ᣬȷ����ͬһƽ���ƶ�
        direction.y = 0;

        // ������嵽��Ŀ��λ�ø�����ֹͣ�ƶ�
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            isMoving = false;
            rb.velocity = Vector3.zero;
            return;
        }

        // �ƶ�����
        Vector3 movement = direction * moveSpeed * Time.deltaTime;
        rb.MovePosition(transform.position + movement);

        // ʹ���������ƶ�����
        if (direction != Vector3.zero)
        {
            Quaternion newRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, rotationSpeed * Time.deltaTime);
        }
    }
}

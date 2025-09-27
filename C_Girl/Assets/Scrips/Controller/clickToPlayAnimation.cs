using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToPlayAnimation : MonoBehaviour
{
    // ����������������Inspector�и�ֵ��
    public Animator targetAnimator;
    // ����������������Animator�����õĲ���һ�£�
    public string animationParameter = "IsPlaying";
    // �Ƿ�����ֻ����һ�Σ������ѭ���л�����/ֹͣ��
    public bool playOnce = true;

    private bool isAnimationPlaying = false;

    void Start()
    {
        // ���δ�ֶ�ָ��Animator���Զ���ȡ��ǰ�����ϵ�Animator���
        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }

        // У���Ҫ���
        if (targetAnimator == null)
        {
            Debug.LogError("������δ�ҵ�Animator�����", this);
        }
    }

    // ��������������������ײ��Collider��
    private void OnMouseDown()
    {
        // ȷ��Animator�������
        if (targetAnimator == null) return;

        // ����������ƶ�������״̬
        if (playOnce && !isAnimationPlaying)
        {
            // ֻ����һ��
            PlayAnimation();
        }
        else
        {
            // �л�����/ֹͣ״̬
            ToggleAnimation();
        }
    }

    // ���Ŷ���
    public void PlayAnimation()
    {
        isAnimationPlaying = true;
        // ���ö�������������Animator�еĲ�������ѡ��SetBool/SetTrigger�ȣ�
        if (targetAnimator.parameters.Length > 0)
        {
            var param = targetAnimator.GetParameter(0);
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    targetAnimator.SetBool(animationParameter, true);
                    break;
                case AnimatorControllerParameterType.Trigger:
                    targetAnimator.SetTrigger(animationParameter);
                    break;
                case AnimatorControllerParameterType.Float:
                    targetAnimator.SetFloat(animationParameter, 1);
                    break;
                case AnimatorControllerParameterType.Int:
                    targetAnimator.SetInteger(animationParameter, 1);
                    break;
            }
        }
    }

    // ֹͣ����
    public void StopAnimation()
    {
        isAnimationPlaying = false;
        if (targetAnimator.parameters.Length > 0)
        {
            var param = targetAnimator.GetParameter(0);
            switch (param.type)
            {
                case AnimatorControllerParameterType.Bool:
                    targetAnimator.SetBool(animationParameter, false);
                    break;
                case AnimatorControllerParameterType.Float:
                    targetAnimator.SetFloat(animationParameter, 0);
                    break;
                case AnimatorControllerParameterType.Int:
                    targetAnimator.SetInteger(animationParameter, 0);
                    break;
            }
        }
    }

    // �л���������/ֹͣ״̬
    public void ToggleAnimation()
    {
        if (isAnimationPlaying)
        {
            StopAnimation();
        }
        else
        {
            PlayAnimation();
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToPlayAnimation : MonoBehaviour
{
    // 动画控制器（需在Inspector中赋值）
    public Animator targetAnimator;
    // 动画参数名（需与Animator中设置的参数一致）
    public string animationParameter = "IsPlaying";
    // 是否点击后只播放一次（否则会循环切换播放/停止）
    public bool playOnce = true;

    private bool isAnimationPlaying = false;

    void Start()
    {
        // 如果未手动指定Animator，自动获取当前物体上的Animator组件
        if (targetAnimator == null)
        {
            targetAnimator = GetComponent<Animator>();
        }

        // 校验必要组件
        if (targetAnimator == null)
        {
            Debug.LogError("物体上未找到Animator组件！", this);
        }
    }

    // 检测鼠标点击（需物体有碰撞器Collider）
    private void OnMouseDown()
    {
        // 确保Animator组件存在
        if (targetAnimator == null) return;

        // 根据需求控制动画播放状态
        if (playOnce && !isAnimationPlaying)
        {
            // 只播放一次
            PlayAnimation();
        }
        else
        {
            // 切换播放/停止状态
            ToggleAnimation();
        }
    }

    // 播放动画
    public void PlayAnimation()
    {
        isAnimationPlaying = true;
        // 设置动画参数（根据Animator中的参数类型选择SetBool/SetTrigger等）
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

    // 停止动画
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

    // 切换动画播放/停止状态
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

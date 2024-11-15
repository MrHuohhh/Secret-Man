using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using QFramework;
using UnityEngine;
using Log = UnityGameFramework.Runtime.Log;

public class KongjieEntity : SampleEntity
{
    public virtual bool IsAIPlayer
    {
        get => false;
    }

    private bool mCtrlable;
    public int JieValue = 0;
    private bool mIsSee = true;

    private float timer = 3;
    Transform m_transform;


    public bool Ctrlable
    {
        get => mCtrlable;
        set
        {
            mCtrlable = value;
            // if (!IsAIPlayer) GF.StaticUI.JoystickEnable = mCtrlable;
        }
    }

    public bool IsWatching
    {
        get => mIsSee;
        set
        {
            mIsSee = value;
            // if (!IsAIPlayer) GF.StaticUI.JoystickEnable = mCtrlable;
        }
    }


    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        m_transform = GetComponent<Transform>();
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        timer -= realElapseSeconds;

        if (timer <= 0)
        {
            // 随机选择下一个状态
            bool shouldRotate = UnityEngine.Random.Range(0, 2) == 0;

            if (shouldRotate)
            {
                // 旋转180度并设置mIsSee为false
                timer = 2; // 下一次旋转的间隔时间
                ActionKit.Sequence()
                    .Callback(() => m_transform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f, RotateMode.Fast))
                    .Delay(0.2f)
                    .Callback(() => mIsSee = false)
                    .Start(this);
            }
            else
            {
                // 旋转回初始状态并设置mIsSee为true
                timer = 1; // 下一次检查的间隔时间
                ActionKit.Sequence()
                    .Callback(() => m_transform.DOLocalRotate(new Vector3(0, 0, -90), 0.5f, RotateMode.Fast))
                    .Delay(0.5f)
                    .Callback(() => mIsSee = true)
                    .Start(this);
            }
        }
    }


    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
    }
}
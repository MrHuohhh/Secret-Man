using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using QFramework;
using UnityEngine;
using Log = UnityGameFramework.Runtime.Log;

public class DoorManEntity : SampleEntity
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
                // mIsSee为false
                timer = 5; // 下一次旋转的间隔时间
                ActionKit.Sequence()
                    .Callback(() =>   //x轴从初始位置0到-6停下,震动,增量移动
                        m_transform.DOMove(new Vector3(50, -40,-1), 0.2f).SetEase(Ease.InOutSine))
                    .Delay(0.2f)
                    .Callback(() =>    m_transform.DOShakePosition(2f, new Vector3(5f, 5f, 0), 10, 90, false, true))
                    .Delay(2f)
                    .Callback(() =>   //x轴从初始位置0到-6停下,震动
                        m_transform.DOMove(new Vector3(16, -24, -1), 0.5f).SetEase(Ease.InOutSine))
                    .Delay(0.2f)
                    .Callback(() => mIsSee = true)
                    .Start(this);
              
            }
            else
            {
                // 旋转回初始状态并设置mIsSee为true
                timer = 3; // 下一次检查的间隔时间
                ActionKit.Sequence()
                    .Callback(() =>  m_transform.DOMove(new Vector3(56, -40,-1), 0.5f).SetEase(Ease.InOutSine))
                    .Delay(0.2f)
                    .Callback(() => mIsSee = false)
                    .Start(this);
            }
        }
    }


    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
    }
}
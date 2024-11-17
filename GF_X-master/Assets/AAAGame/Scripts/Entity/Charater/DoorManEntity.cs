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
    private bool mIsSee = false;
    private bool mIsCanCilck = false;
    private bool misBeginNext = false;


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

    public bool IsOverSee
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

            if (shouldRotate && !misBeginNext)
            {
                timer = 10; // 下一次旋转的间隔时间
                // mIsSee为false
                ActionKit.Sequence()
                    .Callback(() => //x轴从初始位置0到-6停下,震动,增量移动
                         m_transform.DOMove(new Vector3(50, -40, -1), 0.1f).SetEase(Ease.InOutSine))
                     .Callback(() => mIsCanCilck = false)
                  //  m_transform.position = new Vector3(50, -40, -1))
                    .Delay(0.3f)
                    .Callback(() => m_transform.DOShakePosition(2f, new Vector3(5f, 5f, 0), 10, 90, false, true))
                    .Callback(() => mIsCanCilck = true)
                    .Delay(2f)
                    
                    .Callback(getOut)
                    .Start(this);
            }
        }
    }
    
    private void getOut()
    {
        if (!misBeginNext)
        {
            m_transform.DOMove(new Vector3(56, -40, -1), 0.5f).SetEase(Ease.InOutSine);
            mIsSee = false;
            mIsCanCilck = false;
            misBeginNext = false;
        }
    }


    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
        if (mIsCanCilck)
        {
            misBeginNext = true;
            mIsCanCilck = false;
            //停止m_transform.DOShakePosition
            m_transform.DOKill();
            ActionKit.Sequence()
                .Callback(() =>
                    m_transform.DOMove(new Vector3(-50, -6, -5), 0.5f).SetEase(Ease.InOutSine))
                .Delay(0.5f)
                .Callback(() => mIsSee = true)
                .Delay(UnityEngine.Random.Range(2, 4))
                .Callback(() => m_transform.DOShakePosition(1f, new Vector3(2f, 2f, 0), 10, 90, false, true))
                .Delay(1f)
                .Callback(() => misBeginNext = false)
                .Callback(getOut)
                .Start(this);
        }
    }
}
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

    private float timer = 5;
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
        //if (!Ctrlable) return;
        //上下间断性转动z180度
        timer -= realElapseSeconds;
        if (timer <= 3 && timer > 2)
        {
            m_transform.DOLocalRotate(new Vector3(0, 0, 0), 0.5f, RotateMode.Fast);
            mIsSee = true;
        }

        if (timer <= 0)
        {
            timer = 5;
            m_transform.DOLocalRotate(new Vector3(0, 0, 180), 0.5f, RotateMode.Fast);
            mIsSee = false;
        }
    }


    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
    }
}
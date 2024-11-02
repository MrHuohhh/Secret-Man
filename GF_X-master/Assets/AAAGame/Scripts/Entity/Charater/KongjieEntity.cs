using System;
using System.Collections.Generic;
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

    BoxCollider2D boxClick;
    private bool mCtrlable;

    public bool Ctrlable
    {
        get => mCtrlable;
        set
        {
            mCtrlable = value;
            // if (!IsAIPlayer) GF.StaticUI.JoystickEnable = mCtrlable;
        }
    }

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        boxClick = GetComponent<BoxCollider2D>();
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        if (!Ctrlable) return;
    }

    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
        
        
    }
}
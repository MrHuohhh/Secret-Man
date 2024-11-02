using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using QFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using Log = UnityGameFramework.Runtime.Log;

public class KongbuEntity : SampleEntity
{
    public virtual bool IsAIPlayer
    {
        get => false;
    }

    BoxCollider2D boxClick;
    private bool mCtrlable;
    
    private bool mNextLevel = false;
    
    private bool isDragging = false;
    
    public int BuValue = 0;
    
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

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        boxClick = GetComponent<BoxCollider2D>();
        m_transform = GetComponent<Transform>();
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        if (!isDragging && BuValue>0)
        {
            BuValue --;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu, new Dictionary<string, object>
            {
                ["value"] = BuValue,
            }));
        }
    }

    //按下
    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
        isDragging = true;
        if (!mNextLevel)
        {
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.RefreshTimer, new Dictionary<string, object>
            {
                ["Timer"] = 10,
            }));
        
            //掉物品
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.CreateKongjie, new Dictionary<string, Action>
            {
            }));
            mNextLevel = true;
        }
        else
        {
            //dotween旋转
            m_transform.DOLocalRotate(new Vector3(0, 0, 80), 1f, RotateMode.Fast );
            //旋转到固定角度
            if (!GF.Entity.HasEntity("Kongjie")) return;
            Entity KongjieEntity = GF.Entity.GetEntity("Kongjie");
            //KongjieEntity.GetComponent<KongjieEntity>().Ctrlable = true;
        } 
    }
    
    //持续
    void OnMouseDrag()
    {
        if (isDragging)
        {
            BuValue++;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu, new Dictionary<string, object>
            {
                ["value"] = BuValue,
            }));
            Debug.Log("Value increased: " + BuValue);
        }
    }

    void OnMouseUp()
    {
        //dotween旋转
        m_transform.DOLocalRotate(new Vector3(0, 0, 0), 1f, RotateMode.Fast );

        if (isDragging)
        {
            Debug.Log("Object released!");
            isDragging = false;
        }
    }
}
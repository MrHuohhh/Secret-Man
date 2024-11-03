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

    private BoxCollider2D boxClick;
    private bool mCtrlable;

    private bool mNextLevel = false;

    private bool isDragging = false;

    public int BuValue = 0;

    public int JieValue = 0;

    private bool mIsSee = false;

    private Entity KongjieEntity;

    private Transform m_transform;


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
        if (!isDragging && BuValue > 0)
        {
            BuValue--;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu,
                new Dictionary<string, object>
                {
                    ["value"] = BuValue,
                }));
        }

        if (!KongjieEntity)
        {
            if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/Kongjie.prefab"))
            {
            }
            else
            {
                KongjieEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/Kongjie.prefab");
            }
        }
        else
        {
            mIsSee = KongjieEntity.GetComponent<KongjieEntity>().IsWatching;
        }

        if ((!isDragging || !mIsSee) && JieValue > 0 )
        {
            JieValue--;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongjie,
                new Dictionary<string, object>
                {
                    ["value"] = JieValue,
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
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.RefreshTimer,
                new Dictionary<string, object>
                {
                    ["Timer"] = 10,
                }));

            //掉物品
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.CreateKongjie,
                new Dictionary<string, Action>
                {
                }));
            mNextLevel = true;
        }
        else
        {
            //dotween旋转
            m_transform.DOLocalRotate(new Vector3(0, 0, 80), 1f, RotateMode.Fast);
        }
    }

    //持续
    void OnMouseDrag()
    {
        if (isDragging)
        {
            BuValue++;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu,
                new Dictionary<string, object>
                {
                    ["value"] = BuValue,
                }));
            Debug.Log("Value increased: " + BuValue);

            if (!KongjieEntity)
            {
                if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/Kongjie.prefab")) return;
                KongjieEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/Kongjie.prefab");
            }

            ;
            mIsSee = KongjieEntity.GetComponent<KongjieEntity>().IsWatching;

            if (mIsSee)
            {
                JieValue++;
                GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongjie,
                    new Dictionary<string, object>
                    {
                        ["value"] = JieValue,
                    }));
            }
        }
    }

    void OnMouseUp()
    {
        //dotween旋转
        m_transform.DOLocalRotate(new Vector3(0, 0, 0), 1f, RotateMode.Fast);

        if (isDragging)
        {
            Debug.Log("Object released!");
            isDragging = false;
        }
    }
}
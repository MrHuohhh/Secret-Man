using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
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

    public float BuValue = 0;

    public int JieValue = 0;

    private bool mIsSee1 = false;
    private bool mIsOverSee = false;

    private Entity KongjieEntity;

    private Entity DoorManEntity;


    private Transform m_transform;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;
    private int Stage;

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
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        Stage = playerDm.LEVEL_STAGE;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        //阶段清空
        if (Stage != playerDm.LEVEL_STAGE)
        {
            BuValue = 0;
            Stage = playerDm.LEVEL_STAGE;
        }

        RefreshMouseOver();//不操作扣分和还原警戒值
    }

    //按下
    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
        isDragging = true;
       
        m_transform.DOLocalRotate(new Vector3(0, 0, 80), 1f, RotateMode.Fast);
        //}
        
        BuValue = BuValue + lvSettingTb[playerDm.LEVEL_STAGE].ClickScore;
        GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu,
            new Dictionary<string, object>
            {
                ["value"] = BuValue,
            }));

    }

    //持续
    void OnMouseDrag()
    {
        if (isDragging)
        {
            BuValue = BuValue + lvSettingTb[playerDm.LEVEL_STAGE].TapScore;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu,
                new Dictionary<string, object>
                {
                    ["value"] = BuValue,
                }));

            //老板的盯着
            if (!KongjieEntity)
            {
                if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/Kongjie.prefab")) return;
                KongjieEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/Kongjie.prefab");
            }

            ;
            bool mIsSee1 = KongjieEntity.GetComponent<KongjieEntity>().IsWatching;
            //门的盯着
            if (!DoorManEntity)
            {
                if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/DoorMan.prefab")) return;
                DoorManEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/DoorMan.prefab");
            }

            bool mIsOverSee = DoorManEntity.GetComponent<DoorManEntity>().IsOverSee;

            if (mIsSee1 && !mIsOverSee)
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

    //抬起
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
    
    //不操作扣分和还原警戒值
    private  void RefreshMouseOver()
    {
        if (!isDragging && BuValue > 0)
        {
            BuValue = BuValue - lvSettingTb[playerDm.LEVEL_STAGE].ScoreLost;
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
            mIsSee1 = KongjieEntity.GetComponent<KongjieEntity>().IsWatching;
        }

        if (!DoorManEntity)
        {
            if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/DoorMan.prefab"))
            {
            }
            else
            {
                DoorManEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/DoorMan.prefab");
            }
        }
        else
        {
            mIsOverSee = DoorManEntity.GetComponent<DoorManEntity>().IsOverSee;
        }

        if ((!isDragging || (!mIsSee1 || mIsOverSee)) && JieValue > 0)
        {
            JieValue = 0;
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongjie,
                new Dictionary<string, object>
                {
                    ["value"] = JieValue,
                }));
        }
    }
}
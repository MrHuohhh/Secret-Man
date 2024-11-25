using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
using GameFramework.Event;
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

    public int JieValue = 0;

    private bool mIsSee1 = false;
    private bool mIsOverSeeDoor = false;
    private bool mIsOverSeeWindow = false;


    private Entity KongjieEntity;

    private Entity DoorManEntity;

    private Entity WindowEntity;


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

        RefreshMouseOver(); //不操作扣分和还原警戒值
    }

    //按下
    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
        isDragging = true;

        m_transform.DOLocalRotate(new Vector3(0, 0, 80), 1f, RotateMode.Fast);
        //}

        GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu,
            new Dictionary<string, object>
            {
                ["value"] = lvSettingTb[playerDm.LEVEL_STAGE].ClickScore,
            }));
    }

    //持续
    void OnMouseDrag()
    {
        if (isDragging)
        {
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.DragBtnKongbu,
                new Dictionary<string, object>
                {
                    ["value"] = lvSettingTb[playerDm.LEVEL_STAGE].TapScore,
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

            mIsOverSeeDoor = DoorManEntity.GetComponent<DoorManEntity>().IsOverSeeDoor;

            //窗户
            if (!WindowEntity)
            {
                if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/WindowMan.prefab")) return;
                WindowEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/WindowMan.prefab");
            }

            mIsOverSeeWindow = WindowEntity.GetComponent<WindowEntity>().IsOverSeeWindow;

            if (mIsSee1 && !mIsOverSeeDoor && !mIsOverSeeWindow)
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
    private void RefreshMouseOver()
    {
        if (!isDragging)
        {
            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.LoseLove,
                new Dictionary<string, object>
                {
                    ["value"] = lvSettingTb[playerDm.LEVEL_STAGE].ScoreLost,
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
            mIsOverSeeDoor = DoorManEntity.GetComponent<DoorManEntity>().IsOverSeeDoor;
        }

        if (!WindowEntity)
        {
            if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/WindowMan.prefab"))
            {
            }
            else
            {
                WindowEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/WindowMan.prefab");
            }
        }
        else
        {
            mIsOverSeeWindow = WindowEntity.GetComponent<WindowEntity>().IsOverSeeWindow;
        }

        if ((!isDragging || (!mIsSee1 || mIsOverSeeDoor || mIsOverSeeWindow)) && JieValue > 0)
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
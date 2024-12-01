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

    private GameObject mBoy;

    private GameObject mGirl;
    
    private Animator mBoyAnimator;
    private Animator mGirlAnimator;

    private Transform m_transform;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;
    private int Stage;
    private string[] mSound;

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
        mBoy = GameObject.Find("boy");
        mGirl = GameObject.Find("girl");
        mBoyAnimator = mBoy.GetComponent<Animator>();
        mGirlAnimator = mGirl.GetComponent<Animator>();
        m_transform = GetComponent<Transform>();
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        Stage = playerDm.LEVEL_STAGE;
        mSound = new string[2] {"resources://Kiss_1", "resources://Kiss_2"};
        GF.Event.Subscribe(PlayerEventArgs.EventId, OnPlayerEvent);
    }

    private void OnPlayerEvent(object sender, GameEventArgs e)
    {
        var args = e as PlayerEventArgs;

        switch (args.EventType)
        {
            case PlayerEventType.HandsomeYes:
                mBoyAnimator.Play("BoyAry");
                mGirlAnimator.Play("GirlFowller");
                break;
        }
    }


    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        RefreshMouseOver(); //不操作扣分和还原警戒值
    }

    //按下
    void OnMouseDown()
    {
        isDragging = true;

        //m_transform.DOLocalRotate(new Vector3(0, 0, 80), 1f, RotateMode.Fast);
        mBoyAnimator.Play("BoyKiss");
        mGirlAnimator.Play("GirlShy");
        //随机播放AudioKit.PlaySound("resources://Enemy_Success");
        AudioKit.PlaySound(mSound[UnityEngine.Random.Range(0, mSound.Length)]);
        
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
            mBoyAnimator.Play("BoyKiss");
            mGirlAnimator.Play("GirlShy");
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
                KongjieEntity.GetComponent<KongjieEntity>().onFind();
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
        //m_transform.DOLocalRotate(new Vector3(0, 0, 0), 1f, RotateMode.Fast);
        mBoyAnimator.Play("Boy");
        if (isDragging)
        {
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
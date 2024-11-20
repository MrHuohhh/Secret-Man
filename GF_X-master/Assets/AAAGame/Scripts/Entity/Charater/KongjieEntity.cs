using System;
using System.Collections.Generic;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
using QFramework;
using UnityEngine;
using Log = UnityGameFramework.Runtime.Log;

public class KongjieEntity : SampleEntity
{
    private bool mIsSee = true; // 是否在警戒

    private float timer;
    Transform m_transform;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;


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
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Boss_OpenCD;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        timer -= realElapseSeconds;

        if (timer <= 0)
        {
            if (mIsSee)
            {
                EnterNoSeeState();
            }
            else
            {
                EnterCanSeeState();
            }
        }
    }

    //进入看不见状态
    private void EnterNoSeeState()
    {
        // 旋转180度并设置mIsSee为false
        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Boss_OpenCD; // 下一次旋转的间隔时间
        //进入看不见状态概率
        int OpenProp = lvSettingTb[playerDm.LEVEL_STAGE].Event_Boss_OpenProp; //0-100
        if (UnityEngine.Random.Range(0, 100) < OpenProp)
        {
            ActionKit.Sequence()
                .Callback(() => m_transform.DOLocalRotate(new Vector3(0, 0, 90), 0.5f, RotateMode.Fast))
                .Delay(0.2f)
                .Callback(() => mIsSee = false)
                .Start(this);
        }
    }
    
    //进入可见状态
    private void EnterCanSeeState()
    {
        // 旋转回初始状态并设置mIsSee为true
        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Boss_CloseCD;
        ; // 下一次检查的间隔时间
        int CloseProp = lvSettingTb[playerDm.LEVEL_STAGE].Event_Boss_CloseProp;
        if (UnityEngine.Random.Range(0, 100) < CloseProp)
        {
            ActionKit.Sequence()
                .Callback(() => m_transform.DOLocalRotate(new Vector3(0, 0, -90), 0.5f, RotateMode.Fast))
                .Delay(0.5f)
                .Callback(() => mIsSee = true)
                .Start(this);
        }
    }
    
    void OnMouseDown()
    {
        Debug.Log("Object clicked!");
    }
}
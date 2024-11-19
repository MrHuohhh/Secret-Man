using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
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

    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;

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
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_OpenCD;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        timer -= realElapseSeconds;

        if (timer <= 0)
        {
            timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_OpenCD;
            //进入看不见状态概率
            int OpenProp = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_OpenProp; //0-100

            var preTime = 2f;
            var preTimeArr = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_PreTime; // 二维浮点数数组
            // 解析数组
            var timeWeightPairs = preTimeArr.Select(pair => new { Timer = pair[0], Weight = pair[1] }).ToList();
            // 计算总权重
            float totalWeight = timeWeightPairs.Sum(pair => pair.Weight);
            // 生成随机数
            var random = UnityEngine.Random.Range(0, totalWeight);
            // 根据随机数选择时间
            float cumulativeWeight = 0f;
            foreach (var pair in timeWeightPairs)
            {
                cumulativeWeight += pair.Weight;
                if (random < cumulativeWeight)
                {
                    preTime = pair.Timer;
                    break;
                }
            }

            if (UnityEngine.Random.Range(0, 100) < OpenProp && !misBeginNext)
            {
                // mIsSee为false
                ActionKit.Sequence()
                    .Callback(() => //x轴从初始位置0到-6停下,震动,增量移动
                        m_transform.DOMove(new Vector3(50, -40, -1), 0.1f).SetEase(Ease.InOutSine))
                    .Callback(() => mIsCanCilck = false)
                    //  m_transform.position = new Vector3(50, -40, -1))
                    .Delay(0.3f)
                    .Callback(() => m_transform.DOShakePosition(2f, new Vector3(5f, 5f, 0), 10, 90, false, true))
                    .Callback(() => mIsCanCilck = true)
                    .Delay(preTime)
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
        //todo Event_Door_TypeProp类型1,正常/2.敌人
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
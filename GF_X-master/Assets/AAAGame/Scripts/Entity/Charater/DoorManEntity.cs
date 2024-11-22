using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
using QFramework;
using UnityEngine;
using UnityGameFramework.Runtime;
using Log = UnityGameFramework.Runtime.Log;

public class DoorManEntity : SampleEntity
{
    public virtual bool IsAIPlayer
    {
        get => false;
    }

    private bool mIsSee = false; //是否能挡住boss
    private bool mIsCanCilck = false;
    private bool misBeginNext = false;
    private bool mIsEnemy = false;
    private GameObject mStaff;
    private GameObject mDoor;
    private Entity HandsomeEntity;


    private float timer = 3;
    Transform m_transform;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private IDataTable<LevelTable> lvTb;
    private PlayerDataModel playerDm;

    public bool IsOverSee
    {
        get => mIsSee;
        set { mIsSee = value; }
    }


    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        mStaff = transform.Find("Staff").gameObject;
        mDoor = transform.Find("Door").gameObject;
        mStaff.SetActive(false);
        m_transform = GetComponent<Transform>();
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_OpenCD;
        lvTb = GF.DataTable.GetDataTable<LevelTable>();
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
            if (UnityEngine.Random.Range(0, 100) < OpenProp && !misBeginNext)
            {
                var timePre = preTimeGet();
                //todo 先关闭门
                // mIsSee为false
                ActionKit.Sequence()
                    .Callback(() => //x轴从初始位置0到-6停下,震动,增量移动
                        m_transform.DOMove(new Vector3(50, -40, -1), 0.1f).SetEase(Ease.InOutSine))
                    .Callback(() => mIsCanCilck = false)
                    //  m_transform.position = new Vector3(50, -40, -1))
                    .Delay(0.3f)
                    .Callback(() => m_transform.DOShakePosition(timePre, new Vector3(5,5,0)))
                    .Callback(() => mIsCanCilck = true)
                    .Delay(timePre)
                    .Callback(getOut)
                    .Start(this);
            }
        }
    }

    //加权计算本次随机的预告时间
    private float preTimeGet()
    {
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

        return preTime;
    }

    private void getOut()
    {
        if (!misBeginNext)
        {
            mStaff.transform.DOMove(new Vector3(56, -40, -1), 0.5f).SetEase(Ease.InOutSine).SetEase(Ease.InOutSine)
                .OnComplete(() => { mStaff.SetActive(false); });
            mIsSee = false;
            mIsCanCilck = false;
            misBeginNext = false;
        }
    }


    void OnMouseDown()
    {
        OnMouseDownDoor();
    }

    //点击mDoorClick
    private void OnMouseDownDoor()
    {
        int TypeProp = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_TypeProp;
        misBeginNext = true;
        if (mIsCanCilck)
        {
            mIsCanCilck = false;
            //停止m_transform.DOShakePosition
            var lvRow = lvTb.GetDataRow(playerDm.GAME_LEVEL);
            m_transform.DOKill(); //todo 开门
            if (UnityEngine.Random.Range(0, 100) < TypeProp) //Event_Door_TypeProp类型1,正常/2.敌人
            {
                mStaff.SetActive(true);
                //普通员工
                ActionKit.Sequence()
                    .Callback(() =>
                        mStaff.transform.DOMove(new Vector3(-50, -6, -5), 0.5f).SetEase(Ease.InOutSine))
                    .Delay(0.5f)
                    .Callback(() => mIsSee = true)
                    .Delay(UnityEngine.Random.Range(2, 4))
                    .Callback(() =>
                        mStaff.transform.DOShakePosition(lvRow.GlobNum[2],new Vector3(5,5,0)))
                    .Delay(lvRow.GlobNum[2])
                    .Callback(() => misBeginNext = false)
                    .Callback(getOut)
                    .Start(this);
            }
            else
            {
                //帅锅
                if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/Handsome.prefab")) return;
                HandsomeEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/Handsome.prefab");
                //初始化和显示
                HandsomeEntity.GetComponent<HandsomeEntity>().onStartAtt();
                ActionKit.Sequence()
                    .Delay(lvSettingTb[playerDm.LEVEL_STAGE].EnemyMoveTime)
                    .Callback(() => misBeginNext = false)
                    .Start(this);
            }
        }
        else
        {
            mIsCanCilck = false;
            //todo 开空门
            ActionKit.Sequence()
                .Callback(() =>  m_transform.DOShakePosition(1f,new Vector3(2,2,0)))
                .Delay(1f)
                .Callback(() => misBeginNext = false)
                .Start(this);
        }
    }
}
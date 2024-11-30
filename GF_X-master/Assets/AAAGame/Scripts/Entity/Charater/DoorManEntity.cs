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
    private BoxCollider2D m_Collider2D;
    private Animator mDoorAnimator;
    private Animator mStaffAnimator;

    private float timer = 3;
    private Transform m_transform;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private IDataTable<LevelTable> lvTb;
    private PlayerDataModel playerDm;

    public bool IsOverSeeDoor
    {
        get => mIsSee;
        set { mIsSee = value; }
    }


    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        mStaff = transform.Find("Staff").gameObject;
        mDoor = transform.Find("Door").gameObject;
        //mStaffAin在Staff/StaffAin
        GameObject mStaffAin = mStaff.transform.Find("StaffAni").gameObject;
        mStaff.SetActive(false);
        m_Collider2D = GetComponent<BoxCollider2D>();
        m_transform = mDoor.GetComponent<Transform>();
        mDoorAnimator = mDoor.GetComponent<Animator>();
        mStaffAnimator = mStaffAin.GetComponent<Animator>();
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
                m_Collider2D.enabled = true;
                var timePre = preTimeGet();
                //先播放关闭门
                mDoorAnimator.Play("default");
                // mIsSee为false
                AudioKit.PlaySound("resources://Door_Knock");
                ActionKit.Sequence()
                    .Callback(() => mIsCanCilck = false)
                    .Callback(() => m_transform.DOShakePosition(timePre, new Vector3(1, 0, 0)))
                    .Callback(() => mIsCanCilck = true)
                    .Delay(timePre)
                    .Callback(() => m_transform.localPosition = Vector3.zero)
                    .Callback(getOut)
                    .Start(this);
            }
        }
    }

    //加权计算本身随机的持续时间
    private float timeGet()
    {
        var preTime = 2f;
        var preTimeArr = lvSettingTb[playerDm.LEVEL_STAGE].Event_Door_Time; // 二维浮点数数组
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
            //y旋转180
            mStaffAnimator.Play("mishu");
            mStaff.transform.rotation = Quaternion.Euler(0, 180, 0);
            mStaff.transform.DOLocalMove(new Vector3(0,0 , -1), 0.5f).SetEase(Ease.InOutSine).SetEase(Ease.InOutSine)
                .OnComplete(() =>
                {
                    mStaff.SetActive(false);
                });
            mIsSee = false;
            mIsCanCilck = false;
            misBeginNext = false;
            timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Windows_OpenCD;
            m_Collider2D.enabled = true;
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
            m_Collider2D.enabled = false;
            mIsCanCilck = false;
            //停止m_transform.DOShakePosition
            var lvRow = lvTb.GetDataRow(playerDm.GAME_LEVEL);
            m_transform.DOKill(); //开门
            m_transform.localPosition = Vector3.zero;
            mDoorAnimator.Play("door");
            AudioKit.PlaySound("resources://Door_Open");
            if (UnityEngine.Random.Range(0, 100) < TypeProp) //Event_Door_TypeProp类型1,正常/2.敌人
            {
                mStaff.SetActive(true);
                mStaff.transform.rotation = Quaternion.Euler(0, 0, 0);
                //普通员工
                ActionKit.Sequence()
                    .Callback(() =>
                        mStaff.transform.DOLocalMove(new Vector3(-80, 0, 0), 0.5f).SetEase(Ease.InOutSine))
                    .Delay(0.5f)
                    .Callback(() => mIsSee = true)
                    .Callback(() => mStaffAnimator.Play("mishuBoss"))
                    .Delay(timeGet())
                    .Callback(() =>
                        mStaff.transform.DOShakePosition(lvRow.GlobNum[1], new Vector3(1, 1, 0)))
                    .Delay(lvRow.GlobNum[1])
                    .Callback(() => misBeginNext = false)
                    .Callback(() =>  AudioKit.PlaySound("resources://Door_Close"))
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
                    .Callback(() =>  AudioKit.PlaySound("resources://Door_Close"))
                    .Callback(() => misBeginNext = false)
                    .Start(this);
            }
        }
        else
        {
            mIsCanCilck = false;
            m_transform.localPosition = Vector3.zero;
            // 开空门
            ActionKit.Sequence()
                .Callback(() => mDoorAnimator.Play("door"))
                .Delay(1f)
                .Callback(() => m_transform.localPosition = Vector3.zero)
                .Callback(() => misBeginNext = false)
                .Callback(() => mDoorAnimator.Play("default"))
                .Callback(() =>  AudioKit.PlaySound("resources://Door_Close"))
                .Start(this);
        }
    }
}
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

public class WindowEntity : SampleEntity
{
    public virtual bool IsAIPlayer
    {
        get => false;
    }

    private bool mIsSee = false; //是否能挡住boss
    private bool mIsCanCilck = false;
    private bool misBeginNext = false;
    private bool mIsEnemy = false;
    private GameObject mWindow;
    private GameObject mStaff;
    private GameObject mShadow;
    private Entity HandsomeEntity;
    private BoxCollider2D m_Collider2D;


    private float timer = 3;
    private Transform mShadowTrans;
    private Transform mWindowTrans;
    private Transform mStaffTrans;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private IDataTable<LevelTable> lvTb;
    private PlayerDataModel playerDm;

    public bool IsOverSeeWindow
    {
        get => mIsSee;
        set { mIsSee = value; }
    }


    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        mStaff = transform.Find("Staff").gameObject;
        mWindow = transform.Find("Window").gameObject;
        mShadow = transform.Find("Shadow").gameObject;
        mStaff.SetActive(false);
        mShadowTrans = mShadow.GetComponent<Transform>();
        mWindowTrans = mWindow.GetComponent<Transform>();
        mStaffTrans = mStaff.GetComponent<Transform>();
        m_Collider2D = GetComponent<BoxCollider2D>();
        m_Collider2D.enabled = true;
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Windows_OpenCD;
        lvTb = GF.DataTable.GetDataTable<LevelTable>();
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);

        timer -= realElapseSeconds;

        if (timer <= 0)
        {
            timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Windows_OpenCD;
            //进入看不见状态概率
            int OpenProp = lvSettingTb[playerDm.LEVEL_STAGE].Event_Windows_OpenProp; //0-100
            if (UnityEngine.Random.Range(0, 100) < OpenProp && !misBeginNext)
            {
                var timePre = preTimeGet();
                // 先关闭窗
                DOTween.Pause(mWindowTrans);
                DOTween.Pause(mShadowTrans);
                mWindowTrans.localPosition = new Vector3(0, 0, 0);
                mShadow.SetActive(true);
                mStaff.SetActive(false);
                // mIsSee为false
                ActionKit.Sequence()
                    .Callback(() => mIsCanCilck = true)
                    .Callback(() => //x轴从初始位置停下
                        mShadowTrans.DOLocalMove(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine))
                    //  mShadowTrans.position = new Vector3(50, -40, -1))
                    .Delay(0.5f)
                    //.Callback(() => mIsCanCilck = true)
                    .Delay(timePre)
                    .Callback(getOut)
                    .Start(this);
            }
        }
    }

    //加权计算本身随机的持续时间
    private float timeGet()
    {
        var preTime = 2f;
        var preTimeArr = lvSettingTb[playerDm.LEVEL_STAGE].Event_Window_Time; // 二维浮点数数组
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
        var preTimeArr = lvSettingTb[playerDm.LEVEL_STAGE].Event_Window_PreTime; // 二维浮点数数组
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
            DOTween.Pause(mShadowTrans);
            mWindowTrans.DOLocalMove(new Vector3(0, 0, 0), 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                mStaff.SetActive(false);
                mShadow.SetActive(true);
                mShadowTrans.DOLocalMove(new Vector3(-60, 0, 0), 1f).SetEase(Ease.InOutSine).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        mShadowTrans.localPosition = new Vector3(22, 0, 0);
                        m_Collider2D.enabled = true;
                        mIsSee = false;
                        mIsCanCilck = false;
                        misBeginNext = false;
                        timer = lvSettingTb[playerDm.LEVEL_STAGE].Event_Windows_OpenCD;
                    });
            });
        }
    }


    void OnMouseDown()
    {
        OnMouseDownDoor();
    }

    //点击mDoorClick
    private void OnMouseDownDoor()
    {
        int TypeProp = lvSettingTb[playerDm.LEVEL_STAGE].Event_Window_TypeProp;
        misBeginNext = true;
        if (mIsCanCilck)
        {
            m_Collider2D.enabled = false;
            mIsCanCilck = false;
            //停止mShadowTrans.DOShakePosition
            var lvRow = lvTb.GetDataRow(playerDm.GAME_LEVEL);
            mShadowTrans.DOKill(); //todo 开门
            if (UnityEngine.Random.Range(0, 100) < TypeProp) //Event_Window_TypeProp类型1,正常/2.敌人
            {
                mShadow.SetActive(false);
                mStaff.SetActive(true);
                //普通员工
                ActionKit.Sequence()
                    .Callback(() => mWindowTrans.DOLocalMove(new Vector3(0, 36, 0), 0.5f).SetEase(Ease.InOutSine))
                    .Delay(0.5f)
                    .Callback(() => mIsSee = true)
                    .Delay(timeGet())
                    .Callback(() =>
                        mStaffTrans.DOShakePosition(lvRow.GlobNum[2], new Vector3(5, 5, 0)))
                    .Delay(lvRow.GlobNum[2])
                    .Callback(() => misBeginNext = false)
                    .Callback(getOut)
                    .Start(this);
                mShadowTrans.localPosition = new Vector3(-21, 0, 0);
            }
            else
            {
                mShadow.SetActive(false);
                //帅锅
                if (!GF.Entity.HasEntity("Assets/AAAGame/Prefabs/Entity/HandsomeWindow.prefab")) return;
                HandsomeEntity = GF.Entity.GetEntity("Assets/AAAGame/Prefabs/Entity/HandsomeWindow.prefab");
                //初始化和显示
                HandsomeEntity.GetComponent<HandsomeWinEntity>().onStartAtt();
                ActionKit.Sequence()
                    .Callback(() => mWindowTrans.DOLocalMove(new Vector3(0, 36, 0), 0.5f).SetEase(Ease.InOutSine))
                    .Delay(0.5f)
                    .Delay(lvSettingTb[playerDm.LEVEL_STAGE].EnemyMoveTime)
                    .Callback(() => misBeginNext = false)
                    .Callback(getOut)
                    .Callback(() => HandsomeEntity.GetComponent<HandsomeWinEntity>().Out())
                    .Start(this);
            }
        }
        else
        {
            mIsCanCilck = false;
            //暂停spine
            DOTween.Pause(mWindowTrans);
            mWindowTrans.localPosition = new Vector3(0, 0, 0);
            // 开空窗
            ActionKit.Sequence()
                .Callback(() => mWindowTrans.DOLocalMove(new Vector3(0, 36, 0), 0.5f).SetEase(Ease.InOutSine))
                .Delay(1f)
                .Callback(() => mWindowTrans.DOLocalMove(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.InOutSine))
                .Callback(() => misBeginNext = false)
                .Start(this);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
using QFramework;
using UnityEngine;
using Log = UnityGameFramework.Runtime.Log;

public class HandsomeEntity : SampleEntity
{
    public virtual bool IsAIPlayer
    {
        get => false;
    }

    private bool mIsStart = false; //是否开始
    private bool mIsCanCilck = false;
    private bool misDie = false;
    private int mHealth = 0;
    private GameObject mHanson;
    private Vector3 mStartPos;
    private BoxCollider2D mCollider;

    private float timer = 3;
    Transform m_transform;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        mHanson = transform.Find("Hanson").gameObject;
        m_transform = GetComponent<Transform>();
        mCollider = GetComponent<BoxCollider2D>();
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        
        mHanson.SetActive(false);
        mStartPos = new Vector3(65, -31, -1);
        mIsStart = false;
        mCollider.enabled = false;
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
    }

    public void onStartAtt()
    {
        mCollider.enabled = true;
        misDie = false;
        mIsStart = true;
        mHanson.SetActive(true);
        mIsCanCilck = true;
        m_transform.position = mStartPos;
        timer = lvSettingTb[playerDm.LEVEL_STAGE].EnemyMoveTime;
        mHealth = lvSettingTb[playerDm.LEVEL_STAGE].EnemyLives;
        ActionKit.Sequence()
            .Callback(() =>
                m_transform.DOMove(new Vector3(-6, -11, -1), timer).SetEase(Ease.InOutSine).SetEase(Ease.InOutSine))
            .Delay(timer)
            .Callback(Finish)
            .Delay(timer) //todo 动画时间?
            .Callback(() => m_transform.position = mStartPos) //设置位置回到初始位置mStartPos
            .Start(this);
    }

    private void Finish()
    {
        //暂停动画
        DOTween.Pause(m_transform);
        if (!misDie)
        {
            mIsCanCilck = false;
            mIsStart = false;
            mHanson.SetActive(false);
            mCollider.enabled = false;


            //todo 播放动画

            GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(PlayerEventType.LoseLove,
                new Dictionary<string, object>
                {
                    ["value"] = lvSettingTb[playerDm.LEVEL_STAGE].Damage,
                }));
        }
    }


    void OnMouseDown()
    {
        OnMouseDownHanson();
    }

    private void OnMouseDownHanson()
    {
        Log.Error("点击帅锅");
        if (mIsCanCilck && mIsStart)
        {
            //受击抖动
            m_transform.DOShakePosition(0.1f, Vector3.one * 0.05f, 10, 90, false, true).SetEase(Ease.OutQuad);
            mHealth -= 1;
            if (mHealth <= 0)
            {
                misDie = true;
                //todo 动画
                //暂停动画
                DOTween.Pause(m_transform);
                mHanson.SetActive(false);
                m_transform.position = mStartPos;
                mCollider.enabled = false;
            }
        }
    }
}
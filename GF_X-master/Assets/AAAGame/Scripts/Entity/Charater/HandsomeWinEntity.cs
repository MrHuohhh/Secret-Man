using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameFramework;
using GameFramework.DataTable;
using QFramework;
using UnityEngine;
using Log = UnityGameFramework.Runtime.Log;

public class HandsomeWinEntity : SampleEntity
{
    public virtual bool IsAIPlayer
    {
        get => false;
    }

    private bool mIsStart = false; //是否开始
    private bool mIsCanCilck = false;
    private bool misDie = false;
    private int mHealth = 0;
    private GameObject Love;
    private GameObject Hanson;

    private BoxCollider2D mCollider;

    private float timer = 3;
    private Transform m_transform;
    private Transform mLove;

    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;
    private string[] mSound;


    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        Love = transform.Find("Love").gameObject;
        Hanson = transform.Find("Hanson").gameObject;
        m_transform = GetComponent<Transform>();
        mLove = Love.transform;
        mCollider = GetComponent<BoxCollider2D>();
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();

        Hanson.SetActive(false);
        Love.SetActive(false);
        mIsStart = false;
        mCollider.enabled = false;
        mSound = new string[4] {"resources://Attack_1", "resources://Attack_2", "resources://Attack_3", "resources://Attack_4"};
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
        Hanson.SetActive(true);
        Love.SetActive(true);

        Love.SetActive(true);
        mIsCanCilck = true;
        mLove.localPosition =  Vector3.zero;
        timer = lvSettingTb[playerDm.LEVEL_STAGE].EnemyMoveTime;
        mHealth = lvSettingTb[playerDm.LEVEL_STAGE].EnemyLives;
        ActionKit.Sequence()
            .Callback(() =>
                mLove.DOLocalMove(new Vector3(25, -9, -20), timer).SetEase(Ease.InOutSine).SetEase(Ease.InOutSine))
            .Delay(timer)
            .Callback(Finish)
            .Delay(timer) //todo 动画时间?
            .Callback(() => mLove.localPosition =  Vector3.zero) //设置位置回到初始位置 Vector3.zero
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
            //m_transform.gameObject.SetActive(false);
            Love.SetActive(false);
            mCollider.enabled = false;


            //todo 播放动画
            AudioKit.PlaySound("resources://Enemy_Success");
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
        if (mIsCanCilck && mIsStart)
        {
            //受击抖动
            m_transform.DOShakePosition(0.1f, Vector3.one * 0.05f, 10, 90, false, true).SetEase(Ease.OutQuad);
            mHealth -= 1;
            AudioKit.PlaySound(mSound[UnityEngine.Random.Range(0, mSound.Length)]);
            if (mHealth <= 0)
            {
                misDie = true;
                //todo 挨打动画
                //暂停动画
                DOTween.Pause(m_transform);
                //m_transform.gameObject.SetActive(false);
                Love.SetActive(false);
                mLove.localPosition =  Vector3.zero;
                mCollider.enabled = false;
            }
        }
    }

    public void Out()
    {
        Hanson.SetActive(false);
        Love.SetActive(false);
    }
}
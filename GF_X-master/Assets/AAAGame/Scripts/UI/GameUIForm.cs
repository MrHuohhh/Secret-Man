using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using GameFramework.Event;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using QFramework;
using UnityGameFramework.Runtime;
using TMPro;
using UnityEngine.U2D;

public partial class GameUIForm : UIFormBase
{
    private float LvTimer = 0;
    private float LvTimerV = 0;
    private float Kongjie = 0;
    private float kongbu = 0;
    private bool isDragging1 = true;
    private bool isDragging2 = true;
    private bool boom1 = false;
    private bool boom2 = false;
    private bool isCd = false;

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        GF.Event.Subscribe(PlayerEventArgs.EventId, OnPlayerEvent);


        RefreshCoinsText();
        var lvTb = GF.DataTable.GetDataTable<LevelTable>();
        var playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        var lvId = playerDm.GAME_LEVEL;
        LvTimer = lvTb[lvId].LvTimer;
        LvTimerV = LvTimer;
        varTimeBar.fillAmount = LvTimer;

        varKongjieBar.fillAmount = 0;
        varNodProcess.gameObject.SetActive(false);
    }

    private void RefreshCoinsText()
    {
        var playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        coinNumText.text = playerDm.Coins.ToString();
    }

    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        if (LvTimerV >= 0)
        {
            varTimeBar.fillAmount = LvTimerV / LvTimer;
            //LvTimerV -= realElapseSeconds;
            if (Kongjie > 0)
            {
                Kongjie = 0;
                var curProcedure = GF.Procedure.CurrentProcedure;
                if (curProcedure is GameProcedure)
                {
                    var gameProcedure = curProcedure as GameProcedure;
                    gameProcedure.OnGameOver(false);
                }
            }

            if (kongbu > 1)
            {
                // var curProcedure = GF.Procedure.CurrentProcedure;
                // if (curProcedure is GameProcedure)
                // {
                //     var gameProcedure = curProcedure as GameProcedure;
                //     gameProcedure.OnGameOver(true);
                // }
                // LvTimerV = -1;
                // GF.UI.OpenUIForm(UIViews.Lv2d1UIForm);
            }
        }


        #region //电话

        UpdatePhone();
        //检测旋转varNodboom1.gameObject.transform.rotation.z是否在0-10，170-190，350-360之间
        if (
            varNodboom1.gameObject.transform.eulerAngles.z >= 0 &&
            varNodboom1.gameObject.transform.eulerAngles.z <= 10 ||
            varNodboom1.gameObject.transform.eulerAngles.z >= 170 &&
            varNodboom1.gameObject.transform.eulerAngles.z <= 190 ||
            varNodboom1.gameObject.transform.eulerAngles.z >= 350 &&
            varNodboom1.gameObject.transform.eulerAngles.z <= 360)
        {
            boom1 = true;
            varNodboom1.color = Color.yellow;
        }
        else
        {
            boom1 = false;
            varNodboom1.color = Color.blue;
        }

        if (
            varNodboom2.gameObject.transform.eulerAngles.z >= 0 &&
            varNodboom2.gameObject.transform.eulerAngles.z <= 10 ||
            varNodboom2.gameObject.transform.eulerAngles.z >= 170 &&
            varNodboom2.gameObject.transform.eulerAngles.z <= 190 ||
            varNodboom2.gameObject.transform.eulerAngles.z >= 350 &&
            varNodboom2.gameObject.transform.eulerAngles.z <= 360)
        {
            boom2 = true;
            varNodboom2.color = Color.yellow;
        }
        else
        {
            boom2 = false;
            varNodboom2.color = Color.blue;
        }


        //开启下一关
        if (boom1 && boom2 && !isDragging1 && !isDragging2 && !isCd)
        {
            ActionKit.Sequence()
                .Callback(() => GF.Event.Fire(this, ReferencePool.Acquire<PlayerEventArgs>().Fill(
                    PlayerEventType.PhoneCall,
                    null)))
                .Callback(() => isCd = true)
                .Delay(6f)
                .Callback(PhoneCall)
                .Start(this);
        }

        #endregion
    }

    private void UpdatePhone()
    {
        if (isDragging1)
        {
            //增量旋转
            varNodboom1.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 1), 0.1f, RotateMode.LocalAxisAdd);
            if (varNodboom1.gameObject.transform.localRotation.z >= 360)
            {
                varNodboom1.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }

        if (isDragging2)
        {
            //增量旋转
            varNodboom2.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 1), 0.1f, RotateMode.LocalAxisAdd);
            if (varNodboom2.gameObject.transform.localRotation.z >= 360)
            {
                varNodboom2.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    //打电话
    private void PhoneCall()
    {
        //随机角度
        isCd = false;
        boom1 = false;
        boom2 = false;
        isDragging1 = true;
        isDragging2 = true;
        varNodboom1.gameObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
        varNodboom2.gameObject.transform.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(0, 360));
    }

    private void OnPlayerEvent(object sender, GameEventArgs e)
    {
        var args = e as PlayerEventArgs;

        switch (args.EventType)
        {
            case PlayerEventType.RefreshTimer:
                // 重置时间    
                var data = args.EventData as Dictionary<string, object>;
                if (data != null && data.ContainsKey("Timer"))
                {
                    LvTimerV = (int)data["Timer"];
                }

                break;
            case PlayerEventType.DragBtnKongbu:
                var data2 = args.EventData as Dictionary<string, object>;
                if (data2 != null && data2.ContainsKey("value"))
                {
                    varNodProcess.gameObject.SetActive(true);
                    varKongjieBar.fillAmount = (float)data2["value"] / 2000;
                    kongbu = (float)data2["value"];
                    //去掉小数
                    coinNumText.text = Mathf.Floor(kongbu).ToString();
                }

                break;
            case PlayerEventType.DragBtnKongjie:
                var data3 = args.EventData as Dictionary<string, object>;
                if (data3 != null && data3.ContainsKey("value"))
                {
                    //varKongjieBar.fillAmount = (float)(int)data3["value"] / 100;
                    Kongjie = (float)(int)data3["value"];
                }

                break;

            //varNodProcess
        }
    }

    protected override void OnButtonClick(object sender, string btId)
    {
        base.OnButtonClick(sender, btId);
        if (isCd)
        {
            return;
        }
        switch (btId)
        {
            case "boom1Btn":
                if (!isDragging1)
                {
                    isDragging1 = true;
                }
                else
                {
                    isDragging1 = false;
                }

                break;
            case "boom2Btn":
                if (!isDragging2)
                {
                    isDragging2 = true;
                }
                else
                {
                    isDragging2 = false;
                }

                break;
        }
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
        GF.Event.Unsubscribe(PlayerEventArgs.EventId, OnPlayerEvent);
    }
}
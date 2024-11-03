using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using GameFramework.Event;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using UnityGameFramework.Runtime;
using TMPro;
using UnityEngine.U2D;

public partial class GameUIForm : UIFormBase
{
    private float LvTimer = 0;
    private float LvTimerV = 0;
    private float Kongjie = 0;
    private float kongbu = 0;

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
            LvTimerV -= realElapseSeconds;
            if (LvTimerV < 0 || Kongjie == 1)
            {
                LvTimerV = -1;
                var curProcedure = GF.Procedure.CurrentProcedure;
                if (curProcedure is GameProcedure)
                {
                    var gameProcedure = curProcedure as GameProcedure;
                    gameProcedure.OnGameOver(false);
                }
            }
            if (kongbu == 1)
            {
                // var curProcedure = GF.Procedure.CurrentProcedure;
                // if (curProcedure is GameProcedure)
                // {
                //     var gameProcedure = curProcedure as GameProcedure;
                //     gameProcedure.OnGameOver(true);
                // }
                LvTimerV = -1;
                GF.UI.OpenUIForm(UIViews.Lv2d1UIForm);
            }
        }
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
                    varProcessBar.fillAmount = (float)(int)data2["value"] / 200;
                    kongbu = (float)(int)data2["value"] / 200;
                }

                break;
            case PlayerEventType.DragBtnKongjie:
                var data3 = args.EventData as Dictionary<string, object>;
                if (data3 != null && data3.ContainsKey("value"))
                {
                    varKongjieBar.fillAmount = (float)(int)data3["value"] / 100;
                    Kongjie = (float)(int)data3["value"] / 100;

                }

                break;

            //varNodProcess
        }
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
        GF.Event.Unsubscribe(PlayerEventArgs.EventId, OnPlayerEvent);
    }
}
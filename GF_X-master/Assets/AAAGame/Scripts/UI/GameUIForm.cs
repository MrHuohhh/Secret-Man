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
            if (LvTimerV < 0)
            {
                LvTimerV = -1;
                var curProcedure = GF.Procedure.CurrentProcedure;
                if (curProcedure is GameProcedure)
                {
                    var gameProcedure = curProcedure as GameProcedure;
                    gameProcedure.OnGameOver(false);
                }
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
                    varProcessBar.fillAmount =(float)(int)data2["value"] / 500;
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
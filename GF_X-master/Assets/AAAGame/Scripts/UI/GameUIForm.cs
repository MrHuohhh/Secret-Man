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
        RefreshCoinsText();
        var lvTb = GF.DataTable.GetDataTable<LevelTable>();
        var playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        var lvId = playerDm.GAME_LEVEL;
        LvTimer = lvTb[lvId].LvTimer;
        LvTimerV = LvTimer;
        varTimeBar.fillAmount = LvTimer;
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

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
    }
}
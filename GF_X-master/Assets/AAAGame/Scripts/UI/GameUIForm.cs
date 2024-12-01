using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameFramework;
using GameFramework.Event;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using GameFramework.DataTable;
using QFramework;
using UnityGameFramework.Runtime;
using TMPro;
using UnityEngine.U2D;

public partial class GameUIForm : UIFormBase
{
    private float LvTimer = 0;
    private float LvTimerV = 0;
    private float bossNum = 0;
    private float loveNum = 0;
    private bool isDragging1 = true;
    private bool isDragging2 = true;
    private bool boom1 = false;
    private bool boom2 = false;
    private bool isCd = false;
    private IDataTable<Level1SettingTable> lvSettingTb;
    private PlayerDataModel playerDm;

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        GF.Event.Subscribe(PlayerEventArgs.EventId, OnPlayerEvent);


        RefreshCoinsText();
        var lvTb = GF.DataTable.GetDataTable<LevelTable>();
        playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        var lvId = playerDm.GAME_LEVEL;
        LvTimer = lvTb[lvId].LvTimer;
        LvTimerV = LvTimer;
        varTimeBar.fillAmount = LvTimer;

        varKongjieBar.fillAmount = 0;
        varNodProcess.gameObject.SetActive(false);

        lvSettingTb = GF.DataTable.GetDataTable<Level1SettingTable>();
        varLoveStage.text = playerDm.LEVEL_STAGE.ToString();
        //GameObject[5] varLoveArr根据playerDm.LEVEL_STAGE显示
        for (int i = 0; i < varLoveArr.Length; i++)
        {
            if (i < playerDm.LEVEL_STAGE-1)
            {
                varLoveArr[i].gameObject.SetActive(true);
            }
            else
            {
                varLoveArr[i].gameObject.SetActive(false);
            }
        }
        
        loveNum = 0;
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
            if (bossNum > 0)
            {
                bossNum = 0;
                var curProcedure = GF.Procedure.CurrentProcedure;
                if (curProcedure is GameProcedure)
                {
                    var gameProcedure = curProcedure as GameProcedure;
                    gameProcedure.OnGameOver(false);
                }
            }

            if (loveNum >= lvSettingTb[playerDm.LEVEL_STAGE].Target)
            {
                playerDm.LEVEL_STAGE++;
                varLoveStage.text = playerDm.LEVEL_STAGE.ToString();
                for (int i = 0; i < varLoveArr.Length; i++)
                {
                    if (i < playerDm.LEVEL_STAGE-1)
                    {
                        varLoveArr[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        varLoveArr[i].gameObject.SetActive(false);
                    }
                }
                loveNum = 0;
                // 判断是否完成关卡
                if (playerDm.LEVEL_STAGE >= 6)
                {
                    var curProcedure = GF.Procedure.CurrentProcedure;
                    if (curProcedure is GameProcedure)
                    {
                        var gameProcedure = curProcedure as GameProcedure;
                        gameProcedure.OnGameOver(true);
                    }
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
                // var data = args.EventData as Dictionary<string, object>;
                // if (data != null && data.ContainsKey("Timer"))
                // {
                //     LvTimerV = (int)data["Timer"];
                // }
                break;
            case PlayerEventType.DragBtnKongbu:
                var data2 = args.EventData as Dictionary<string, object>;
                if (data2 != null && data2.ContainsKey("value"))
                {
                    varNodProcess.gameObject.SetActive(true);
                    var stageScore = lvSettingTb[playerDm.LEVEL_STAGE].Target;
                    loveNum += (float)(int)data2["value"];
                    varKongjieBar.fillAmount = loveNum / stageScore;
                    //去掉小数
                    coinNumText.text = Mathf.Floor(loveNum).ToString();
                }

                break;
            case PlayerEventType.LoseLove:
                var data3 = args.EventData as Dictionary<string, object>;
                if (data3 != null && data3.ContainsKey("value"))
                {
                    varNodProcess.gameObject.SetActive(true);
                    var stageScore = lvSettingTb[playerDm.LEVEL_STAGE].Target;
                    if (loveNum > 0)
                    {
                        loveNum -= (float)(int)data3["value"];
                        if (loveNum < 0)
                        {
                            loveNum = 0;
                        }
                    }

                    varKongjieBar.fillAmount = loveNum / stageScore;
                    //去掉小数
                    coinNumText.text = Mathf.Floor(loveNum).ToString();
                }

                break;
            case PlayerEventType.DragBtnKongjie:
                var data4 = args.EventData as Dictionary<string, object>;
                if (data4 != null && data4.ContainsKey("value"))
                {
                    //varKongjieBar.fillAmount = (float)(int)data3["value"] / 100;
                    bossNum = (float)(int)data4["value"];
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
        }
    }

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
        GF.Event.Unsubscribe(PlayerEventArgs.EventId, OnPlayerEvent);
    }
}
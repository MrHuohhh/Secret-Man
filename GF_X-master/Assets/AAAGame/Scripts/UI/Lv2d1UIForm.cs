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
using TMPro;
using UnityEngine.U2D;
using Log = UnityGameFramework.Runtime.Log;

public partial class Lv2d1UIForm : UIFormBase, IPointerDownHandler, IPointerUpHandler
{
    private float LvTimer = 0;
    private float LvTimerV = 0;
    private bool isDragging1 = false;
    private bool isDragging2 = false;
    private bool isDragging3 = false;
    private bool boom1 = false;
    private bool boom2 = false;
    private bool boom3 = false;

    private bool lv3Start = false;
    private float speed = 500f; // 移动速度
    private float range = 500f; // 移动范围
    private bool isStop = false;
    private int lv3Times = 0;

    protected override void OnOpen(object userData)
    {
        base.OnOpen(userData);
        GF.Event.Subscribe(PlayerEventArgs.EventId, OnPlayerEvent);


        var lvTb = GF.DataTable.GetDataTable<LevelTable>();
        var playerDm = GF.DataModel.GetOrCreate<PlayerDataModel>();
        var lvId = playerDm.GAME_LEVEL;
        LvTimer = lvTb[lvId].LvTimer;
        LvTimerV = LvTimer;
        varTimeBar.fillAmount = 1;
        // varKongjieBar.fillAmount = 0;
        varNodAttack.gameObject.SetActive(true);
        varBtnCilck.gameObject.SetActive(true);
        varNodBoom.gameObject.SetActive(false);
        varNodEnd.gameObject.SetActive(false);

        //按钮抬起onup事件varBoom1Btn
    }


    protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
    {
        base.OnUpdate(elapseSeconds, realElapseSeconds);
        if (LvTimerV >= 0)
        {
            varTimeBar.fillAmount = LvTimerV / LvTimer;
            LvTimerV -= realElapseSeconds;
            if (LvTimerV < 0 )
            {
                LvTimerV = -1;
                var curProcedure = GF.Procedure.CurrentProcedure;
                if (curProcedure is GameProcedure)
                {
                    var gameProcedure = curProcedure as GameProcedure;
                    OnClose(false, null);
                    gameProcedure.OnGameOver(false);
                }
            }
        }

        #region 2-1

        //随机小幅度震动varNodAttack
        varNodAttack.DOShakePosition(0.5f, new Vector3(5f, 5f, 0), 10, 90, false, true);
        varNodAttack.transform.DOMove(varNodAttack.transform.position + new Vector3(-0.5f, 0, 0), 1f);


        if (varNodAttack.transform.localPosition.x >= 2000)
        {
            LvTimerV = 20;
            LvTimer = 20;
            varNodAttack.gameObject.SetActive(false);
            varBtnCilck.gameObject.SetActive(false);
            varNodBoom.gameObject.SetActive(true);
        }

        #endregion

        #region 2-2

        //长按
        OnDrag();

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

        if (
            varNodBoom3.gameObject.transform.eulerAngles.z >= 0 &&
            varNodBoom3.gameObject.transform.eulerAngles.z <= 10 ||
            varNodBoom3.gameObject.transform.eulerAngles.z >= 350 &&
            varNodBoom3.gameObject.transform.eulerAngles.z <= 360)
        {
            boom3 = true;
            varNodboom3Arr[0].color = Color.yellow;
            varNodboom3Arr[1].color = Color.yellow;
        }
        else
        {
            boom3 = false;
            varNodboom3Arr[0].color = Color.blue;
            varNodboom3Arr[1].color = Color.blue;
        }

        //开启下一关
        if (boom1 && boom2 && boom3&& !isDragging1&&!isDragging2&&!isDragging3&&!lv3Start)
        {
            LvTimerV = 15;
            LvTimer = 15;
            varNodBoom.gameObject.SetActive(false);
            varNodEnd.gameObject.SetActive(true);
            varNodboom2.gameObject.transform.eulerAngles = new Vector3(0, 0, -1);
            boom1 = false;
            lv3Start = true;
        }

        #endregion


        #region 2-3

        if (lv3Start)
        {
            if (!isStop)
            {
                //控制varPoint在x轴移动到-500到500之间,循环
                float x = Mathf.PingPong(Time.time * speed * (1 + lv3Times), range * 2) - range;
                varPoint.transform.localPosition = new Vector3(x, 380, 0);
            }
        }

        if (lv3Times > 3)
        {
            LvTimerV = 10;
            var curProcedure = GF.Procedure.CurrentProcedure;
            if (curProcedure is GameProcedure)
            {
                var gameProcedure = curProcedure as GameProcedure;
                GF.Event.Unsubscribe(PlayerEventArgs.EventId, OnPlayerEvent);
                gameProcedure.OnGameOver(true);
                OnClose(false, null);
            }
        }

        #endregion
    }

    private void OnPlayerEvent(object sender, GameEventArgs e)
    {
        var args = e as PlayerEventArgs;
    }

    protected override void OnButtonClick(object sender, string btId)
    {
        base.OnButtonClick(sender, btId);
        switch (btId)
        {
            case "BtnClick":
                //varNodAttack向右移动一小段
                varNodAttack.transform.DOMove(varNodAttack.transform.position + new Vector3(20, 0, 0), 1f);
                break;
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
            case "boom3Btn":
                if (!isDragging3)
                {
                    isDragging3 = true;
                }
                else
                {
                    isDragging3 = false;
                }

                break;
            case "BtnLv3Click":
                if (!isStop)
                {
                    isStop = true;
                    if (-120 < varPoint.transform.localPosition.x && varPoint.transform.localPosition.x < 120)
                    {
                        ActionKit.Sequence()
                            .Callback(() =>
                                varKongbu.transform.DOLocalMove(varKongbu.transform.localPosition + new Vector3(0, 150f, 0), 0.3f))
                            .Delay(0.1f)
                            .Callback(() =>
                                varKongjie.transform.DOLocalMove(varKongjie.transform.localPosition + new Vector3(0, 50f, 0),
                                    0.5f))
                            .Delay(0.2f)
                            .Callback(() =>
                                varKongbu.transform.DOLocalMove(varKongbu.transform.localPosition + new Vector3(0, -150f, 0),
                                    0.5f))
                            .Delay(0.5f)
                            .Callback(() => isStop = false)
                            .Start(this);
                        lv3Times++;
                    }
                    else
                    {
                        ActionKit.Sequence()
                            .Delay(1f)
                            .Callback(() => isStop = false)
                            .Start(this);
                    }
                }

                break;
        }
    }


    #region 2-2

    private void OnDrag()
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

        if (isDragging3)
        {
            //增量旋转
            varNodBoom3.gameObject.transform.DOLocalRotate(new Vector3(0, 0, 1), 0.1f, RotateMode.LocalAxisAdd);
            if (varNodBoom3.gameObject.transform.localRotation.z >= 360)
            {
                varNodBoom3.gameObject.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging1 = false;
        isDragging2 = false;
        isDragging3 = false;
    }

    #endregion

    #region 2-3

    #endregion

    protected override void OnClose(bool isShutdown, object userData)
    {
        base.OnClose(isShutdown, userData);
    }
}
﻿using GameFramework;
using GameFramework.Event;
using GameFramework.Fsm;
using GameFramework.Procedure;
using UnityEngine;
using UnityGameFramework.Runtime;

public class SecretManMenuProcedure : ProcedureBase
{
    int menuUIFormId;
    int LevelBaseId;
    LevelBase lvEntity;
    MenuUIForm menuUIForm;

    IFsm<IProcedureManager> procedure;

    protected override void OnInit(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnInit(procedureOwner);
    }

    protected override void OnEnter(IFsm<IProcedureManager> procedureOwner)
    {
        base.OnEnter(procedureOwner);
        procedure = procedureOwner;
        GF.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess); //订阅Entity打开事件, Entity显示成功时触发
        GF.Event.Subscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess); //订阅UI打开事件, UI打开成功时触发
        ShowLevel(); //加载关卡
        //var res = await GF.WebRequest.AddWebRequestAsync("https://blog.csdn.net/final5788");
        //Log.Info(Utility.Converter.GetString(res.Bytes));
    }

    protected override void OnUpdate(IFsm<IProcedureManager> procedureOwner, float elapseSeconds,
        float realElapseSeconds)
    {
        base.OnUpdate(procedureOwner, elapseSeconds, realElapseSeconds);
        if (menuUIForm == null|| lvEntity == null )
        {
            return;
        }
        

        if (GF.UI.IsValidUIForm(menuUIForm.UIForm) && GF.Entity.IsValidEntity(lvEntity.Entity) && lvEntity.IsAllReady)
        {
            if (Input.GetMouseButtonDown(0) && !GF.UI.IsPointerOverUIObject(Input.mousePosition) &&
                GF.UI.GetTopUIFormId() == menuUIFormId)
            {
                EnterGame();
            }
        }
    }

    protected override void OnLeave(IFsm<IProcedureManager> procedureOwner, bool isShutdown)
    {
        if (!isShutdown)
        {
            GF.UI.CloseUIForm(menuUIFormId);
        }

        GF.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GF.Event.Unsubscribe(OpenUIFormSuccessEventArgs.EventId, OnOpenUIFormSuccess);
        base.OnLeave(procedureOwner, isShutdown);
    }

    public void EnterGame()
    {
        this.procedure.SetData<VarInt32>("LevelBaseId", LevelBaseId);
        ChangeState<GameProcedure>(procedure);
    }

    public void ShowLevel()
    {
        lvEntity = null;
        menuUIForm = null;
        if (GF.Base.IsGamePaused)
        {
            GF.Base.ResumeGame();
        }

        GF.UI.CloseAllLoadingUIForms();
        GF.UI.CloseAllLoadedUIForms();
        GF.Entity.HideAllLoadingEntities();
        GF.Entity.HideAllLoadedEntities();

        //异步打开主菜单UI
        var uiParms = UIParams.Create(); //用于给UI传递各种参数
        uiParms.OpenCallback = uiLogic => { menuUIForm = uiLogic as MenuUIForm; };
        menuUIFormId = GF.UI.OpenUIForm(UIViews.MenuUIForm, uiParms);

        //动态创建关卡
        var lvTb = GF.DataTable.GetDataTable<LevelTable>();
        var playerMd = GF.DataModel.GetOrCreate<PlayerDataModel>();
        var lvRow = lvTb.GetDataRow(playerMd.GAME_LEVEL);

        var lvParams = EntityParams.Create(Vector3.zero, Vector3.zero, Vector3.one);
        lvParams.Set(LevelBase.P_LevelData, lvRow);
        lvParams.Set(LevelBase.P_LevelReadyCallback, (GameFrameworkAction)OnLevelAllReady);
        LevelBaseId = GF.Entity.ShowEntity(lvRow.LvPfbName, lvRow.LvLogicName, Const.EntityGroup.Level, lvParams);
    }

    private void OnLevelAllReady()
    {
        procedure.SetData<VarUnityObject>("LevelBase", lvEntity);
        GF.BuiltinView.HideLoadingProgress();
    }

    private void OnOpenUIFormSuccess(object sender, GameEventArgs e)
    {
        var args = e as OpenUIFormSuccessEventArgs;
    }

    private void OnShowEntitySuccess(object sender, GameEventArgs e)
    {
        var args = e as ShowEntitySuccessEventArgs;
        if (args.Entity.Id == LevelBaseId)
        {
            lvEntity = args.Entity.Logic as LevelBase;
            //CameraFollower.Instance.SetFollowTarget(lvEntity.transform);
        }
    }
}
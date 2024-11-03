using GameFramework;
using GameFramework.Event;
using System.Collections.Generic;
using UnityEngine;
using UnityGameFramework.Runtime;

public class Level1d1Entity : LevelBase
{
    public const string P_LevelData = "LevelData";
    public const string P_LevelReadyCallback = "OnLevelReady";
    public bool IsAllReady { get; private set; }
    private Transform playerSpawnPoint;
    private Transform KongjieSpawnPoint;
    List<int> loadEntityTaskList;
    int mPlayerId;
    int mKongjieId;
    public int PlayerId { get => mPlayerId; }
    List<int> enemyList;
    
    private GameObject m_Item;

    protected override void OnInit(object userData)
    {
        base.OnInit(userData);
        loadEntityTaskList = new List<int>();
        enemyList = new List<int>();
        playerSpawnPoint = transform.Find("PlayerSpawnPoint");
        KongjieSpawnPoint = transform.Find("KongjieSpawnPoint");
        m_Item = GameObject.Find(("Item"));
        m_Item.SetActive(false);
    }
    protected override void OnShow(object userData)
    {
        base.OnShow(userData);
        GF.Event.Subscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GF.Event.Subscribe(PlayerEventArgs.EventId, OnPlayerEvent);

        IsAllReady = false;
        loadEntityTaskList?.Clear();
        enemyList?.Clear();
        SpawnAllEntity();
    }
    protected override void OnHide(bool isShutdown, object userData)
    {
        GF.Event.Unsubscribe(ShowEntitySuccessEventArgs.EventId, OnShowEntitySuccess);
        GF.Event.Unsubscribe(PlayerEventArgs.EventId, OnPlayerEvent);

        base.OnHide(isShutdown, userData);
    }


    private void SpawnAllEntity()
    {
        var playerParams = EntityParams.Create(playerSpawnPoint.position, playerSpawnPoint.eulerAngles, playerSpawnPoint.localScale);

        mPlayerId = GF.Entity.ShowEntity<KongbuEntity>("Player", Const.EntityGroup.Player, playerParams);
        loadEntityTaskList.Add(mPlayerId);
        
       
    }
    public void StartGame()
    {
        // var player = GF.Entity.GetEntity<PlayerEntity>(mPlayerId);
        // player.Ctrlable = true;
    }
    private void OnShowEntitySuccess(object sender, GameEventArgs e)
    {
        var eArgs = e as ShowEntitySuccessEventArgs;
        if (loadEntityTaskList.Contains(eArgs.Entity.Id))
        {
            loadEntityTaskList.Remove(eArgs.Entity.Id);
            IsAllReady = loadEntityTaskList.Count <= 0;
            if (eArgs.Entity.Id == mPlayerId)
            {
               // CameraFollower.Instance.SetFollowTarget(eArgs.Entity.transform);
            }
            if (IsAllReady)
            {
                if (Params.TryGet<VarObject>(LevelEntity.P_LevelReadyCallback, out var callback))
                {
                    (callback.Value as GameFrameworkAction).Invoke();
                }
            }
        }
    }
    
    private void OnPlayerEvent(object sender, GameEventArgs e)
    {
        var args = e as PlayerEventArgs;
        
        switch (args.EventType)
        {
            case PlayerEventType.CreateKongjie:
                CreateKongjie();
                // 重置时间    
                break;
        }
        
    }

    internal void AddEnemies(int v)
    {
        var player = GF.Entity.GetEntity<PlayerEntity>(mPlayerId);
        int spawnCount = v;
        for (int i = 0; i < spawnCount; i++)
        {
            var randomPos = UnityEngine.Random.insideUnitCircle * 5;
            var enemyParams = EntityParams.Create();
            enemyParams.position = player.transform.position + new Vector3(randomPos.x, 0, randomPos.y);
            enemyParams.eulerAngles = Vector3.up * UnityEngine.Random.value * 360f;
            var enemyId = GF.Entity.ShowEntity<SampleEntity>("MyPlayer", Const.EntityGroup.Player, enemyParams);
            enemyList.Add(enemyId);
        }
    }

    internal void RemoveEnemies(int v)
    {
        for (int i = 0; i < v; i++)
        {
            if (enemyList.Count <= 0)
            {
                break;
            }
            int eId = enemyList[0];
            GF.Entity.HideEntitySafe(eId);
            enemyList.RemoveAt(0);
        }
    }
    
    
    internal void CreateKongjie()
    {
        var kongjieParams = EntityParams.Create(KongjieSpawnPoint.position, KongjieSpawnPoint.eulerAngles, KongjieSpawnPoint.localScale);

        mKongjieId = GF.Entity.ShowEntity<KongjieEntity>("Kongjie", Const.EntityGroup.Player, kongjieParams);
        loadEntityTaskList.Add(mKongjieId);
        m_Item.SetActive(true);
    }
}

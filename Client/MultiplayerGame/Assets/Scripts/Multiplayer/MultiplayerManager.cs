using System.Collections.Generic;
using Colyseus;
using Core;
using UnityEngine;

public class MultiplayerManager : ColyseusManager<MultiplayerManager>
{
    [SerializeField] public Skins skins;
    [field: SerializeField] public LossCounter lossCounter { get; private set; }
    [field: SerializeField] public SpawnPoints spawnPoints { get; private set; }
    [SerializeField] private PlayerCharacter _player;
    [SerializeField] private EnemyController _enemy;
    
    private ColyseusRoom<State> _room;
    private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
    protected override void Awake()
    {
        base.Awake();
        
        Instance.InitializeClient();
        Connect();
    }

    private async void Connect()
    {
        spawnPoints.GetPoint(Random.Range(0, spawnPoints.length), out Vector3 spawnPosition, out Vector3 spawnRotation);
        
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
            {"skins", skins.Length},
            {"spPoints", spawnPoints.length},
            {"speed", _player.speed},
            {"hp", _player.maxHealth},
            {"spX", spawnPosition.x},
            {"spY", spawnPosition.y},
            {"spZ", spawnPosition.z},
            {"sprY", spawnRotation.y},
        };
        
        _room = await Instance.client.JoinOrCreate<State>("state_handler", data);
        
        _room.OnStateChange += StateChange;
        _room.OnMessage<string>("Shoot", ApplyShoot);
    }

    private void ApplyShoot(string jsonShootInfo)
    {
        ShootInfo shootInfo = JsonUtility.FromJson<ShootInfo>(jsonShootInfo);
        if (_enemies.ContainsKey(shootInfo.key) == false)
        {
            Debug.LogError("Enemy is have not, but he is try to shoot");
            return;
        }
        _enemies[shootInfo.key].Shoot(shootInfo);
    }

    private void StateChange(State state, bool isFirststate)
    {
        if (!isFirststate) return;
        
        state.players.ForEach((key, player) =>
        {
          if (key == _room.SessionId) CreatePlayer(player);
          else CreateEnemy(key, player);
        });

        _room.State.players.OnAdd += CreateEnemy;
        _room.State.players.OnRemove += RemoveEnemy;
    }

    private void CreatePlayer(Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);
        Quaternion rotation = Quaternion.Euler(0, player.rY, 0);
        
        var playerCharacter = Instantiate(_player, position, rotation);

        player.OnChange += playerCharacter.OnChange;
        _room.OnMessage<int>("Restart", playerCharacter.GetComponent<PlayerController>().Restart);
        playerCharacter.GetComponent<SetSkin>().Set(skins.GetMaterial(player.skin));
    }
    

    private void CreateEnemy(string sessionID, Player player)
    {
        var position = new Vector3(player.pX, player.pY, player.pZ);

        var enemy = Instantiate(_enemy, position, Quaternion.identity);
        
        enemy.Init(player, sessionID);
        enemy.GetComponent<SetSkin>().Set(skins.GetMaterial(player.skin));
        
        _enemies.Add(sessionID, enemy);
    }
    
    private void RemoveEnemy(string key, Player value)
    {
        if (!_enemies.ContainsKey(key)) return;

        var enemy = _enemies[key];
        enemy.Destroy();

        _enemies.Remove(key);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        _room.Leave();
    }

    public void SendMessage(string key, Dictionary<string, object> data)
    {
        _room.Send(key, data);
    }

    public void SendMessage(string key, string data)
    {
        _room.Send(key, data);
    }

    public string GetSessionId() =>  _room.SessionId;
}

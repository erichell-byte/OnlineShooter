using System.Collections.Generic;
using Colyseus;
using Unity.VisualScripting;
using UnityEngine;

namespace SnakeGame
{
    public class MultiplayerManager : ColyseusManager<MultiplayerManager>
    {
        #region Server
        
        private const string GameRoomName = "state_handler";

        private ColyseusRoom<State> _room;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            InitializeClient();
            Connection();
        }

        private async void Connection()
        {
            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "skins", _skins.Length }
            };
            
            _room = await client.JoinOrCreate<State>(GameRoomName, data);
            _room.OnStateChange += OnChange;
        }

        private void OnChange(State state, bool isFirstState)
        {
            if (!isFirstState) return;
            _room.OnStateChange -= OnChange;
            
            state.players.ForEach((key, player) =>
            {
                if (key == _room.SessionId) CreatePlayer(player);
                else CreateEnemy(key, player);
            });

            state.players.OnAdd += CreateEnemy;
            state.players.OnRemove += RemoveEnemy;
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _room.Send(key, data);
        }

        protected override void OnApplicationQuit()
        {
            base.OnApplicationQuit();
            LeaveRoom();
        }

        private void LeaveRoom()
        {
            _room?.Leave();
        }

        #endregion

        #region Player

        [SerializeField] private Controller _controllerPrefab;
        [SerializeField] private SnakeHead snakeHeadPrefab;
        [SerializeField] private PlayerAim _playerAim;
        [SerializeField] private Skins _skins;

        private void CreatePlayer(Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            Quaternion quaternion = Quaternion.identity;
            
            SnakeHead snakeHead = Instantiate(snakeHeadPrefab, position, quaternion);
            snakeHead.Init(player.d, _skins.GetMaterial(player.skin));

            PlayerAim aim = Instantiate(_playerAim, position, quaternion);
            aim.Init(snakeHead.Speed);
            Controller controller = Instantiate(_controllerPrefab);
            controller.Init(aim, player, snakeHead);
        }

        #endregion
        
        #region Enemy

        private Dictionary<string, EnemyController> _enemies = new Dictionary<string, EnemyController>();
        private void CreateEnemy(string key, Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            SnakeHead snakeHead = Instantiate(snakeHeadPrefab, position, Quaternion.identity);
            
            snakeHead.Init(player.d, _skins.GetMaterial(player.skin));
            EnemyController enemy = snakeHead.AddComponent<EnemyController>();
            enemy.Init(player, snakeHead);
            
            _enemies.Add(key, enemy);
        }
        
        private void RemoveEnemy(string key, Player value)
        {
            if (!_enemies.ContainsKey(key))
            {
                Debug.LogError("Try to delete enemy, which does not exist in dictionary");
                return;
            }

            EnemyController enemy = _enemies[key];
            _enemies.Remove(key);
            enemy.Destroy();

        }
        
        #endregion
    }
}
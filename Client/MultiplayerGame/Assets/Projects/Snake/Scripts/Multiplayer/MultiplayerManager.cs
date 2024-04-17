using System.Collections.Generic;
using System.Linq;
using Colyseus;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

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
                { "login", PlayerSettings.Instance.Login},
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

            _room.State.apples.ForEach(CreateApple);
            _room.State.apples.OnAdd += (key, apple) => CreateApple(apple);
            _room.State.apples.OnRemove += RemoveApple;
        }

        public void SendMessage(string key, Dictionary<string, object> data)
        {
            _room.Send(key, data);
        }
        
        public void SendMessage(string key, string data)
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
            snakeHead.Init(player.d, _skins.GetMaterial(player.skin), isPlayer: true);

            PlayerAim aim = Instantiate(_playerAim, position, quaternion);
            aim.Init(snakeHead._head, snakeHead.Speed);
            
            Controller controller = Instantiate(_controllerPrefab);
            controller.Init(_room.SessionId, aim, player, snakeHead);
            
            AddLeader(_room.SessionId, player);
        }

        #endregion
        
        #region Enemy

        private Dictionary<string, EnemyController> _enemies = new ();
        private void CreateEnemy(string clientId, Player player)
        {
            Vector3 position = new Vector3(player.x, 0, player.z);
            SnakeHead snakeHead = Instantiate(snakeHeadPrefab, position, Quaternion.identity);
            
            snakeHead.Init(player.d, _skins.GetMaterial(player.skin));
            EnemyController enemy = snakeHead.AddComponent<EnemyController>();
            enemy.Init(clientId, player, snakeHead);
            
            _enemies.Add(clientId, enemy);
            
            AddLeader(clientId, player);
        }
        
        private void RemoveEnemy(string key, Player value)
        {
            if (key == _room.SessionId) return;
            
            RemoveLeader(key);
            
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
        
        #region Apple

        [SerializeField] private Apple _applePrefab;
        private Dictionary<Vector2Float,Apple> _apples = new ();
        private void CreateApple(Vector2Float vector2Float)
        {
            Vector3 position = new Vector3(vector2Float.x, 0, vector2Float.z);
            Apple apple = Instantiate(_applePrefab, position, Quaternion.identity);
            apple.Init(vector2Float);
            _apples.Add(vector2Float, apple); 
        }
        
        private void RemoveApple(int key, Vector2Float vector2Float)
        {
            if (_apples.ContainsKey(vector2Float) == false) return;

            Apple apple = _apples[vector2Float];
            _apples.Remove(vector2Float);
            apple.Destroy();
        }
        #endregion
        
        #region LeaderBoard

        private class LoginScorePair
        {
            public string login;
            public float score;
        }
        
        [SerializeField] private Text _text;

        private Dictionary<string, LoginScorePair> _leaders = new Dictionary<string, LoginScorePair>();

        private void AddLeader(string sessionId, Player player)
        {
            if (_leaders.ContainsKey(sessionId)) return;
            
            _leaders.Add(sessionId, new LoginScorePair()
            {
                login = player.login,
                score = player.score
            });
            
            UpdateBoard();
        }

        private void RemoveLeader(string sessionId)
        {
            if (_leaders.ContainsKey(sessionId) == false) return;

            _leaders.Remove(sessionId);
            
            UpdateBoard();
        }

        public void UpdateScore(string sessionid, int score)
        {
            if (_leaders.ContainsKey(sessionid) == false) return;

            _leaders[sessionid].score = score;
            UpdateBoard();
        }

        private void UpdateBoard()
        {
            int topCount = Mathf.Clamp(_leaders.Count, 0, 8);
            var top8 = _leaders.OrderByDescending(pair => pair.Value.score).Take(topCount);

            string text = "";
            int i = 1;
            foreach (var item in top8)
            {
                text += $"{i}. {item.Value.login}: {item.Value.score}\n";
                i++;
            }

            _text.text = text;
        }

        #endregion
    }
}
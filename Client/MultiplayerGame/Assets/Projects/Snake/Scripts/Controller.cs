using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

namespace SnakeGame
{
    public class Controller : MonoBehaviour
    {
        [SerializeField] private Transform _cursor;
        [SerializeField] private float _cameraOffsetY = 15f;

        private MultiplayerManager _multiplayerManager;
        private PlayerAim _playerAim;
        private Player _player;
        private SnakeHead _snakeHead;
        private Camera _camera;
        private Plane _plane;
        private string _clientId;


        public void Init(string clientId, PlayerAim aim, Player player, SnakeHead snakeHead)
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _playerAim = aim;
            _player = player;
            _clientId = clientId;
            _snakeHead = snakeHead;
            _camera = Camera.main;
            _plane = new Plane(Vector3.up, Vector3.zero);
            
            _camera.transform.parent = snakeHead.transform;
            _camera.transform.localPosition = Vector3.up * _cameraOffsetY;
            
            _snakeHead._loginView.SetLoginText(player.login);
            
            _player.OnChange += OnChange;
            
        }

        private void Update()
        {
            if (Input.GetMouseButton(0))
            {
                MoveCursor();
                _playerAim.SetTargetDirection(_cursor.position);
            }

            SendMove();
        }

        private void SendMove()
        {
            _playerAim.GetMoveInfo(out Vector3 position);

            Dictionary<string, object> data = new Dictionary<string, object>()
            {
                { "x", position.x },
                { "z", position.z }
            };
            
            _multiplayerManager.SendMessage("move", data);
            
        }

        private void MoveCursor()
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            _plane.Raycast(ray, out float distance);
            Vector3 point = ray.GetPoint(distance);

            _cursor.position = point;
        }
        
        private void OnChange(List<DataChange> changes)
        {
            if (_snakeHead == null) return;
            
            var position = _snakeHead.transform.position;
            for (int i = 0; i < changes.Count; i++)
            {
                switch (changes[i].Field)
                {
                    case "x":
                        position.x = (float)changes[i].Value;
                        break;
                    case "z":
                        position.z = (float)changes[i].Value;
                        break;
                    case "d":
                        _snakeHead.SetDetailCount((byte)changes[i].Value);
                        break;
                    case "score":
                        _multiplayerManager.UpdateScore(_clientId, (ushort)changes[i].Value);
                        break;
                    default:
                        Debug.LogWarning($"Нe обрабатывается изменение поля {changes[i].Value}");
                        break;
                }
            }
            //TODO может нужно позицию задавать playerAim?
            _snakeHead.SetRotation(position);
        }

        public void Destroy()
        {
            _camera.transform.parent = null;
            
            _player.OnChange -= OnChange;
            _snakeHead.Destroy(_clientId);
            Destroy(gameObject);
        }
    }
}

using System.Collections.Generic;
using Colyseus.Schema;
using Unity.VisualScripting;
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
        private SnakeHead snakeHead;
        private Camera _camera;
        private Plane _plane;


        public void Init(PlayerAim aim, Player player, SnakeHead snakeHead)
        {
            _multiplayerManager = MultiplayerManager.Instance;
            _playerAim = aim;
            _player = player;
            this.snakeHead = snakeHead;
            _camera = Camera.main;
            _plane = new Plane(Vector3.up, Vector3.zero);
            
            this.snakeHead.AddComponent<CameraManager>().Init(_cameraOffsetY);
            
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
            var position = snakeHead.transform.position;
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
                        snakeHead.SetDetailCount((byte)changes[i].Value);
                        break;
                    default:
                        Debug.LogWarning($"Нe обрабатывается изменение поля {changes[i].Value}");
                        break;
                }
            }
            snakeHead.SetRotation(position);
        }

        public void Destroy()
        {
            _player.OnChange -= OnChange;
            snakeHead.Destroy();
        }
    }
}

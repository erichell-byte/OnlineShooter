using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

namespace SnakeGame
{
    public class EnemyController : MonoBehaviour
    {
        private SnakeHead _snakeHead;
        private Player _player;
        private string _clientId;
        
        public void Init(string clientId, Player player, SnakeHead snakeHead)
        {
            _player = player;
            _snakeHead = snakeHead;
            _clientId = clientId;
            
            _snakeHead._loginView.SetLoginText(player.login);
            
            _player.OnChange += OnChange;
        }

        private void OnChange(List<DataChange> changes)
        {
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
                        MultiplayerManager.Instance.UpdateScore(_clientId, (ushort)changes[i].Value);
                        break;
                    default:
                        Debug.LogWarning($"Нe обрабатывается изменение поля {changes[i].Value}");
                        break;
                }
            }
            _snakeHead.SetRotation(position);
        }

        public void Destroy()
        {
            _player.OnChange -= OnChange;
            _snakeHead.Destroy(_clientId);
        }
    }
}
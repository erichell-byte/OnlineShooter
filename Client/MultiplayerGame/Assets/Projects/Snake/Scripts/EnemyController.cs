using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

namespace SnakeGame
{
    public class EnemyController : MonoBehaviour
    {
        private SnakeHead snakeHead;
        private Player _player;
        public void Init(Player player, SnakeHead snakeHead)
        {
            _player = player;
            this.snakeHead = snakeHead;
            
            _player.OnChange += OnChange;
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
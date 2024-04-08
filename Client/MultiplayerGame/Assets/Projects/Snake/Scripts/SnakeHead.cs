using UnityEngine;

namespace SnakeGame
{
    public class SnakeHead : SnakeAbstractPart
    {
        public float Speed
        {
            get { return _speed; }
        }
        [SerializeField] private SnakeTail snakeTailPrefab;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private Transform _head;

        private Vector3 _targetDirection;
        private SnakeTail snakeTail;

        public void Init(int detailCount, Material skinMaterial)
        {
            snakeTail = Instantiate(snakeTailPrefab, transform.position, Quaternion.identity);
            snakeTail.Init(_head, _speed, detailCount, skinMaterial);
            SetMaterial(skinMaterial);
        }

        public void SetDetailCount(int detailCount)
        {
            snakeTail.SetDetailCount(detailCount);
        }

        private void Update()
        {
            Move();
        }

        private void Move()
        {
            transform.position += _head.forward * Time.deltaTime * _speed;
        }

        public void SetRotation(Vector3 pointToLook)
        {
            _head.LookAt(pointToLook);
        }
        
        
        public void Destroy()
        {
            snakeTail.Destroy();
            Destroy(gameObject);
        }
    }
}
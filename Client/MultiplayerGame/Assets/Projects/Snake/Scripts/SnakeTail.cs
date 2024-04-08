
using System.Collections.Generic;
using UnityEngine;

namespace SnakeGame
{
    public class SnakeTail : SnakeAbstractPart
    {
        [SerializeField] private SnakeDetail _detailPrefab;
        [SerializeField] private float _detailDistance;

        private float _snakeSpeed = 3f;
        private Transform _head;
        private List<Transform> _details = new();
        private List<Vector3> _positionHistory = new List<Vector3>();
        private List<Quaternion> _rotationHistory = new List<Quaternion>();
        private Material _skinMaterial;

        public void Init(Transform head, float snakeSpeed, int detailCount, Material skinMaterial)
        {
            _snakeSpeed = snakeSpeed;
            _head = head;
            _skinMaterial = skinMaterial;
            _details.Add(transform);
            _positionHistory.Add(_head.position);
            _rotationHistory.Add(_head.rotation);
            _positionHistory.Add(transform.position);
            _rotationHistory.Add(transform.rotation);

            SetDetailCount(detailCount);
            SetMaterial(_skinMaterial);
        }

        public void Destroy()
        {
            for (int i = 0; i < _details.Count; i++)
            {
                Destroy(_details[i].gameObject);
            }
        }

        private void Update()
        {
            float distance = (_head.position - _positionHistory[0]).magnitude;

            while (distance > _detailDistance)
            {
                Vector3 direction = (_head.position - _positionHistory[0]).normalized;
                
                _positionHistory.Insert(0, _positionHistory[0] + direction * _detailDistance);
                _positionHistory.RemoveAt(_positionHistory.Count - 1);
                
                _rotationHistory.Insert(0, _head.rotation);
                _rotationHistory.RemoveAt(_rotationHistory.Count - 1);
                
                distance -= _detailDistance;
            }

            for (int i = 0; i < _details.Count; i++)
            {
                float percent = distance / _detailDistance;
                _details[i].position = Vector3.Lerp(_positionHistory[i + 1], _positionHistory[i], percent);
                _details[i].rotation = Quaternion.Lerp(_rotationHistory[i + 1], _rotationHistory[i], percent);
            }
        }

        public void SetDetailCount(int detailCount)
        {
            if (_details.Count - 1 == detailCount) return;

            int diff = (_details.Count - 1) - detailCount;

            if (diff < 1)
            {
                for (int i = 0; i < -diff; i++)
                {
                    AddDetail();
                }
            }
            else
            {
                for (int i = 0; i < diff; i++)
                {
                    RemoveDetail();
                }
            }
        }

        private void AddDetail()
        {
            Vector3 position = _details[_details.Count - 1].position;
            Quaternion rotation = _details[_details.Count - 1].rotation;
            var detail = Instantiate(_detailPrefab, position, rotation);
            detail.SetMaterial(_skinMaterial);
            _details.Insert(0, detail.transform);
            _positionHistory.Add(position);
            _rotationHistory.Add(rotation);
        }

        private void RemoveDetail()
        {
            if (_details.Count <= 1)
            {
                Debug.LogError("Try to delete detail, which does not exist");
                return;
            }

            Transform detail = _details[0];
            _details.Remove(detail);
            Destroy(detail.gameObject);
            _positionHistory.RemoveAt(_positionHistory.Count - 1);
            _rotationHistory.RemoveAt(_rotationHistory.Count - 1);
        }


    }
}
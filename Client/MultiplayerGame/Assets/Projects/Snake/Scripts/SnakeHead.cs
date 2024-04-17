using UnityEngine;

namespace SnakeGame
{
	public class SnakeHead : SnakeAbstractPart
	{
		public float Speed
		{
			get { return _speed; }
		}

		[field: SerializeField] public Transform _head { get; private set; }
		[field: SerializeField] public LoginView _loginView { get; private set; }

		[SerializeField] private int _playerLayer = 8;
		[SerializeField] private SnakeTail snakeTailPrefab;
		[SerializeField] private float _speed = 2f;


		private Vector3 _targetDirection;
		private SnakeTail snakeTail;

		public void Init(int detailCount, Material skinMaterial, bool isPlayer = false)
		{
			if (isPlayer)
			{
				gameObject.layer = _playerLayer;
				var childrens = GetComponentsInChildren<Transform>();
				foreach (var child in childrens)
				{
					child.gameObject.layer = _playerLayer;
				}
			}

			snakeTail = Instantiate(snakeTailPrefab, transform.position, Quaternion.identity);
			snakeTail.Init(_head, _speed, detailCount, skinMaterial, _playerLayer, isPlayer);

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
		
		public void Destroy(string clientId)
		{
			var detailPosition = snakeTail.GetDetailPositions();
			detailPosition.id = clientId;
			string json = JsonUtility.ToJson(detailPosition);
			MultiplayerManager.Instance.SendMessage("gameOver", json);
			snakeTail.Destroy();
			Destroy(gameObject);
		}
	}
}
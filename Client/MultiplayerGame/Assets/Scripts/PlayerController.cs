using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public class PlayerController : MonoBehaviour
	{
		[SerializeField] private PlayerCharacter _player;
		[SerializeField] private PlayerGun _playerGun;
		[SerializeField] private float _mouseSensetivity = 2f;

		private MultiplayerManager multiplayerManager;

		private void Start()
		{
			multiplayerManager = MultiplayerManager.Instance;
		}

		private void Update()
		{
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");

			float mouseX = Input.GetAxis("Mouse X");
			float mouseY = Input.GetAxis("Mouse Y");

			bool isShoot = Input.GetMouseButton(0);
			bool space = Input.GetKeyDown(KeyCode.Space);
			bool crouch  = Input.GetKey(KeyCode.LeftControl);
			
			_player.SetInput(h, v, mouseX * _mouseSensetivity);
			_player.RotateX(-mouseY * _mouseSensetivity);
			_player.isCrouch = crouch;
			if (space) _player.Jump();
			
			
			if (isShoot && _playerGun.TryShoot(out ShootInfo shootInfo)) SendShoot(ref shootInfo);

			SendMove();
		}

		private void SendShoot(ref ShootInfo shootInfo)
		{
			shootInfo.key = multiplayerManager.GetSessionId();
			var json = JsonUtility.ToJson(shootInfo);
			
			multiplayerManager.SendMessage("shoot", json);
		}
		
		private void SendMove()
		{
			_player.GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY, out bool isCrouch);
			Dictionary<string, object> data = new Dictionary<string, object>()
			{
				{"pX", position.x},
				{"pY", position.y},
				{"pZ", position.z},
				{"vX", velocity.x},
				{"vY", velocity.y},
				{"vZ", velocity.z},
				{"rX", rotateX},
				{"rY", rotateY},
				{"c", isCrouch ? 1 : 0},
			};
			multiplayerManager.SendMessage("move", data);
		}
		
	}

	[Serializable]
	public struct ShootInfo
	{
		public string key;
		public float pX;
		public float pY;
		public float pZ;
		public float vX;
		public float vY;
		public float vZ;
	}
}

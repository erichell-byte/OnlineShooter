using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public  class PlayerController : MonoBehaviour
	{
		[SerializeField] private float _restartDelay;
		[SerializeField] private PlayerCharacter _player;
		[SerializeField] private PlayerGun _playerGun;
		[SerializeField] private float _mouseSensetivity = 2f;

		private MultiplayerManager multiplayerManager;
		private bool _hold;
		private bool _hideCursor;
		private void Start()
		{
			multiplayerManager = MultiplayerManager.Instance;
			_hideCursor = true;
			Cursor.lockState = CursorLockMode.Locked;
		}

		private void Update()
		{
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				_hideCursor = !_hideCursor;
				Cursor.lockState = _hideCursor ? CursorLockMode.Locked : CursorLockMode.None;
			}
			
			if (_hold) return;
			float h = Input.GetAxisRaw("Horizontal");
			float v = Input.GetAxisRaw("Vertical");
			
			float mouseX = 0;
			float mouseY = 0;
			bool isShoot = false;
			
			if (_hideCursor)
			{
				mouseX = Input.GetAxis("Mouse X");
				mouseY = Input.GetAxis("Mouse Y");
				isShoot = Input.GetMouseButton(0);
			}
			
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
				{"c", isCrouch},
			};
			multiplayerManager.SendMessage("move", data);
		}
		
		public void Restart(int spawnIndex)
		{
			MultiplayerManager.Instance.spawnPoints.GetPoint(spawnIndex, out Vector3 position, out Vector3 rotation);
			StartCoroutine(Hold());

			rotation.z = 0;
			rotation.x = 0;
			_player.transform.position = position;
			_player.transform.eulerAngles = rotation;
			
			_player.SetInput(0,0,0);
			
			Dictionary<string, object> data = new Dictionary<string, object>()
			{
				{"pX", position.x},
				{"pY", position.y},
				{"pZ", position.z},
				{"vX", 0},
				{"vY", 0},
				{"vZ", 0},
				{"rX", 0},
				{"rY", rotation.y},
				{"c", false},
			};
			multiplayerManager.SendMessage("move", data);
		}

		private IEnumerator Hold()
		{
			_hold = true;
			yield return new WaitForSecondsRealtime(_restartDelay);
			_hold = false;
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

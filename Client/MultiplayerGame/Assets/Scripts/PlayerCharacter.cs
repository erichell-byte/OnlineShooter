using System;
using System.Collections.Generic;
using Colyseus.Schema;
using UnityEngine;

public class PlayerCharacter : Character
{
   [SerializeField] private Health _health;
   [SerializeField] private Rigidbody _rb;
   [SerializeField] private Transform _head;
   [SerializeField] private Transform _cameraPoint;
   [SerializeField] private float _maxHeadAngle = 90;
   [SerializeField] private float _minHeadAngle = -90;
   [SerializeField] private float _jumpForce = 50f;
   [SerializeField] private CheckFly checkFly;
   [SerializeField] private float _jumpDelay = .15f;
   
   private float _inputV;
   private float _inputH;
   private float _rotateY;
   private float _currentRotateX;
   private float _jumpTime;
   private bool _isCrouch;

   private Vector3 direction;

   private void Start()
   {
      Transform camera = Camera.main.transform;
      camera.parent = _cameraPoint;
      camera.localPosition = Vector3.zero;
      camera.localRotation = Quaternion.identity;
      
      _health.SetMax(maxHealth);
      _health.SetCurrent(maxHealth);
   }
   
   private void FixedUpdate()
   {
      Move();
      RotateY();
   }

   public void SetInput(float h, float v, float rotateY)
   {
      _inputV = h;
      _inputH = v;
      _rotateY = rotateY;
   }
   
   private void Move()
   {
      Vector3 velocity = (transform.forward * _inputH + transform.right * _inputV).normalized * speed;
      velocity.y = _rb.velocity.y;
      base.velocity = velocity;
      _rb.velocity = base.velocity;
   }

   public void RotateX(float value)
   {
      _currentRotateX = Mathf.Clamp(_currentRotateX + value, _minHeadAngle, _maxHeadAngle);
      _head.localEulerAngles = new Vector3(_currentRotateX, 0, 0);
   }

   public void RotateY()
   {
      _rb.angularVelocity = new Vector3(0, _rotateY, 0);
      _rotateY = 0;
   }

   public void GetMoveInfo(out Vector3 position, out Vector3 velocity, out float rotateX, out float rotateY, out bool isCrouch)
   {
      position = transform.position;
      velocity = _rb.velocity;
      rotateY = transform.eulerAngles.y;
      rotateX = _head.localEulerAngles.x;
      isCrouch = base.isCrouch;
   }
   
   public void Jump()
   {
      if (checkFly.IsFly) return;
      if (Time.time - _jumpTime < _jumpDelay) return;

      _jumpTime = Time.time;
      _rb.AddForce(0,_jumpForce,0, ForceMode.VelocityChange);
   }
   
   public void OnChange(List<DataChange> changes)
   {
      foreach (var dataChange in changes)
      {
         switch (dataChange.Field)
         {
            case "currentHP":
               _health.SetCurrent((sbyte)dataChange.Value);
               break;
            case "loss":
               MultiplayerManager.Instance._lossCounter.SetPlayerLoss((byte)dataChange.Value);
               break;
            default:
               Debug.LogWarning("Не обрабатывается изменение поля" + dataChange.Field);
               break;
         }
      }
   }
   
}

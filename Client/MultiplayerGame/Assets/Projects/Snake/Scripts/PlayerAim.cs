using UnityEngine;

public class PlayerAim : MonoBehaviour
{
	[SerializeField] private float _rotationSpeed = 90f;

	private Vector3 _targetDirection = Vector3.zero;
	private float _speed;

	public void Init(float speed)
	{
		_speed = speed;
	}
	private void Update()
	{
		Rotate();
		Move();
	}

	private void Move()
	{
		transform.position += transform.forward * _speed * Time.deltaTime;
	}

	private void Rotate()
	{
		Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
		transform.rotation =
			Quaternion.RotateTowards(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
	}

	public void SetTargetDirection(Vector3 pointToLook)
	{
		_targetDirection = pointToLook - transform.position;
	}
	
	public void GetMoveInfo(out Vector3 position)
	{
		position = transform.position;
	}
}

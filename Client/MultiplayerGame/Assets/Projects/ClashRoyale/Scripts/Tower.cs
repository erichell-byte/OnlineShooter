using UnityEngine;

[RequireComponent( typeof (Health))]
public class Tower : MonoBehaviour, IHealth
{
	[field: SerializeField] public Health health { get; private set; }
	[field: SerializeField] public float radius { get; private set; } = 2f;
	
	[SerializeField] public bool isEnemy;
	
	private void Start()
	{
		health.OnDied += Destroy;
	}

	public float GetDistance(in Vector3 point)
	{
		return Vector3.Distance(transform.position, point) - radius;
	}
	
	private void Destroy()
	{
		MapInfo.Instance.RemoveEntity(isEnemy, isTower: true, this);
		Destroy(gameObject);
	}
}

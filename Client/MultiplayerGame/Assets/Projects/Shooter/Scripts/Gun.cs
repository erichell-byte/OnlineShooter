using System;
using UnityEngine;

namespace Shooter
{
	public abstract class Gun : MonoBehaviour
	{
		[SerializeField] protected Bullet _bulletPrefab;
		public Action shoot;
	}
}
using Core;
using UnityEngine;

public class PlayerGun : Gun
{
    [SerializeField] private Transform _bulletPoint;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private float _shootDelay;
    [SerializeField] private int _damage;

    private float _lastShootTime;
    public bool TryShoot(out ShootInfo info)
    {
        info = new ShootInfo();
        if (Time.time - _lastShootTime < _shootDelay) return false;

        _lastShootTime = Time.time;
        
        var position = _bulletPoint.position;
        var velocity = _bulletPoint.forward * _bulletSpeed;
        Instantiate(_bulletPrefab, position, _bulletPoint.rotation).Init(velocity, _damage);
        
        shoot?.Invoke();
        
        info.pX = position.x;
        info.pY = position.y;
        info.pZ = position.z;
        info.vX = velocity.x;
        info.vY = velocity.y;
        info.vZ = velocity.z;
        
        return true;
    }
    
}

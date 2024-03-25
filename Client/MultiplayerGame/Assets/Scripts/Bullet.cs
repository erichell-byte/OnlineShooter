using System.Collections;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidbody;
    [SerializeField] private float _lifetime = 5f;
    
    private int _damage;
    
    public void Init(Vector3 velocity, int damage = 0)
    {
        _damage = damage;
        rigidbody.velocity = velocity;
        StartCoroutine(DelayDestroy());
    }

    private IEnumerator DelayDestroy()
    {
        yield return new WaitForSecondsRealtime(_lifetime);
        Destroy();
    }

    private void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out Damageable damageableComponent))
        {
            damageableComponent.CauseDamageByBullet(_damage, collision.contacts[0].point, collision.contacts[0].normal);
        }
        Destroy();
    }
}

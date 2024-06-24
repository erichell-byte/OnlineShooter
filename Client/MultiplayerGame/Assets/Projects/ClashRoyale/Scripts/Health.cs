using System;
using System.Collections;
using UnityEngine;

public class Health : MonoBehaviour
{
    [field: SerializeField] public float max { get; private set; } = 10f;
    private float _current;

    public event Action<float> UpdateHealth; 
    public event Action OnDied; 

    private void Start()
    {
        _current = max;
    }

    public void ApplyDamage(float value)
    {
        _current -= value;
        if (_current < 0)
        {
            _current = 0;
            OnDied?.Invoke();
        }
        
        UpdateHealth?.Invoke(_current);
    }

    public void ApplyDamageDelay(float delay, float damage)
    {
        StartCoroutine(DelayDamage(delay, damage));
    }

    private IEnumerator DelayDamage(float delay, float damage)
    {
        yield return new WaitForSeconds(delay);
        ApplyDamage(damage);
    }
    
}

public interface IHealth
{
    Health health { get; }
}

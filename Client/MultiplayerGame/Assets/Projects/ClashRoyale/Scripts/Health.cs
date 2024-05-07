using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [field: SerializeField] public float max { get; private set; } = 10f;
    private float _current;

    public event Action<float> OnChangeInPercent; 
    public event Action OnDied; 

    private void Start()
    {
        _current = max;
        OnChangeInPercent?.Invoke(1f);
    }

    public void ApplyDamage(float value)
    {
        _current -= value;
        if (_current < 0)
        {
            _current = 0;
            OnDied?.Invoke();
        }
        
        OnChangeInPercent?.Invoke(_current / max);
        Debug.Log($" обьект {name}: было {_current + value}, стало {_current} здоровья");
    }
}

interface IHealth
{
    Health health { get; }
}

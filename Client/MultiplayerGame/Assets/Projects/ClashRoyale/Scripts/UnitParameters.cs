using UnityEngine;

public class UnitParameters : MonoBehaviour
{
    [SerializeField] private float _startAttackDistance  = 1f;
    [SerializeField] private float _stopAttackDistance  = 1.5f;
    [field: SerializeField] public float modelRadius { get; private set; } = 1f;
    [field: SerializeField] public float startChaseDistance { get; private set; } = 5f;
    [field: SerializeField] public float stopChaseDistance { get; private set; } = 7f;
    [field: SerializeField] public float speed { get; private set; } = 4f;
    public float startAttackDistance => modelRadius + _startAttackDistance;
    public float stopAttackDistance => modelRadius + _stopAttackDistance;
  
    
}

using UnityEngine;

[RequireComponent(typeof(UnitParameters), typeof (Health))]
public class Unit : MonoBehaviour, IHealth
{
    [field: SerializeField] public Health health { get; private set; }
    [field: SerializeField] public bool isEnemy { get; private set; }
    [field: SerializeField] public UnitParameters parameters { get; private set; }
    [SerializeField] private UnitState _defaultStateSO;
    [SerializeField] private UnitState _chaseStateSO;
    [SerializeField] private UnitState _attackStateSO;
    
    private UnitState _defaultState;
    private UnitState _chaseState;
    private UnitState _attackState;
    private UnitState _currentState;
    
    private void Start()
    {
        health.OnDied += Destroy;
        _defaultState = Instantiate(_defaultStateSO);
        _defaultState.Constructor(this);
        
        _chaseState = Instantiate(_chaseStateSO);
        _chaseState.Constructor(this);
        
        _attackState = Instantiate(_attackStateSO);
        _attackState.Constructor(this);

        _currentState = _defaultState;
        _currentState.Init();
    }

    private void Update()
    {
        _currentState.Run();
    }

    public void SetState(UnitStateType stateType)
    {
        _currentState.Finish();

        switch (stateType)
        {
            case UnitStateType.Default:
                _currentState = _defaultState;
                break;
            case UnitStateType.Chase:
                _currentState = _chaseState;
                break;
            case UnitStateType.Attack:
                _currentState = _attackState;
                break;
            default:
                Debug.LogError($"didnt handle state {stateType} ");
                break;
        }
        _currentState.Init();
    }
    
    private void Destroy()
    {
        MapInfo.Instance.RemoveEntity(isEnemy, isTower: false, this);
        Destroy(gameObject);
    }
    
#if UNITY_EDITOR
    [SerializeField] private bool _isDebug = false;
    private void OnDrawGizmos()
    {
        if (!_isDebug) return;
        if (_chaseStateSO != null) _chaseStateSO.DebugDrawDistance(this);
    }
#endif
}

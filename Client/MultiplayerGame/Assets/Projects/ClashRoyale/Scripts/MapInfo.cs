using System.Collections.Generic;
using UnityEngine;

public class MapInfo : MonoBehaviour
{
    #region SingleOneScene

    public static MapInfo Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    #endregion
    
    [SerializeField] private List<Tower> _enemyTowers = new();
    [SerializeField] private List<Tower> _playerTowers = new();
    
    [SerializeField] private List<Unit> _enemyWalkingUnits = new();
    [SerializeField] private List<Unit> _playerWalkingUnits = new();
    
    [SerializeField] private List<Unit> _enemyFlyUnits = new();
    [SerializeField] private List<Unit> _playerFlynits = new();

    private void Start()
    {
        SubscribeDestroy(_enemyTowers);
        SubscribeDestroy(_playerTowers);
        SubscribeDestroy(_enemyWalkingUnits);
        SubscribeDestroy(_playerWalkingUnits);
    }

    public void AddUnit(Unit unit)
    {
        List<Unit> list;
        if (unit.isEnemy) list = unit.parameters.isFly ? _enemyFlyUnits : _enemyWalkingUnits;
        else list = unit.parameters.isFly ? _playerFlynits : _playerWalkingUnits;
        
        AddObjectToList(list, unit);
    }

    public bool TryGetNearestAnyUnit(in Vector3 currentPosition, bool isEnemy, out Unit unit, out float distance)
    {
        TryGetNearestWalkingUnit(currentPosition, isEnemy, out Unit walkingUnit, out float walkingDistance);
        TryGetNearestFlyUnit(currentPosition, isEnemy, out Unit flyingUnit, out float flyingDistance);

        if (flyingDistance < walkingDistance)
        {
            unit = flyingUnit;
            distance = flyingDistance;
        }
        else
        {
            unit = walkingUnit;
            distance = walkingDistance;
        }

        return unit;
    }
    
    public bool TryGetNearestWalkingUnit(in Vector3 currentPosition, bool isEnemy, out Unit unit, out float distance)
    {
        List<Unit> units = isEnemy ? _enemyWalkingUnits : _playerWalkingUnits;
        unit = GetNearest(currentPosition, units, out distance);

        return unit;
    }
    
    public bool TryGetNearestFlyUnit(in Vector3 currentPosition, bool isEnemy, out Unit unit, out float distance)
    {
        List<Unit> units = isEnemy ? _enemyFlyUnits : _playerFlynits;
        unit = GetNearest(currentPosition, units, out distance);

        return unit;
    }
    
    public Tower GetNearestTower(in Vector3 currentPosition, bool isEnemy)
    {
        List<Tower> towers = isEnemy ? _enemyTowers : _playerTowers;
        
        return GetNearest(currentPosition, towers, out float distance);
    }
    
    private void SubscribeDestroy<T>(List<T> objects) where T : IDestroyed
    {
        // если бы тут был цикл по индексу, то пришлось бы создавать переменную под обьект,
        // потому что передавать objects[i]  нельзя в безымянный метод, потому что на отписке он запомнит не конкретный обьект
        // а обьект под конкретным индексом. и будет всегда удалять обьетк под этим индексом
        foreach (T obj in objects) 
        {
            obj.Destroyed += RemoveAndUnsubscribe;

            void RemoveAndUnsubscribe()
            {
                RemoveObjectFromList(objects, obj);
                obj.Destroyed -= RemoveAndUnsubscribe;
            }
        }
    }
    
    private void AddObjectToList<T>(List<T> list, T obj) where T : IDestroyed
    {
        list.Add(obj);
        obj.Destroyed += RemoveAndUnsubscribe;
        
        void RemoveAndUnsubscribe()
        {
            RemoveObjectFromList(list, obj);
            obj.Destroyed -= RemoveAndUnsubscribe;
        }
    }

    private void RemoveObjectFromList<T>(List<T> list, T obj)
    {
        if (list.Contains(obj)) list.Remove(obj);
    }

    private T GetNearest<T>(in Vector3 currentPosition, List<T> objects, out float distance) where T : MonoBehaviour
    {
        distance = float.MaxValue;
        if (objects.Count <= 0) return null;

        distance = Vector3.Distance(currentPosition, objects[0].transform.position);
        T nearest = objects[0];
        for (int i = 1; i < objects.Count; i++)
        {
            var tmpDistance = Vector3.Distance(currentPosition, objects[i].transform.position);
            if (tmpDistance >= distance) continue;

            distance = tmpDistance;
            nearest = objects[i];
        }

        return nearest;
    }
}

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
    
    [SerializeField] private List<Unit> _enemyUnits = new();
    [SerializeField] private List<Unit> _playerUnits = new();
    
    public bool TryGetNearestUnit(in Vector3 currentPosition, bool isEnemy, out Unit unit, out float distance)
    {
        List<Unit> units = isEnemy ? _enemyUnits : _playerUnits;
        unit = GetNearest(currentPosition, units, out distance);

        return unit;
    }
    
    public Tower GetNearestTower(in Vector3 currentPosition, bool isEnemy)
    {
        List<Tower> towers = isEnemy ? _enemyTowers : _playerTowers;
        
        return GetNearest(currentPosition, towers, out float distance);
    }

    public void RemoveEntity(bool isEnemy, bool isTower, object entity)
    {
        if (isEnemy)
        {
            if (isTower)
            {
                _enemyTowers.Remove(entity as Tower);
            }
            else
            {
                _enemyUnits.Remove(entity as Unit);
            }
        }
        else
        {
            if (isTower)
            {
                _playerTowers.Remove(entity as Tower);
            }
            else
            {
                _playerUnits.Remove(entity as Unit);
            }
        }
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

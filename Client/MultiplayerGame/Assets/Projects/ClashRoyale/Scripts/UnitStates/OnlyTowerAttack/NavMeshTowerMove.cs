using UnityEngine;

[CreateAssetMenu(fileName = "_NavMeshTowerMove", menuName = "UnitState/NavMeshTowerMove")]
public class NavMeshTowerMove : UnitStateNavMeshMove
{
	protected override bool TryFindTarget(out UnitStateType changeType)
	{
		if (_nearestTower == null)
		{
			changeType = UnitStateType.None;
			return false;
		}
        
		float distanceToTarget = _nearestTower.GetDistance(_unit.transform.position);
		if (distanceToTarget <= _unit.parameters.startAttackDistance)
		{
			changeType = UnitStateType.Attack;
			return true;
		}
		else
		{
			changeType = UnitStateType.None;
			return false;
		}
	}
}

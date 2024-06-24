using UnityEngine;
using UnityEngine.AI;

public abstract class UnitStateNavMeshMove : UnitState
{
	private NavMeshAgent _agent;
	protected bool _targetIsEnemy;
	protected Tower _nearestTower;

	public override void Constructor(Unit unit)
	{
		base.Constructor(unit);

		_agent = _unit.GetComponent<NavMeshAgent>();
		if (_agent == null)
			Debug.LogError($"Player {unit.name} have not NavMeshAgent");

		_targetIsEnemy = !_unit.isEnemy;

		_agent.speed = _unit.parameters.speed;
		_agent.stoppingDistance = _unit.parameters.startAttackDistance;
		_agent.radius = _unit.parameters.modelRadius;
	}

	public override void Init()
	{
		Vector3 unitPosition = _unit.transform.position;
		_nearestTower = MapInfo.Instance.GetNearestTower(in unitPosition, _targetIsEnemy);
		if (_nearestTower == null) return;

		_agent.SetDestination(_nearestTower.transform.position);
	}

	public override void Run()
	{
		if (TryFindTarget(out UnitStateType unitStateType))
			_unit.SetState(unitStateType);
	}

	public override void Finish()
	{
		_agent.SetDestination(_unit.transform.position);
	}
	protected abstract bool TryFindTarget(out UnitStateType changeType);
}
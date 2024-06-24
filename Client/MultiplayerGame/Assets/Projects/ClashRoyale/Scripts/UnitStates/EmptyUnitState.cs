using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "_EmptyState", menuName = "UnitState/EmptyState")]
public class EmptyUnitState : UnitState
{
	public override void Init()
	{
		_unit.SetState(UnitStateType.Default);
	}

	public override void Run()
	{
	}

	public override void Finish()
	{
		Debug.LogWarning($"Unit {_unit.name} has been at empty state, he transit to default state");
	}
}

using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class TargetedHopState : State, IUpdatableState
	{
		private ReactiveEvent<Vector3>  _hopRequest;
		private Transform               _transform;
		private ReactiveVariable<float> _hopRange;

		private readonly EntitiesLifeContext _entitiesLifeContext;
		private readonly ITargetSelector _targetSelector;

		public TargetedHopState (Entity entity, EntitiesLifeContext entitiesLifeContext, ITargetSelector targetSelector)
		{
			_hopRequest = entity.HopRequest;
			_transform = entity.Transform;
			_hopRange = entity.HopRange;

			_entitiesLifeContext = entitiesLifeContext;
			_targetSelector = targetSelector;
		}

		public override void Enter ()
		{
			base.Enter();

			if (_entitiesLifeContext.Entities.Count <= 1)
			{
				return;
			}

			Entity target = _targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities);

			Vector3 hopDirection = target.Transform.position - _transform.position;
			float   hopDistance  = Mathf.Min(_hopRange.Value, hopDirection.magnitude);
			Vector3 newPosition  = _transform.position + hopDirection.normalized * hopDistance;

			_hopRequest.Invoke(newPosition);
		}

		public void Update (float deltaTime)
		{

		}
	}
}

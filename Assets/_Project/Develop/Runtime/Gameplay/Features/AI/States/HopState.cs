using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class HopState : State, IUpdatableState
	{
		private ReactiveEvent<Vector3> _hopRequest;
		private Transform              _transform;
		private ReactiveVariable<float> _hopRange;

		public HopState (Entity entity)
		{
			_hopRequest = entity.HopRequest;
			_transform  = entity.Transform;
			_hopRange   = entity.HopRange;
		}

		public override void Enter ()
		{
			base.Enter();

			Vector2 positionOffset          = Random.insideUnitCircle.normalized * Random.Range(0, _hopRange.Value);
			Vector3 processedPositionOffset = new Vector3(positionOffset.x, 0, positionOffset.y);
			Vector3 newPosition             = _transform.position + processedPositionOffset;

			_hopRequest?.Invoke(newPosition);
		}

		public void Update (float deltaTime)
		{

		}
	}
}

using UnityEngine;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class PlayerInputRotationState : State, IUpdatableState
	{
		private readonly IInputService _inputService;

		private ReactiveVariable<Vector3> _rotationDirection;
		private Transform _transform;

		public PlayerInputRotationState (Entity entity, IInputService inputService)
		{
			_inputService      = inputService;
			_rotationDirection = entity.RotationDirection;
			_transform = entity.Transform;
		}

		public void Update (float deltaTime)
		{
			if (_inputService.PointPosition == null)
			{
				return;
			}
				_rotationDirection.Value = _inputService.PointPosition.Value - _transform.position;
		}
	}
}

using System;
using System.Collections.Generic;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hop
{
	public class HopSystem : IInitializableSystem, IDisposableSystem
	{
		private ReactiveEvent           _hopRequest;
		private ReactiveEvent           _targetedHopRequest;
		private ReactiveEvent           _hopEvent;
		private ReactiveVariable<float> _hopRange;
		private Transform               _transform;
		private ICompositeCondition     _canHop;

		private readonly ITargetSelector         _targetSelector;
		private readonly EntitiesLifeContext _entitiesLifeContext;

		private readonly List<IDisposable> _disposables = new List<IDisposable>();

		public HopSystem (EntitiesLifeContext entitiesLifeContext, ITargetSelector targetSelector)
		{
			_entitiesLifeContext = entitiesLifeContext;
			_targetSelector = targetSelector;
		}

		public void OnInit (Entity entity)
		{
			_hopRequest          = entity.HopRequest;
			_hopEvent            = entity.HopEvent;
			_transform           = entity.Transform;
			_hopRange            = entity.HopRange;
			_canHop              = entity.CanHop;

			_disposables.Add(_hopRequest.Subscribe(OnHopRequest));
		}

		private void TargetedHop (Entity entity)
		{
			if (!_canHop.Evaluate())
			{
				return;
			}

			if (entity == null)
			{
				return;
			}

			Vector3 hopDirection = entity.Transform.position - _transform.position;
			float hopDistance = Mathf.Min(_hopRange.Value, hopDirection.magnitude);
			Vector3 newPosition = _transform.position + hopDirection.normalized * hopDistance;

			_transform.position = newPosition;

			_hopEvent.Invoke();
		}

		private void OnHopRequest ()
		{
			if (!_canHop.Evaluate())
			{
				return;
			}

			if (_entitiesLifeContext.Entities.Count > 1)
			{
				TargetedHop(_targetSelector.SelectTargetFrom(_entitiesLifeContext.Entities));
				return;
			}

			Vector2 positionOffset = Random.insideUnitCircle.normalized * Random.Range(0, _hopRange.Value);
			Vector3 processedPositionOffset = new Vector3(positionOffset.x, 0, positionOffset.y);
			Vector3 newPosition = _transform.position + processedPositionOffset;

			_transform.position = newPosition;

			_hopEvent.Invoke();
		}

		public void OnDispose ()
		{
			foreach (IDisposable disposable in _disposables)
			{
				disposable.Dispose();
			}
		}
	}
}

using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hop
{
	public class SlamSystem : IInitializableSystem, IDisposableSystem
	{
		private ReactiveEvent           _hopEvent;
		private ReactiveEvent           _slamEvent;
		private ReactiveVariable<float> _slamRange;
		private ReactiveVariable<float> _slamDamage;
		private Transform               _transform;
		private Entity                  _entity;

		private readonly CollidersRegistryService _collidersRegistry;

		private IDisposable _hopEventDisposable;

		public SlamSystem (CollidersRegistryService collidersRegistryService)
		{
			_collidersRegistry = collidersRegistryService;
		}

		public void OnInit (Entity entity)
		{
			_hopEvent   = entity.HopEvent;
			_slamEvent  = entity.SlamEvent;
			_slamRange  = entity.SlamRange;
			_slamDamage = entity.SlamDamage;
			_transform  = entity.Transform;
			_entity     = entity;

			_hopEventDisposable = _hopEvent.Subscribe(OnHopEvent);
		}

		private void OnHopEvent ()
		{
			Collider[] hitColliders = Physics.OverlapSphere(
				_transform.position,
				_slamRange.Value,
				LayerMask.GetMask("Characters"));

			foreach (Collider hitCollider in hitColliders)
			{
				Entity entity = _collidersRegistry.GetBy(hitCollider);

				if (entity.HasComponent<TakeDamageRequest>() && entity != _entity)
				{
					entity.TakeDamageRequest.Invoke(_slamDamage.Value);
				}
			}
		}

		public void OnDispose ()
		{
			_hopEventDisposable.Dispose();
		}
	}
}

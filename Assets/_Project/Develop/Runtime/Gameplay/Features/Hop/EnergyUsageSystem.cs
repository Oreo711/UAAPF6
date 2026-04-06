using System;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hop
{
	public class EnergyUsageSystem : IInitializableSystem, IDisposableSystem
	{
		private ReactiveEvent           _hopEvent;
		private ReactiveVariable<float> _energyUsage;
		private ReactiveVariable<float> _currentEnergy;

		private IDisposable _hopEventDisposable;

		public void OnInit (Entity entity)
		{
			_hopEvent    = entity.HopEvent;
			_energyUsage = entity.EnergyUsage;
			_currentEnergy = entity.CurrentEnergy;

			_hopEventDisposable = _hopEvent.Subscribe(OnHopEvent);
		}

		private void OnHopEvent ()
		{
			_currentEnergy.Value -= _energyUsage.Value;

			Debug.Log($"Remaining Energy: {_currentEnergy.Value}");
		}

		public void OnDispose ()
		{
			_hopEventDisposable.Dispose();
		}
	}
}

using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hop
{
	public class EnergyRegenerationSystem : IInitializableSystem, IUpdatableSystem
	{
		private ReactiveVariable<float> _currentEnergy;
		private ReactiveVariable<float> _maxEnergy;
		private ReactiveVariable<float> _initialEnergyRegenerationCooldown;
		private ReactiveVariable<float> _energyRegenerationCooldown;

		public void OnInit (Entity entity)
		{
			_currentEnergy                     = entity.CurrentEnergy;
			_maxEnergy                         = entity.MaxEnergy;
			_initialEnergyRegenerationCooldown = entity.InitialEnergyRegenerationCooldown;
			_energyRegenerationCooldown        = entity.CurrentEnergyRegenerationCooldown;
		}

		public void OnUpdate (float deltaTime)
		{
			if (_energyRegenerationCooldown.Value > 0)
			{
				_energyRegenerationCooldown.Value -= deltaTime;
				return;
			}

			_currentEnergy.Value = Mathf.Min(_maxEnergy.Value, _currentEnergy.Value + _maxEnergy.Value * 0.1f);
			Debug.Log($"Remaining energy: {_currentEnergy.Value}");
			_energyRegenerationCooldown.Value =  _initialEnergyRegenerationCooldown.Value;
		}
	}
}

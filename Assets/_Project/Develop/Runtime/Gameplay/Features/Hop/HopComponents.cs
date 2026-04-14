using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hop
{
	public class CurrentEnergy : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class MaxEnergy : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class EnergyUsage : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class InitialEnergyRegenerationCooldown : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class CurrentEnergyRegenerationCooldown : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class HopRange : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class HopRequest : IEntityComponent
	{
		public ReactiveEvent<Vector3> Value;
	}

	public class HopEvent : IEntityComponent
	{
		public ReactiveEvent Value;
	}

	public class CanHop : IEntityComponent
	{
		public ICompositeCondition Value;
	}
}

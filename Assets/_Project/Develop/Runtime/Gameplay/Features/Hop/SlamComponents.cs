using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.Hop
{
	public class SlamDamage : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class SlamRange : IEntityComponent
	{
		public ReactiveVariable<float> Value;
	}

	public class SlamEvent : IEntityComponent
	{
		public ReactiveEvent Value;
	}
}

using System.Collections.Generic;
using System.Linq;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.ApplyDamage;
using Assets._Project.Develop.Runtime.Gameplay.Features.LifeCycle;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class LowestHealthTargetSelector : ITargetSelector
	{
		private Entity _source;

		public LowestHealthTargetSelector(Entity entity)
		{
			_source = entity;
		}

		public Entity SelectTargetFrom (IEnumerable<Entity> targets)
		{
			IEnumerable<Entity> selectedTargets = targets.Where(target =>
			{
				bool result = target.HasComponent<CurrentHealth>();

				result = result && (target != _source);

				return result;
			});

			if (selectedTargets.Any() == false)
				return null;

			Entity lowestHealthTarget = selectedTargets.First();
			float  minHealth          = lowestHealthTarget.CurrentHealth.Value;

			foreach (Entity target in selectedTargets)
			{
				float health = target.CurrentHealth.Value;

				if (health < minHealth)
				{
					minHealth          = health;
					lowestHealthTarget = target;
				}
			}

			return lowestHealthTarget;
		}
	}
}

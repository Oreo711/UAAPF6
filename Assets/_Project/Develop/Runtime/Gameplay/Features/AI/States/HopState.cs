using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.StateMachineCore;
using Assets._Project.Develop.Runtime.Utilities.Timer;


namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI.States
{
	public class HopState : State, IUpdatableState
	{
		private ReactiveEvent _hopRequest;

		public HopState (Entity entity)
		{
			_hopRequest = entity.HopRequest;
		}

		public override void Enter ()
		{
			base.Enter();

			_hopRequest?.Invoke();
		}

		public void Update (float deltaTime)
		{

		}
	}
}

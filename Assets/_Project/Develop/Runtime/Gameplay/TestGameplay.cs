using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using UnityEngine;


namespace Assets._Project.Develop.Runtime.Gameplay
{
    public class TestGameplay : MonoBehaviour
    {
        private DIContainer _container;
        private EntitiesFactory _entitiesFactory;
        private BrainsFactory _brainsFactory;

        private Entity _hopper;
        private Entity _aggressiveHopper;
        private Entity _hero;

        private bool _isRunning;

        public void Initialize(DIContainer container)
        {
            _container = container;
            _entitiesFactory = _container.Resolve<EntitiesFactory>();
            _brainsFactory = _container.Resolve<BrainsFactory>();
        }

        public void Run()
        {
            _hero = _entitiesFactory.CreateHero(new Vector3(-4, 0, 0));
            _brainsFactory.CreateManualHeroBrain(_hero);

            _hopper = _entitiesFactory.CreateHopper(Vector3.zero);
            _brainsFactory.CreateHopperBrain(_hopper);

            _aggressiveHopper = _entitiesFactory.CreateHopper(new Vector3(4, 0, 0));
            _brainsFactory.CreateAggressiveHopperBrain(_aggressiveHopper);

            _isRunning = true;
        }

        private void Update()
        {
            if (_isRunning == false)
                return;
        }
    }
}

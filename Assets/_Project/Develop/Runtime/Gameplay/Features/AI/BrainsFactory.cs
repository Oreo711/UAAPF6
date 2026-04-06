using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.Features.AI.States;
using Assets._Project.Develop.Runtime.Gameplay.Features.InputFeature;
using Assets._Project.Develop.Runtime.Infrastructure.DI;
using Assets._Project.Develop.Runtime.Utilities.Conditions;
using Assets._Project.Develop.Runtime.Utilities.Reactive;
using Assets._Project.Develop.Runtime.Utilities.Timer;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.AI
{
    public class BrainsFactory
    {
        private readonly DIContainer _container;
        private readonly TimerServiceFactory _timerServiceFactory;
        private readonly AIBrainsContext _brainsContext;
        private readonly IInputService _inputService;
        private readonly EntitiesLifeContext _entitiesLifeContext;

        public BrainsFactory(DIContainer container)
        {
            _container = container;
            _timerServiceFactory = _container.Resolve<TimerServiceFactory>();
            _brainsContext = _container.Resolve<AIBrainsContext>();
            _inputService = _container.Resolve<IInputService>();
            _entitiesLifeContext = _container.Resolve<EntitiesLifeContext>();
        }

        public StateMachineBrain CreateMainHeroBrain(Entity entity, ITargetSelector targetSelector)
        {
            AIStateMachine combatState = CreateAutoAttackStateMachine(entity);

            PlayerInputMovementState movementState = new PlayerInputMovementState(entity, _inputService);

            ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

            ICompositeCondition fromMovementToCombatStateCondition = new CompositeCondition()
                .Add(new FuncCondition(() => currentTarget.Value != null))
                .Add(new FuncCondition(() => _inputService.Direction == Vector3.zero));

            ICompositeCondition fromCombatToMovementStateCondition = new CompositeCondition(LogicOperations.Or)
                .Add(new FuncCondition(() => currentTarget.Value == null))
                .Add(new FuncCondition(() => _inputService.Direction != Vector3.zero));

            AIStateMachine behaviour = new AIStateMachine();

            behaviour.AddState(movementState);
            behaviour.AddState(combatState);

            behaviour.AddTransition(movementState, combatState, fromMovementToCombatStateCondition);
            behaviour.AddTransition(combatState, movementState, fromCombatToMovementStateCondition);

            FindTargetState findTargetState = new FindTargetState(targetSelector, _entitiesLifeContext, entity);
            AIParallelState parallelState = new AIParallelState(findTargetState, behaviour);

            AIStateMachine rootStateMachine = new AIStateMachine();
            rootStateMachine.AddState(parallelState);

            StateMachineBrain brain = new StateMachineBrain(rootStateMachine);
            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        public StateMachineBrain CreateGhostBrain(Entity entity)
        {
            AIStateMachine stateMachine = CreateRandomMovementStateMachine(entity);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        private AIStateMachine CreateRandomMovementStateMachine(Entity entity)
        {
            List<IDisposable> disposables = new List<IDisposable>();

            RandomMovementState randomMovementState = new RandomMovementState(entity, 0.5f);

            EmptyState emptyState = new EmptyState();

            TimerService movementTimer = _timerServiceFactory.Create(2f);
            disposables.Add(movementTimer);
            disposables.Add(randomMovementState.Entered.Subscribe(movementTimer.Restart));

            TimerService idleTimer = _timerServiceFactory.Create(3f);
            disposables.Add(idleTimer);
            disposables.Add(emptyState.Entered.Subscribe(idleTimer.Restart));

            FuncCondition movementTimerEndedCondition = new FuncCondition(() => movementTimer.IsOver);
            FuncCondition idleTimerEndedCondition = new FuncCondition(() => idleTimer.IsOver);

            AIStateMachine stateMachine = new AIStateMachine(disposables);

            stateMachine.AddState(randomMovementState);
            stateMachine.AddState(emptyState);

            stateMachine.AddTransition(randomMovementState, emptyState, movementTimerEndedCondition);
            stateMachine.AddTransition(emptyState, randomMovementState, idleTimerEndedCondition);

            return stateMachine;
        }

        private AIStateMachine CreateAutoAttackStateMachine(Entity entity)
        {
            RotateToTargetState rotateToTargetState = new RotateToTargetState(entity);

            AttackTriggerState attackTriggerState = new AttackTriggerState(entity);

            ICondition canAttack = entity.CanStartAttack;
            Transform transform = entity.Transform;
            ReactiveVariable<Entity> currentTarget = entity.CurrentTarget;

            ICompositeCondition fromRotateToAttackCondition = new CompositeCondition()
                .Add(canAttack)
                .Add(new FuncCondition(() =>
                {
                    Entity target = currentTarget.Value;

                    if (target == null)
                        return false;

                    float angleToTarget = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(target.Transform.position - transform.position));
                    return angleToTarget < 1f;
                }));

            ReactiveVariable<bool> inAttackProcess = entity.InAttackProcess;

            ICondition fromAttackToRotateStateCondition = new FuncCondition(() => inAttackProcess.Value == false);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(rotateToTargetState);
            stateMachine.AddState(attackTriggerState);

            stateMachine.AddTransition(rotateToTargetState, attackTriggerState, fromRotateToAttackCondition);
            stateMachine.AddTransition(attackTriggerState, rotateToTargetState, fromAttackToRotateStateCondition);

            return stateMachine;
        }

        public StateMachineBrain CreateManualHeroBrain(Entity entity)
        {
            AIStateMachine stateMachine = CreateManualHeroStateMachine(entity);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        private AIStateMachine CreateManualHeroStateMachine (Entity entity)
        {
            AIStateMachine           combatState   = CreateManualCombatStateMachine(entity);
            PlayerInputMovementState movementState = new PlayerInputMovementState(entity, _inputService);

            ICondition mustEngageCombat    = new FuncCondition(() => _inputService.Direction == Vector3.zero);
            ICondition mustDisengageCombat = new FuncCondition(() => _inputService.Direction != Vector3.zero);

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(movementState);
            stateMachine.AddState(combatState);

            stateMachine.AddTransition(movementState, combatState, mustEngageCombat);
            stateMachine.AddTransition(combatState, movementState, mustDisengageCombat);

            return stateMachine;
        }

        private AIStateMachine CreateManualCombatStateMachine (Entity entity)
        {
            PlayerInputRotationState rotationState = new PlayerInputRotationState(entity, _inputService);
            AttackTriggerState       attackState   = new AttackTriggerState(entity);

            ICompositeCondition mustAttack = new CompositeCondition()
                .Add(new FuncCondition(() => entity.MoveDirection.Value == Vector3.zero))
                .Add(new FuncCondition(() => _inputService.Holding));

            ICompositeCondition mustStopAttacking = new CompositeCondition()
                .Add(new FuncCondition(() => entity.MoveDirection.Value == Vector3.zero))
                .Add(new FuncCondition(() => !_inputService.Holding));

            AIStateMachine stateMachine = new AIStateMachine();

            stateMachine.AddState(rotationState);
            stateMachine.AddState(attackState);

            stateMachine.AddTransition(rotationState, attackState, mustAttack);
            stateMachine.AddTransition(attackState, rotationState, mustStopAttacking);

            return stateMachine;
        }

        public StateMachineBrain CreateHopperBrain (Entity entity)
        {
            AIStateMachine stateMachine = CreateHopStateMachine(entity);
            StateMachineBrain brain = new StateMachineBrain(stateMachine);

            _brainsContext.SetFor(entity, brain);

            return brain;
        }

        private AIStateMachine CreateHopStateMachine (Entity entity)
        {
            List<IDisposable> disposables = new List<IDisposable>();

            HopState   hopState   = new HopState(entity);
            EmptyState emptyState = new EmptyState();

            TimerService timer = _timerServiceFactory.Create(3f);
            disposables.Add(timer);
            disposables.Add(emptyState.Entered.Subscribe(timer.Restart));

            ICompositeCondition canHop = new CompositeCondition()
                .Add(new FuncCondition(() => timer.IsOver))
                .Add(new FuncCondition(() => entity.CanHop.Evaluate()))
                .Add(new FuncCondition(() => entity.CurrentEnergy.Value >= entity.EnergyUsage.Value * 2));

            bool hopEventInvoked = false;

            disposables.Add(entity.HopEvent.Subscribe(() => hopEventInvoked = true));

            ICondition hasHopped = new FuncCondition(() =>
            {
                bool result = hopEventInvoked;
                hopEventInvoked = false;
                return result;
            });

            AIStateMachine stateMachine = new AIStateMachine(disposables);

            stateMachine.AddState(emptyState);
            stateMachine.AddState(hopState);

            stateMachine.AddTransition(emptyState, hopState, canHop);
            stateMachine.AddTransition(hopState, emptyState, hasHopped);

            return stateMachine;
        }
    }
}

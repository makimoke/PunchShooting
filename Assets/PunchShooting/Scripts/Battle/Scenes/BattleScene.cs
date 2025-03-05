using System;
using Cysharp.Threading.Tasks;
using IceMilkTea.StateMachine;
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Views.Player;
using UnityEngine;
using VContainer;

namespace PunchShooting.Battle.Scenes
{
    public class BattleScene : MonoBehaviour
    {
        private PlayerResourceProvider _playerResourceProvider;
        private PlayerShipViewController _playerShipViewController;
        private StageStatusDataAccessor _stageStatusDataAccessor;
        private ImtStateMachine<BattleScene> _stateMachine;

        private void Awake()
        {
            _stateMachine = new ImtStateMachine<BattleScene>(this);
            _stateMachine.AddTransition<LoadState, ResetState>((int)StateEventId.Finish);
            _stateMachine.AddTransition<ResetState, StandbyState>((int)StateEventId.Finish);
            _stateMachine.AddTransition<StandbyState, PlayState>((int)StateEventId.Play);
            _stateMachine.AddTransition<PlayState, GameOverState>((int)StateEventId.Miss);
            _stateMachine.AddTransition<PlayState, StageClearState>((int)StateEventId.Finish);
            _stateMachine.AddTransition<GameOverState, ResetState>((int)StateEventId.Retry);
            _stateMachine.AddTransition<StageClearState, ResetState>((int)StateEventId.Finish);
            _stateMachine.SetStartState<LoadState>();
        }

        private void Start()
        {
        }

        private void Update()
        {
            _stateMachine.Update();
        }

        [Inject]
        public void Construct(StageStatusDataAccessor stageStatusDataAccessor,
            PlayerResourceProvider playerResourceProvider,
            PlayerShipViewController playerShipViewController)
        {
            _stageStatusDataAccessor = stageStatusDataAccessor;
            _playerResourceProvider = playerResourceProvider;
            _playerShipViewController = playerShipViewController;
        }

        //TODO:Actionを別の方式にする？
        private async UniTask WaitSecondsAction(float seconds, Action action)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(seconds));
            action.Invoke();
        }

        //TODO:Actionを別の方式にする？
        private async UniTask WaitLoadResource(Action action)
        {
            await _playerResourceProvider.LoadAsync();
            _playerShipViewController.Initialize();
            action.Invoke();
        }


        private enum StateEventId
        {
            Play,
            Miss,
            Retry,
            Exit,
            Finish
        }

        private class LoadState : ImtStateMachine<BattleScene, int>.State
        {
            protected override void Enter()
            {
                Context.WaitLoadResource(() => StateMachine.SendEvent((int)StateEventId.Finish)).Forget();
            }
        }

        private class ResetState : ImtStateMachine<BattleScene, int>.State
        {
            protected override void Enter()
            {
                StateMachine.SendEvent((int)StateEventId.Finish);
            }
        }

        private class StandbyState : ImtStateMachine<BattleScene, int>.State
        {
            protected override void Enter()
            {
                Context._stageStatusDataAccessor.Status = StageStatusDataAccessor.StageStatus.Ready;
            }

            protected override void Update()
            {
                //if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                {
                    StateMachine.SendEvent((int)StateEventId.Play);
                }
            }
        }

        private class PlayState : ImtStateMachine<BattleScene, int>.State
        {
            protected override void Enter()
            {
                Context._stageStatusDataAccessor.Status = StageStatusDataAccessor.StageStatus.Playing;
            }

            protected override void Update()
            {
                Context._playerShipViewController.Update(Time.deltaTime);

                //Context._stageEnemyGenerator.Update(Time.deltaTime);

                //Context._playerStatusDataHolder.ReflectDamage();

                /*if (Context._stageStatusDataHolder.LivingEnemyCount == 0 && Context._stageEnemyGenerator.IsCompleted)
                {
                    StateMachine.SendEvent((int)StateEventId.Finish);
                }
                else if (Context._playerStatusDataHolder.Status.isDead)
                {
                    //Context._enemyCreator.DestoryAllEnemies();
                    StateMachine.SendEvent((int)StateEventId.Miss);
                }*/
            }
        }

        private class GameOverState : ImtStateMachine<BattleScene, int>.State
        {
            protected override void Enter()
            {
                Context._stageStatusDataAccessor.Status = StageStatusDataAccessor.StageStatus.GameOver;
                Context.WaitSecondsAction(1.0f,
                    () => { StateMachine.SendEvent((int)StateEventId.Retry); }).Forget();
            }
        }

        private class StageClearState : ImtStateMachine<BattleScene, int>.State
        {
            protected override void Enter()
            {
                Context._stageStatusDataAccessor.Status = StageStatusDataAccessor.StageStatus.StageClear;
                Context.WaitSecondsAction(1.0f,
                    () => { StateMachine.SendEvent((int)StateEventId.Finish); }).Forget();
            }
        }
    }
}
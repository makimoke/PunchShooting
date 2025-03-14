using System;
using Cysharp.Threading.Tasks;
using IceMilkTea.StateMachine;
using PunchShooting.Battle.Calculators;
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Enemy;
using PunchShooting.Battle.Data.Player;
using PunchShooting.Battle.Definitions.Player;
using PunchShooting.Battle.Logic.Enemy;
using PunchShooting.Battle.Logic.Player;
using PunchShooting.Battle.Views.Enemy;
using PunchShooting.Battle.Views.Player;
using R3;
using UnityEngine;
using VContainer;

namespace PunchShooting.Battle.Scenes
{
    public class BattleScene : MonoBehaviour
    {
        //TODO 後で移動する
        private const float ShotTimeInterval = 1.0f; //弾発射間隔
        private float _bulletCounter = ShotTimeInterval;
        private DisposableBag _disposableBag;
        private EnemiesViewController _enemiesViewController;
        private EnemyBaseParamDataAccessor _enemyBaseParamDataAccessor;
        private EnemyResourceProvider _enemyResourceProvider;
        private EnemyStatusDataAccessor _enemyStatusDataAccessor;
        private EnemyStatusLogic _enemyStatusLogic;
        private PlayerBulletBaseParamDataAccessor _playerBulletBaseParamDataAccessor;
        private PlayerBulletStatusDataAccessor _playerBulletStatusDataAccessor;
        private PlayerBulletStatusLogic _playerBulletStatusLogic;
        private PlayerResourceProvider _playerResourceProvider;
        private PlayerShipViewController _playerShipViewController;
        private PlayerStatusDataAccessor _playerStatusDataAccessor;
        private StageEnemyGenerator _stageEnemyGenerator;
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

        private void OnDestroy()
        {
            _disposableBag.Dispose();
        }

        [Inject]
        public void Construct(StageStatusDataAccessor stageStatusDataAccessor,
            PlayerBulletBaseParamDataAccessor playerBulletBaseParamDataAccessor,
            PlayerBulletStatusDataAccessor playerBulletStatusDataAccessor,
            PlayerStatusDataAccessor playerStatusDataAccessor,
            EnemyBaseParamDataAccessor enemyBaseParamDataAccessor,
            EnemyStatusDataAccessor enemyStatusDataAccessor,
            PlayerResourceProvider playerResourceProvider,
            EnemyResourceProvider enemyResourceProvider,
            PlayerShipViewController playerShipViewController,
            EnemiesViewController enemiesViewController,
            PlayerBulletStatusLogic playerBulletStatusLogic,
            EnemyStatusLogic enemyStatusLogic,
            StageEnemyGenerator stageEnemyGenerator)
        {
            _stageStatusDataAccessor = stageStatusDataAccessor;
            _playerBulletBaseParamDataAccessor = playerBulletBaseParamDataAccessor;
            _playerBulletStatusDataAccessor = playerBulletStatusDataAccessor;
            _playerStatusDataAccessor = playerStatusDataAccessor;
            _enemyBaseParamDataAccessor = enemyBaseParamDataAccessor;
            _enemyStatusDataAccessor = enemyStatusDataAccessor;
            _playerResourceProvider = playerResourceProvider;
            _enemyResourceProvider = enemyResourceProvider;
            _playerShipViewController = playerShipViewController;
            _enemiesViewController = enemiesViewController;
            _playerBulletStatusLogic = playerBulletStatusLogic;
            _enemyStatusLogic = enemyStatusLogic;
            _stageEnemyGenerator = stageEnemyGenerator;
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
            await _enemyResourceProvider.LoadAsync();
            await _playerBulletBaseParamDataAccessor.LoadAsync();
            await _enemyBaseParamDataAccessor.LoadAsync();
            await _stageEnemyGenerator.LoadAsync();
            _playerShipViewController.Initialize();
            _enemiesViewController.Initialize();
            _playerShipViewController.OnCollidedBulletSubject
                .Subscribe(collisionResult =>
                {
                    Debug.Log($"OnCollidedBulletSubject: SourceId={collisionResult.SourceId} OpponentId={collisionResult.OpponentId}");
                    var playerBulletStatus = _playerBulletStatusDataAccessor.FindStatus(collisionResult.SourceId);
                    var enemyStatus = _enemyStatusDataAccessor.FindStatus(collisionResult.OpponentId);
                    DamageCalculator.AddDamage(playerBulletStatus, enemyStatus);
                })
                .AddTo(ref _disposableBag);
            _playerShipViewController.OnDestroyedBulletSubject
                .Subscribe(instanceId => _playerBulletStatusLogic.RemoveBullet(instanceId))
                .AddTo(ref _disposableBag);
            _enemiesViewController.OnCollidedEnemySubject
                .Subscribe(collisionResult =>
                {
                    Debug.Log($"OnCollidedEnemySubject: SourceId={collisionResult.SourceId} OpponentId={collisionResult.OpponentId}");
                    var enemyStatus = _enemyStatusDataAccessor.FindStatus(collisionResult.SourceId);
                    var playerStatus = _playerStatusDataAccessor.Status;
                    DamageCalculator.AddDamage(enemyStatus, playerStatus);
                })
                .AddTo(ref _disposableBag);
            _enemiesViewController.OnDestroyedEnemySubject
                .Subscribe(instanceId => _enemyStatusLogic.RemoveEnemy(instanceId))
                .AddTo(ref _disposableBag);
            _stageEnemyGenerator.OnCreateEnemySubject
                .Subscribe(param => CreateEnemy(param))
                .AddTo(ref _disposableBag);
            _enemyStatusLogic.OnDamageSubject
                .Subscribe(objectStatus => _enemiesViewController.ReceivedDamage(objectStatus.InstanceId, objectStatus.Damage))
                .AddTo(ref _disposableBag);
            _enemyStatusLogic.OnDeadSubject
                .Subscribe(instanceId => _enemiesViewController.DestroyEnemy(instanceId))
                .AddTo(ref _disposableBag);

            action.Invoke();
        }

        //自動で弾を発射する
        private void FirePlayerBulletAutomatically(float deltaTime)
        {
            _bulletCounter -= deltaTime;
            if (_bulletCounter <= 0.0f)
            {
                var baseParam = _playerBulletBaseParamDataAccessor.FindBaseParam(PlayerBulletBaseParamDefinition.ParamId.PBul001);
                var objectStatus = _playerBulletStatusLogic.CreateBullet(baseParam);

                _playerShipViewController.CreateBullet(objectStatus.InstanceId);

                _bulletCounter = ShotTimeInterval;
            }
        }

        private void CreateEnemy(StageEnemyCreateParam stageEnemyCreateParam)
        {
            var baseParam = _enemyBaseParamDataAccessor.FindBaseParam(stageEnemyCreateParam.Id);
            var objectStatus = _enemyStatusLogic.CreateEnemy(baseParam);
            _enemiesViewController.CreateEnemy(objectStatus.InstanceId, baseParam.PrefabId, baseParam.SpriteId, stageEnemyCreateParam.Position);
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
                Context._playerStatusDataAccessor.Reset();
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
                Context._enemyStatusLogic.ProcessDamage();
                
                Context._playerShipViewController.Update(Time.deltaTime);
                Context._enemiesViewController.Update(Time.deltaTime);

                //時間で弾発射
                Context.FirePlayerBulletAutomatically(Time.deltaTime);

                Context._stageEnemyGenerator.Update(Time.deltaTime);

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
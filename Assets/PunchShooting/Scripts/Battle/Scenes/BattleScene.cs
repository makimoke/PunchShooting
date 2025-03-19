using System;
using Cysharp.Threading.Tasks;
using IceMilkTea.StateMachine;
using PunchShooting.Battle.Calculators;
using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Enemy;
using PunchShooting.Battle.Data.Player;
using PunchShooting.Battle.Definitions.Player;
using PunchShooting.Battle.Logics.Enemy;
using PunchShooting.Battle.Logics.Player;
using PunchShooting.Battle.Views;
using PunchShooting.Battle.Views.Enemy;
using PunchShooting.Battle.Views.Player;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace PunchShooting.Battle.Scenes
{
    public class BattleScene : MonoBehaviour
    {
        private Vector2 _currentLookInputValue = Vector2.zero;
        private DisposableBag _disposableBag;
        private EnemiesViewController _enemiesViewController;
        private EnemyResourceProvider _enemyResourceProvider;
        private EnemySettingsDataAccessor _enemySettingsDataAccessor;
        private EnemyStatusDataAccessor _enemyStatusDataAccessor;
        private EnemyStatusLogic _enemyStatusLogic;
        private PlayerBulletSettingsDataAccessor _playerBulletSettingsDataAccessor;
        private PlayerBulletStatusDataAccessor _playerBulletStatusDataAccessor;
        private PlayerBulletStatusLogic _playerBulletStatusLogic;
        private PlayerBulletsViewController _playerBulletsViewController;
        private PlayerInput _playerInput;
        private PlayerResourceProvider _playerResourceProvider;
        private PlayerScoreLogic _playerScoreLogic;
        private PlayerShipViewController _playerShipViewController;
        private PlayerStatusDataAccessor _playerStatusDataAccessor;
        private PlayerStatusLogic _playerStatusLogic;
        private StageEnemyGenerator _stageEnemyGenerator;
        private StageStatusDataAccessor _stageStatusDataAccessor;
        private StageStatusViewController _stageStatusViewController;
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
            _playerInput.actions["Right Weapon"].started += OnLook;
            _playerInput.actions["Right Weapon"].performed += OnLook;
            //_playerInput.actions["Right Weapon"].canceled += OnLook;
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
            PlayerBulletSettingsDataAccessor playerBulletSettingsDataAccessor,
            PlayerBulletStatusDataAccessor playerBulletStatusDataAccessor,
            PlayerStatusDataAccessor playerStatusDataAccessor,
            EnemySettingsDataAccessor enemySettingsDataAccessor,
            EnemyStatusDataAccessor enemyStatusDataAccessor,
            PlayerResourceProvider playerResourceProvider,
            EnemyResourceProvider enemyResourceProvider,
            PlayerShipViewController playerShipViewController,
            PlayerBulletsViewController playerBulletsViewController,
            EnemiesViewController enemiesViewController,
            StageStatusViewController stageStatusViewController,
            PlayerStatusLogic playerStatusLogic,
            PlayerScoreLogic playerScoreLogic,
            PlayerBulletStatusLogic playerBulletStatusLogic,
            EnemyStatusLogic enemyStatusLogic,
            StageEnemyGenerator stageEnemyGenerator,
            PlayerInput playerInput)
        {
            _stageStatusDataAccessor = stageStatusDataAccessor;
            _playerBulletSettingsDataAccessor = playerBulletSettingsDataAccessor;
            _playerBulletStatusDataAccessor = playerBulletStatusDataAccessor;
            _playerStatusDataAccessor = playerStatusDataAccessor;
            _enemySettingsDataAccessor = enemySettingsDataAccessor;
            _enemyStatusDataAccessor = enemyStatusDataAccessor;
            _playerResourceProvider = playerResourceProvider;
            _enemyResourceProvider = enemyResourceProvider;
            _playerShipViewController = playerShipViewController;
            _playerBulletsViewController = playerBulletsViewController;
            _enemiesViewController = enemiesViewController;
            _stageStatusViewController = stageStatusViewController;
            _playerStatusLogic = playerStatusLogic;
            _playerScoreLogic = playerScoreLogic;
            _playerBulletStatusLogic = playerBulletStatusLogic;
            _enemyStatusLogic = enemyStatusLogic;
            _stageEnemyGenerator = stageEnemyGenerator;
            _playerInput = playerInput;
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
            await _playerBulletSettingsDataAccessor.LoadAsync();
            await _enemySettingsDataAccessor.LoadAsync();
            await _stageEnemyGenerator.LoadAsync();
            _playerShipViewController.Initialize();
            _playerBulletsViewController.Initialize();
            _enemiesViewController.Initialize();
            _stageStatusViewController.Initialize();
            _playerBulletsViewController.OnCollidedBulletSubject
                .Subscribe(collisionResult =>
                {
                    Debug.Log($"OnCollidedBulletSubject: SourceId={collisionResult.SourceId} OpponentId={collisionResult.OpponentId}");
                    var playerBulletStatus = _playerBulletStatusDataAccessor.FindStatus(collisionResult.SourceId);
                    var enemyStatus = _enemyStatusDataAccessor.FindStatus(collisionResult.OpponentId);
                    DamageCalculator.AddDamage(playerBulletStatus, enemyStatus);
                })
                .AddTo(ref _disposableBag);
            _playerBulletsViewController.OnDestroyedBulletSubject
                .Subscribe(instanceId => _playerBulletStatusLogic.RemoveBullet(instanceId))
                .AddTo(ref _disposableBag);
            _enemiesViewController.OnCollidedSubject
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
                .Subscribe(objectStatus =>
                {
                    _enemiesViewController.ReceivedDamage(objectStatus.InstanceId, objectStatus.Damage);
                    if (objectStatus.isDead)
                    {
                        _playerScoreLogic.AddScore(objectStatus.Score);
                    }
                })
                .AddTo(ref _disposableBag);
            _enemyStatusLogic.OnDeadSubject
                .Subscribe(instanceId => _enemiesViewController.DestroyEnemy(instanceId))
                .AddTo(ref _disposableBag);
            _playerBulletStatusLogic.OnDamageSubject
                .Subscribe(objectStatus => _playerBulletsViewController.ReceivedDamage(objectStatus.InstanceId, objectStatus.Damage))
                .AddTo(ref _disposableBag);
            _playerBulletStatusLogic.OnDeadSubject
                .Subscribe(instanceId => _playerBulletsViewController.DestroyBullet(instanceId))
                .AddTo(ref _disposableBag);
            _playerStatusLogic.OnDamageSubject
                .Subscribe(objectStatus => _playerShipViewController.ReceivedDamage(objectStatus.Damage))
                .AddTo(ref _disposableBag);
            _playerStatusLogic.OnDeadSubject
                .Subscribe(_ => _stateMachine.SendEvent((int)StateEventId.Miss))
                .AddTo(ref _disposableBag);

            action.Invoke();
        }

        //自動で弾を発射する
        private void FirePlayerBulletAutomatically(float deltaTime)
        {
            //左武器
            var settings = _playerBulletSettingsDataAccessor.FindSettings(PlayerBulletSettingsDefinition.ParamId.PBul001);
            if (_playerStatusLogic.Cooldown(PlayerWeaponDefinition.WeaponIndex.Left, deltaTime, settings.CoolTime))
            {
                var objectStatus = _playerBulletStatusLogic.CreateBullet(settings);
                _playerBulletsViewController.CreateBullet(objectStatus.InstanceId, settings.PrefabId, settings.SpriteId, settings.Position + _playerShipViewController.Position,
                    new Vector3(0.0f, 4.0f, 0.0f));
            }

            //右武器
            settings = _playerBulletSettingsDataAccessor.FindSettings(PlayerBulletSettingsDefinition.ParamId.PBul002);
            if (_playerStatusLogic.Cooldown(PlayerWeaponDefinition.WeaponIndex.Right, deltaTime, settings.CoolTime))
            {
                var velocity = Vector3.up * 4.0f;
                var inputLookAxis = _currentLookInputValue;
                if (inputLookAxis.sqrMagnitude != 0.0f && inputLookAxis.x >= 0.0f)
                {
                    //左側には撃てない
                    velocity = inputLookAxis.normalized * 4.0f;
                }

                Debug.Log($"inputLookAxis={inputLookAxis}");

                var objectStatus = _playerBulletStatusLogic.CreateBullet(settings);
                _playerBulletsViewController.CreateBullet(objectStatus.InstanceId, settings.PrefabId, settings.SpriteId, settings.Position + _playerShipViewController.Position, velocity);
            }
        }

        public void OnLook(InputAction.CallbackContext context)
        {
            _currentLookInputValue = context.ReadValue<Vector2>();
            Debug.Log($"OnLook:{_currentLookInputValue}");
        }


        private void CreateEnemy(StageEnemyCreateParam stageEnemyCreateParam)
        {
            var baseParam = _enemySettingsDataAccessor.FindSettings(stageEnemyCreateParam.Id);
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
                //オブジェクト削除
                Context._enemiesViewController.DestroyAllEnemis();
                Context._enemyStatusLogic.RemoveAllEnemis();
                Context._playerBulletsViewController.DestroyAllBullets();
                Context._playerBulletStatusLogic.RemoveAllBullets();
                //リセット
                Context._playerStatusDataAccessor.Reset();
                Context._stageEnemyGenerator.Reset();
                Context._playerScoreLogic.Reset();
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
                Context._playerStatusLogic.ProcessDamage();
                Context._playerBulletStatusLogic.ProcessDamage();

                var deltaTime = Time.deltaTime;
                Context._playerShipViewController.Update(deltaTime);
                Context._playerBulletsViewController.Update(deltaTime);
                Context._enemiesViewController.Update(deltaTime);

                //時間で弾発射
                Context.FirePlayerBulletAutomatically(deltaTime);

                Context._stageEnemyGenerator.Update(deltaTime);

                if (Context._enemyStatusDataAccessor.EnemiesCount == 0 && Context._stageEnemyGenerator.IsCompleted)
                {
                    //敵全滅
                    StateMachine.SendEvent((int)StateEventId.Finish);
                }
                else if (Context._playerStatusDataAccessor.Status.isDead)
                {
                    //プレイヤ死亡
                    StateMachine.SendEvent((int)StateEventId.Miss);
                }
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
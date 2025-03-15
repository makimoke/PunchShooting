using PunchShooting.Battle.Data;
using PunchShooting.Battle.Data.Enemy;
using PunchShooting.Battle.Data.Player;
using PunchShooting.Battle.Logic;
using PunchShooting.Battle.Logic.Enemy;
using PunchShooting.Battle.Logic.Player;
using PunchShooting.Battle.Scenes;
using PunchShooting.Battle.Views;
using PunchShooting.Battle.Views.Enemy;
using PunchShooting.Battle.Views.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using VContainer;
using VContainer.Unity;

namespace PunchShooting.Battle.Systems
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private BattleScene battleScene;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private BattleFieldView battleFieldView;
        [FormerlySerializedAs("stageStatusView")] [SerializeField] private StatusBarView statusBarView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(battleScene);
            builder.RegisterComponent(playerInput);
            builder.RegisterComponent(battleFieldView);
            builder.RegisterComponent(statusBarView);
            //View
            builder.Register<PlayerResourceProvider>(Lifetime.Singleton);
            builder.Register<PlayerShipViewController>(Lifetime.Singleton);
            builder.Register<PlayerBulletsViewController>(Lifetime.Singleton);
            builder.Register<PlayerBulletViewCreator>(Lifetime.Singleton);
            builder.Register<EnemyResourceProvider>(Lifetime.Singleton);
            builder.Register<EnemiesViewController>(Lifetime.Singleton);
            builder.Register<EnemyViewCreator>(Lifetime.Singleton);
            builder.Register<StageStatusViewController>(Lifetime.Singleton);
            //Data
            builder.Register<PlayerScoreDataAccessor>(Lifetime.Singleton);
            builder.Register<PlayerStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<PlayerBulletBaseParamDataAccessor>(Lifetime.Singleton);
            builder.Register<StageStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<PlayerBulletStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<EnemyStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<EnemyBaseParamDataAccessor>(Lifetime.Singleton);
            //Logic
            builder.Register<InstanceIdGenerator>(Lifetime.Singleton);
            builder.Register<PlayerStatusLogic>(Lifetime.Singleton);
            builder.Register<PlayerScoreLogic>(Lifetime.Singleton);
            builder.Register<PlayerBulletStatusLogic>(Lifetime.Singleton);
            builder.Register<EnemyStatusLogic>(Lifetime.Singleton);
            builder.Register<StageEnemyGenerator>(Lifetime.Singleton);
        }
    }
}
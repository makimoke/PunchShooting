using PunchShooting.Battle.Data;
using PunchShooting.Battle.Logic;
using PunchShooting.Battle.Scenes;
using PunchShooting.Battle.Views;
using PunchShooting.Battle.Views.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace PunchShooting.Battle.Systems
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private BattleScene battleScene;
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private BattleFieldView battleFieldView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(battleScene);
            builder.RegisterComponent(playerInput);
            builder.RegisterComponent(battleFieldView);
            builder.Register<PlayerResourceProvider>(Lifetime.Singleton);
            builder.Register<PlayerShipViewController>(Lifetime.Singleton);
            builder.Register<PlayerBulletViewCreator>(Lifetime.Singleton);
            builder.Register<PlayerScoreDataAccessor>(Lifetime.Singleton);
            builder.Register<PlayerStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<StageStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<PlayerBulletStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<EnemyStatusDataAccessor>(Lifetime.Singleton);
            builder.Register<InstanceIdGenerator>(Lifetime.Singleton);
            builder.Register<PlayerBulletStatusLogic>(Lifetime.Singleton);
        }
    }
}
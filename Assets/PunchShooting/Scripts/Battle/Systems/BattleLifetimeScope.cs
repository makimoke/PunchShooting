using PunchShooting.Battle.Views;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace PunchShooting.Battle.Systems
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerInput playerInput;
        [SerializeField] private BattleFieldView battleFieldView;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerInput);
            builder.RegisterComponent(battleFieldView);
        }
    }
}
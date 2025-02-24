using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace PunchShooting.Battle.Systems
{
    public class BattleLifetimeScope : LifetimeScope
    {
        [SerializeField] private PlayerInput playerInput;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponent(playerInput );
        }
    }
    
}
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace PunchShooting.Battle.Views.Player
{
    //自機
    public class PlayerShipViewController : IDisposable
    {
        private readonly PlayerInput _playerInput;
        private readonly PlayerResourceProvider _playerResourceProvider;
        private PlayerShipView _playerShipView;
        private const float InputMoveScale = 0.04f;

        [Inject]
        public PlayerShipViewController(PlayerResourceProvider playerResourceProvider,
            PlayerInput playerInput)
        {
            _playerResourceProvider = playerResourceProvider;
            _playerInput = playerInput;
        }

        public void Dispose()
        {
        }

        public void Initialize()
        {
            _playerShipView = _playerResourceProvider.InstantiatePlayerShip();
        }

        //毎フレーム呼ばれる
        public void Update()
        {
            var inputMoveAxis = _playerInput.actions["Move"].ReadValue<Vector2>();
            if (inputMoveAxis != Vector2.zero)
            {
                _playerShipView.AddPosition(inputMoveAxis * InputMoveScale);
            }
        }
    }
}
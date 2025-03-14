using System;
using PunchShooting.Battle.Definitions;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

namespace PunchShooting.Battle.Views.Player
{
    //自機
    public class PlayerShipViewController : IDisposable
    {
        //定数
        private const float InputMoveScale = 0.02f;

        private readonly PlayerInput _playerInput;

        private readonly PlayerResourceProvider _playerResourceProvider;
        //public readonly Subject<SpriteCollisionResult> OnCollidedBulletSubject = new(); //弾と敵が衝突した

        //public readonly Subject<long> OnDestroyedBulletSubject = new();

        private DisposableBag _disposableBag;
        private PlayerShipView _playerShipView;


        [Inject]
        public PlayerShipViewController(PlayerResourceProvider playerResourceProvider,
            PlayerInput playerInput)
        {
            _playerResourceProvider = playerResourceProvider;
            _playerInput = playerInput;
        }

        public Vector3 Position => _playerShipView.Position;

        public void Dispose()
        {
            _disposableBag.Dispose();
        }

        public void Initialize()
        {
            _playerShipView = _playerResourceProvider.InstantiatePlayerShip(SpriteResourceDefinition.PrefabId.PShip);
        }

        //毎フレーム呼ばれる
        public void Update(float deltaTime)
        {
            //入力で移動
            var inputMoveAxis = _playerInput.actions["Move"].ReadValue<Vector2>();
            if (inputMoveAxis != Vector2.zero)
            {
                _playerShipView.AddPosition(inputMoveAxis * InputMoveScale);
            }
        }
    }
}
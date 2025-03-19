using PunchShooting.Battle.Definitions.Player;
using UnityEngine;

namespace PunchShooting.Battle.Calculators.Player
{
    public static class PlayerBulletCalculator
    {
        //スティック入力から移動速度算出
        public static Vector2 CollectVelocity(Vector2 inputValue, float speed, PlayerWeaponDefinition.WeaponIndex weaponIndex)
        {
            var velocity = Vector2.up;
            var inputLookAxis = inputValue.normalized;
            if (inputLookAxis.sqrMagnitude >= PlayerWeaponDefinition.StickSquaredThreshold)
            {
                velocity = inputLookAxis;

                if ((weaponIndex == PlayerWeaponDefinition.WeaponIndex.Right && inputLookAxis.x < 0.0f) ||
                    (weaponIndex == PlayerWeaponDefinition.WeaponIndex.Left && inputLookAxis.x > 0.0f))
                {
                    velocity = new Vector2(0, inputLookAxis.y);
                }

                if (velocity == Vector2.zero)
                {
                    velocity = Vector2.up;
                }
            }

            return velocity * speed;
        }
    }
}
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
                if ((weaponIndex == PlayerWeaponDefinition.WeaponIndex.Right && inputLookAxis.x < 0.0f) ||
                    (weaponIndex == PlayerWeaponDefinition.WeaponIndex.Left && inputLookAxis.x > 0.0f))
                {
                    if (inputLookAxis.y < 0.0f)
                    {
                        velocity = Vector2.down;
                    }
                }
                else
                {
                    velocity = inputLookAxis;
                }
            }

            return velocity * speed;
        }
    }
}
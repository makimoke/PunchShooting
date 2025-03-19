namespace PunchShooting.Battle.Definitions.Player
{
    //プレイヤ武器定義
    public static class PlayerWeaponDefinition
    {
        public enum WeaponIndex
        {
            Left,
            Right
        }

        public const int WeaponCount = 2;

        public const float StickSquaredThreshold = 0.1f; //スティックしきい値（２乗）
    }
}
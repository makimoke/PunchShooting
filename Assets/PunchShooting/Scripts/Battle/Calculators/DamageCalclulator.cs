using PunchShooting.Battle.Data;

namespace PunchShooting.Battle.Calculators
{
    public static class DamageCalculator
    {
        // ダメージ加算
        // 戻り値：攻撃値
        public static int AddDamage(ObjectStatus ally, ObjectStatus opponent)
        {
            if (ally.isDead || opponent.isDead) return 0;

            opponent.Damage += ally.Str;
            ally.Damage += opponent.Str;

            return ally.Str;
        }
    }
}

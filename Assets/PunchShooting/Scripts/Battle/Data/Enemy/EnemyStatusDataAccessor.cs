using System.Collections.Generic;

namespace PunchShooting.Battle.Data.Enemy
{
    //敵ステータス
    public class EnemyStatusDataAccessor
    {
        private readonly Dictionary<long, ObjectStatus> _statusDictionary = new();

        public void AddStatus(ObjectStatus status)
        {
            _statusDictionary[status.InstanceId] = status;
        }

        public void RemoveStatus(long instanceId)
        {
            _statusDictionary.Remove(instanceId);
        }

        public ObjectStatus FindStatus(long instanceId)
        {
            return _statusDictionary[instanceId];
        }

        public void RemoveAllStatuses()
        {
            _statusDictionary.Clear();
        }
    }
}
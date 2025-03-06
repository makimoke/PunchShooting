using System.Collections.Generic;

namespace PunchShooting.Battle.Data
{
    //敵ステータス
    public class EnemyStatusDataAccessor
    {
        private readonly Dictionary<long, ObjectStatus> _statusDictionary = new();

        public void AddStatus(long instanceId, ObjectStatus status)
        {
            _statusDictionary[instanceId] = status;
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
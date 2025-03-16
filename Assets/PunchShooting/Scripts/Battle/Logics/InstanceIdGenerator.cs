namespace PunchShooting.Battle.Logics
{
    //インスタンスID生成
    public class InstanceIdGenerator
    {
        private long _lastInstanceId;

        public long GenerateId()
        {
            return ++_lastInstanceId;
        }
    }
}
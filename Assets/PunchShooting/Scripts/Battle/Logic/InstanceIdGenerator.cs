namespace PunchShooting.Battle.Logic
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
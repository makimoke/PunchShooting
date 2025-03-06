namespace PunchShooting.Battle.Views
{
    //スプライト衝突結果 (TODO:良い名前は後で考える)
    public class SpriteCollisionResult
    {
        public SpriteCollisionResult(long sourceId, long opponentId)
        {
            SourceId = sourceId;
            OpponentId = opponentId;
        }

        public long SourceId { get; private set; }
        public long OpponentId { get; private set; }
    }
}
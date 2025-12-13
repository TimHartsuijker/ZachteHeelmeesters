namespace backend.Models
{
    public class LoginAttempt
    {
        public int FailCount { get; set; } = 0;
        public int BlockCount { get; set; } = 0;
        public DateTime? BlockUntil { get; set; } = null;

        private static readonly TimeSpan BlockDuration = TimeSpan.FromMinutes(15);

        public void RegisterFail()
        {
            FailCount++;
        }

        public void Block()
        {
            BlockCount++;
            BlockUntil = DateTime.UtcNow.Add(BlockDuration);
        }

        public bool IsBlocked()
        {
            if (BlockUntil == null) return false;

            if (DateTime.UtcNow >= BlockUntil)
            {
                BlockUntil = null;
                FailCount = 0;
                return false;
            }

            return true;
        }

        public double BlockTimeLeft()
        {
            if (BlockUntil == null) return 0;
            return (BlockUntil.Value - DateTime.UtcNow).TotalMinutes;
        }

        public void ResetFailsOnly()
        {
            FailCount = 0;
            BlockUntil = null;
        }
    }
}

namespace QueueLess.Models.Enums
{
    public enum QueueEntryStatus
    {
        Waiting = 0,
        Serving = 1,
        Served = 2,
        Skipped = 3,
        Expired = 4
    }
}

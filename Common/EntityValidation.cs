namespace QueueLess.Common
{
    public static class EntityValidation
    {
        public static class ServiceLocation
        {
            public const int NameMaxLength = 150;
            public const int AddressMaxLength = 300;
            public const int PhoneNumberMaxLength = 50;
        }

        public static class Queue
        {
            public const int NameMaxLength = 120;
            public const int DescriptionMaxLength = 500;
            public const int AverageServiceTimeMin = 1;
            public const int AverageServiceTimeMax = 240;
        }

        public static class QueueEntry
        {
            public const int ClientNameMaxLength = 100;
        }
    }
}

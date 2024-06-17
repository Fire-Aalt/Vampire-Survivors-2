using System;

namespace Game
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class SmartUpdateAttribute : Attribute
    {
        public UpdateInterval Interval { get; }

        public SmartUpdateAttribute(UpdateInterval interval)
        {
            Interval = interval;
        }
    }

    public enum UpdateInterval
    {
        TenTimes = 0, // Every frame
        FiveTimes = 1,     // Approximately 30 times per second
        TwoTimes = 2,   // Approximately 10 times per second
        Slow = 3      // Approximately 5 times per second
    }
}

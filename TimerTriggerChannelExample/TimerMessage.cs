using System;

namespace TimerTriggerChannelExample;

public record TimerMessage
{
    public DateTimeOffset Timestamp{ get; set; }
}

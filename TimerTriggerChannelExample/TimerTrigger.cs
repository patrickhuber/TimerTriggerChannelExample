using System;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace TimerTriggerChannelExample;

/// <summary>
/// Creates a timer trigger that generates a timer message at the given period.
/// <see cref="https://www.youtube.com/watch?v=J4JL4zR_l-0"/> 
/// </summary>
public class TimerTrigger : BackgroundService
{
    private readonly TimeSpan period;
    private readonly TimeProvider timeProvider;
    private readonly ChannelWriter<TimerMessage> timerChannel;

    public TimerTrigger(TimeProvider timeProvider, ChannelWriter<TimerMessage> timerChannel, IOptions<TimerTriggerOptions> options)
    {
        this.period = options.Value.Period;
        this.timeProvider = timeProvider;
        this.timerChannel = timerChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        PeriodicTimer timer = new PeriodicTimer(this.period, this.timeProvider);
        while (await timer.WaitForNextTickAsync(stoppingToken)
            && !stoppingToken.IsCancellationRequested)
        {
            TimerMessage message = new TimerMessage
            { 
                Timestamp = this.timeProvider.GetUtcNow()
            };
            await this.timerChannel.WriteAsync(message);
        }
    }
}

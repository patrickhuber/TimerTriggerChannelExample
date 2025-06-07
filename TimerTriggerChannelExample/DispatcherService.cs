// See https://aka.ms/new-console-template for more information
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace TimerTriggerChannelExample;

public class DispatcherService : BackgroundService
{
    private ChannelReader<TimerMessage> timerChannel;
    private readonly ChannelWriter<ConsoleMessage> consoleChannel;

    public DispatcherService(
        ChannelReader<TimerMessage> timerChannel,
        ChannelWriter<ConsoleMessage> consoleChannel)
    {
        this.timerChannel = timerChannel;
        this.consoleChannel = consoleChannel;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (TimerMessage message in timerChannel.ReadAllAsync(stoppingToken))
        {
            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            ConsoleMessage consoleMessage = new ConsoleMessage
            {
                Message = "Triggerd at {0}",
                Args = new object[] { message.Timestamp }
            };
            await this.consoleChannel.WriteAsync(consoleMessage, stoppingToken);
        }
    }
}

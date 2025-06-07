using System;
using System.Threading.Channels;
using Microsoft.Extensions.Hosting;

namespace TimerTriggerChannelExample;

public class ConsoleService : BackgroundService
{
    private readonly ChannelReader<ConsoleMessage> consoleChannel;

    public ConsoleService(ChannelReader<ConsoleMessage> consoleChannel)
    {
        this.consoleChannel = consoleChannel;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (ConsoleMessage consoleMessage in this.consoleChannel.ReadAllAsync(stoppingToken))
        {
            Console.WriteLine(consoleMessage.Message, consoleMessage.Args);
        }
    }
}

// See https://aka.ms/new-console-template for more information
using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TimerTriggerChannelExample;

Console.WriteLine("Hello, World!");
/*
builder.Services.Configure<TransientFaultHandlingOptions>(
    builder.Configuration.GetSection(
        key: nameof(TransientFaultHandlingOptions)));
*/
IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((host, services) =>
    {
        // load configurations (this case appsettings.json)
        IConfiguration configuration = host.Configuration;
        services.Configure<TimerTriggerOptions>(configuration.GetSection(nameof(TimerTrigger)));

        // create the timer channel. We will register the reader and writer indepentently
        Channel<TimerMessage> timerChannel = Channel.CreateBounded<TimerMessage>(1);

        // Register TimerTrigger dependencies and service
        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(timerChannel.Writer);
        services.AddHostedService<TimerTrigger>();

        // Create the console channel
        Channel<ConsoleMessage> consoleChannel = Channel.CreateBounded<ConsoleMessage>(1);

        // Register DispatcherService dependencies and service
        services.AddSingleton(timerChannel.Reader);
        services.AddSingleton(consoleChannel.Writer);
        services.AddHostedService<DispatcherService>();

        // Register ConsoleService dependencies and service
        services.AddSingleton(consoleChannel.Reader);
        services.AddHostedService<ConsoleService>();
    })
    .Build();

await host.RunAsync();
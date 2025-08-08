using Microsoft.Extensions.Options;
using Microsoft.Extensions.Time.Testing;
using System.Threading.Channels;

namespace TimerTriggerChannelExample.Tests
{
    [TestClass]
    public sealed class TimerTriggerTests
    {
        public TestContext TestContext { get; set; }

        public TimerTriggerTests(TestContext testContext)
        {
            this.TestContext = testContext;
        }

        [TestMethod]
        public async Task TestCanTriggerTimerAsync()
        {
            // Arrange
            FakeTimeProvider timeProvider = new FakeTimeProvider();
            Channel<TimerMessage> timerChannel = Channel.CreateUnbounded<TimerMessage>();
            TimerTrigger timerTrigger = new TimerTrigger(
                timeProvider,
                timerChannel.Writer,
                Options.Create(new TimerTriggerOptions { Period = TimeSpan.FromSeconds(1) }));
       
            CancellationToken cancellationToken = this.TestContext.CancellationTokenSource.Token;

            // Act - start the timer trigger and advance the time by 1 seconds
            await timerTrigger.StartAsync(cancellationToken);
            timeProvider.Advance(TimeSpan.FromSeconds(1));

            // Assert
            // there should be 1 message in the channel
            Assert.AreEqual(1, timerChannel.Reader.Count);

            // read the messages from the channel
            for (int i = 0; i < timerChannel.Reader.Count; i++)
            {
                await timerChannel.Reader.ReadAsync(cancellationToken);
            }

            // there should be 0 messages in the channel
            Assert.AreEqual(0, timerChannel.Reader.Count);

            // advance the time by 1 seconds to trigger the timer again
            timeProvider.Advance(TimeSpan.FromSeconds(1));

            // there should be 1 message in the channel
            Assert.AreEqual(1, timerChannel.Reader.Count);

            // read the messages from the channel
            for (int i = 0; i < timerChannel.Reader.Count; i++)
            {
                await timerChannel.Reader.ReadAsync(cancellationToken);
            }

            // there should be 0 messages in the channel
            Assert.AreEqual(0, timerChannel.Reader.Count);

            // stop the timer trigger
            await timerTrigger.StopAsync(cancellationToken);
        }
    }
}

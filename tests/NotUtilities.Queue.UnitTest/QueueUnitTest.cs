using NotUtilities.Queue;
using NotUtilities.Queue.Interface;

namespace NotUtilities.Queue.UnitTest
{
    [TestFixture]
    public class QueueUnitTest
    {
        [Test]
        public async Task QueueMessageAsync_WithValidMessage_QueuesSuccessfully()
        {
            var queue = new Queue(10);
            string topic = "Test Topic";
            string expectedMessage = "Test Message";

            var messageId = await queue.QueueMessageAsync(topic, expectedMessage);

            Assert.That(messageId, Is.Not.EqualTo(Guid.Empty), "Message ID should not be Guid.Empty.");
        }

        [Test]
        public void QueueMessageAsync_WithNullTopic_ThrowsArgumentNullException()
        {
            var queue = new Queue(10);
            string expectedMessage = "Test Message";

            var ex1 = Assert.ThrowsAsync<ArgumentNullException>(async () => await queue.QueueMessageAsync(null, expectedMessage));
            var ex2 = Assert.ThrowsAsync<ArgumentNullException>(async () => await queue.QueueMessageAsync(string.Empty, expectedMessage));

            Assert.That(ex1.ParamName, Is.EqualTo("topic"), "ArgumentNullException should be thrown for null message.");
            Assert.That(ex2.ParamName, Is.EqualTo("topic"), "ArgumentNullException should be thrown for null message.");
        }

        [Test]
        public void QueueMessageAsync_WithNullMessage_ThrowsArgumentNullException()
        {
            var queue = new Queue(10);
            string topic = "Test Topic";

            var ex = Assert.ThrowsAsync<ArgumentNullException>(async () => await queue.QueueMessageAsync<string>(topic, null));
            Assert.That(ex.ParamName, Is.EqualTo("message"), "ArgumentNullException should be thrown for null message.");
        }

        [Test]
        public async Task DequeueMessageAsync_WithValidTopic_ReturnsExpectedMessage()
        {
            var queue = new Queue(10);
            string topic = "Test Topic";
            string expectedMessage = "Test Message";

            await queue.QueueMessageAsync(topic, expectedMessage);
            var (actualMessage, messageContext) = await queue.DequeueMessageAsync<string>(topic);

            Assert.That(actualMessage, Is.EqualTo(expectedMessage), "The dequeued message should match the enqueued message.");
            Assert.That(messageContext.Topic, Is.EqualTo(topic), "The message context should contain the correct topic.");
        }

        [Test]
        public async Task DequeueMessageAsync_WhenEmpty_WaitTillNewMessageArrive()
        {
            var queue = new Queue(10);
            string topic = "Test Topic";
            string expectedMessage = "Test Message";

            var timeoutTask = Task.Delay(1000);
            var addMessagesTask = Task.Run(async () =>
            {
                await queue.QueueMessageAsync(topic, expectedMessage);
            });
            var readMessagesTask = Task.Run(async () =>
            {
                return await queue.DequeueMessageAsync<string>(topic);
            });

            await Task.WhenAll([timeoutTask, addMessagesTask, readMessagesTask]);

            Assert.That(readMessagesTask.IsCompleted, Is.EqualTo(true));
            Assert.That(readMessagesTask.Result.Item1, Is.EqualTo(expectedMessage), "The dequeued message should match the enqueued message.");
            Assert.That(readMessagesTask.Result.Item2.Topic, Is.EqualTo(topic), "The message context should contain the correct topic.");
        }

        [Test]
        public async Task DisposeAsync_CompletesAllQueues_NoFurtherOperationsAllowed()
        {
            var queue = new Queue(10);
            string topic = "Test Topic";
            string expectedMessage = "Test Message";

            await queue.QueueMessageAsync(topic, expectedMessage);
            await queue.DisposeAsync();

            Assert.ThrowsAsync<ObjectDisposedException>(async () => await queue.QueueMessageAsync(topic, expectedMessage));
            Assert.ThrowsAsync<ObjectDisposedException>(async () => await queue.DequeueMessageAsync<string>(topic));
        }
    }
}
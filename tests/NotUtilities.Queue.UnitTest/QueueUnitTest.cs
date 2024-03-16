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

        //[Test]
        //public async Task ConcurrentQueueAndDequeueOperations_ShouldMaintainIntegrity()
        //{
        //    var queue = new Queue(50); // Increased capacity for concurrency test
        //    string topic = "concurrentTopic";
        //    int messageCount = 100;

        //    var enqueueTasks = new Task[messageCount];
        //    for (int i = 0; i < messageCount; i++)
        //    {
        //        string message = $"Message {i}";
        //        enqueueTasks[i] = queue.QueueMessageAsync(topic, message);
        //    }

        //    await Task.WhenAll(enqueueTasks);

        //    var dequeueTasks = new Task<(string, IMessageContext)>[messageCount];
        //    for (int i = 0; i < messageCount; i++)
        //    {
        //        dequeueTasks[i] = queue.DequeueMessageAsync<string>(topic);
        //    }

        //    await Task.WhenAll(dequeueTasks);

        //    // Validate all messages were dequeued correctly. This is a basic integrity check.
        //    // A more sophisticated approach may be needed to ensure the integrity of message order or content.
        //    Assert.That(dequeueTasks.All(t => t.Result.Item1.StartsWith("Message")), "All messages should be dequeued successfully.");
        //}
    }

}
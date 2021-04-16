using Amazon.SQS;
using Amazon.SQS.Model;
using LocalStack.Client;
using LocalStack.Client.Options;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IntegrationTests.Infrastructure
{
    public class LocalStackAccess
    {
        public LocalStackAccess()
        {
            BuildSQSClient();
        }

        public AmazonSQSClient SQSClient { get; private set; }

        private void BuildSQSClient()
        {
            var sessionOptions = new SessionOptions();
            var configOptions = new ConfigOptions();

            var session = SessionStandalone.Init()
                                .WithSessionOptions(sessionOptions)
                                .WithConfigurationOptions(configOptions).Create();

            SQSClient = session.CreateClientByImplementation<AmazonSQSClient>();
        }

        public void CreateQueue(string queueName)
        {
            SQSClient.CreateQueueAsync(new CreateQueueRequest
            {
                QueueName = queueName
            }).GetAwaiter().GetResult();
        }

        public async Task<IEnumerable<string>> GetMessages()
        {
            var messagesResponse = await SQSClient.ReceiveMessageAsync(new ReceiveMessageRequest
            {
                QueueUrl = "http://localhost:4566/000000000000/ProductQueue",
                WaitTimeSeconds = 1
            });

            var messages = messagesResponse.Messages.Select(x => x.Body);

            return messages;
        }
    }
}

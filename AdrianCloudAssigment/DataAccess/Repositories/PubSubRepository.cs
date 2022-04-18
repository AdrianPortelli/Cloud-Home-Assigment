using Common;
using DataAccess.Interfaces;
using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public class PubSubRepository : IPubSubRepository
    {
        private string projectId;
        private string cloudTopic;
        public PubSubRepository(string _projectId, string _topic)
        {
            cloudTopic = _topic;
            projectId = _projectId;
        }

        public async Task<string> Publish(File file)
        {
            TopicName topic = new TopicName(projectId, cloudTopic);

            PublisherClient client = PublisherClient.Create(topic);

            string file_serialized = JsonConvert.SerializeObject(file);
            PubsubMessage message = new PubsubMessage
            {
                Data = ByteString.CopyFromUtf8(file_serialized)
            };

            return await client.PublishAsync(message);
        }
    }
}

using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Sender
{
    public class SendService : ISendService
    {
        private readonly string _connectionString = "Endpoint=sb://dealservicebus.servicebus.windows.net/;SharedAccessKeyName=Send_Data;SharedAccessKey=4Amc3rl1N2F59cHG/5Zw/kUlwMna9uKA8AVutWP1GhU=;EntityPath=userservice_details";
        private readonly string _queueName = "userservice_details";
        private IQueueClient _queueClient;
        public SendService()
        {
            _queueClient = new QueueClient(_connectionString,_queueName);
        }
        public async Task Publish(string message)
        {
            var Message = new Message(Encoding.UTF8.GetBytes(message));
            await this._queueClient.SendAsync(Message);
            await this._queueClient.CloseAsync();

        }
    }
}

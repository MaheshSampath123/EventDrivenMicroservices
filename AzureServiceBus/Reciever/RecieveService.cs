using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Reciever
{
    class RecieveService:IRecieveService
    {
        private readonly string _connectionString = "Endpoint=sb://dealservicebus.servicebus.windows.net/;SharedAccessKeyName=Listen_Data;SharedAccessKey=OTT4n8+ANbunc+1E2x1sjuX51jK7ShyYbWWeMcgU+b8=;EntityPath=userservice_details";
        private readonly string _queueName = "userservice_details";
        private IQueueClient _queueClient;
        public RecieveService()
        {
            _queueClient = new QueueClient(_connectionString, _queueName);
        }
        public async Task Subscribe()
        {

        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AzureServiceBus.Sender
{
    public interface ISendService
    {
        Task Publish(string message);
    }
}

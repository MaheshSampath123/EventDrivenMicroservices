using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using PostService.Data;

namespace PostService
{
    public class Function
    {
        private readonly  PostDBContext _postDBContext;
        public Function()
        {
            var contextOptions = new DbContextOptionsBuilder<PostDBContext>()
                  .UseSqlServer("Data Source=tcp:generacsqlserver.database.windows.net;Initial Catalog=PostService;User ID=sqlAdmin;Password=generac@D360")
                  .Options;
          var dbContext = new PostDBContext(contextOptions);
            _postDBContext = dbContext;
        }
        public void processservicebus([ServiceBusTrigger("myqueuemessage", Connection = "ServiceBusConnection")] string myQueueItem, ILogger log)
   
        {
            //log.LogInformation(myQueueItem);
            List<UserDetails> message = JsonConvert.DeserializeObject<List<UserDetails>>("["+myQueueItem+"]");
            //var message = JsonConvert.DeserializeObject<List<UserDetails>>(JsonConvert.DeserializeObject<string>(myQueueItem));
            //UserDetails data = new UserDetails();

            foreach(var record in message)
            {
                if (_postDBContext.UserDetails.Any(e => e.ID == record.ID))
                {
                    _postDBContext.UserDetails.Update(record);
                }
                else
                {
                    _postDBContext.UserDetails.Add(record);
                }
                _postDBContext.SaveChanges();
            }
        }
    }
}

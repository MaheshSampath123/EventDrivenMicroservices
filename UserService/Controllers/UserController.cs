using BC = BCrypt.Net.BCrypt;
using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.Data;
using Microsoft.AspNetCore.Authorization;
using UserService.Helpers;

namespace UserService.Controllers
{
    [EnableCors("AllowAll")]
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserDBContext _context;
        private readonly IDistributedCache _distributedCache;
        List<UserDetails> userDetails = new List<UserDetails>();

        public UserController(UserDBContext context, IDistributedCache distributedCache)
        {
            _context = context;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        //[Authorize]
        public async Task<ActionResult<IEnumerable<UserDetails>>> GetUser()
        {
            try
            {
                string data = await _distributedCache.GetStringAsync("Userdata1");
                if (!string.IsNullOrEmpty(data))
                {
                    userDetails = JsonConvert.DeserializeObject<List<UserDetails>>(data);
                    return userDetails;
                }
                else
                {
                    userDetails = await _context.UserData.ToListAsync();
                    if (userDetails != null)
                    {
                        await _distributedCache.SetStringAsync("Userdata1", JsonConvert.SerializeObject(userDetails));
                    }
                    return userDetails;
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        [EnableCors("AllowAll")]
        [HttpPost]
        [Route("AuthenticateByUsername")]
        //[Authorize]
        public async Task<ActionResult> AuthenticateByUsername(UserDetails user)
        {
            UserDetails user1 = user;
            //user.Password = BC.Verify(user1.Password);
            user.Password = CommonHelpers.Encrypt(user.Password);
            if (user != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(user.Mail) && string.IsNullOrEmpty(user.Password))
                        return Content("Username and Password should Enter!");
                    else if (string.IsNullOrEmpty(user.Mail) && !string.IsNullOrEmpty(user.Password))
                        return Content("Username is Required!");
                    else if (!string.IsNullOrEmpty(user.Mail) && string.IsNullOrEmpty(user.Password))
                        return Content("Password is Required!");
                    else if (!string.IsNullOrEmpty(user.Mail) && !string.IsNullOrEmpty(user.Password))
                        //if (_context.UserData.Any(x => x.Mail == user.Mail && CommonHelpers.Decrypt(x.Password) == user.Password))
                        if (_context.UserData.Any(x => x.Mail == user.Mail && x.Password == user.Password))
                            return Content("Valid User");
                    return Content("Not Valid User");
                }
                catch(Exception ex)
                {
                    throw ex;
                }
            }
            else
            {
                return Content("User Details Required");
            }
        }

        [HttpPut("{id}")]
        //[Authorize]
        public async Task<IActionResult> PutUser(int id, UserDetails user)
        {
            _context.UserData.Update(user);
            _context.SaveChanges();
            userDetails = await _context.UserData.ToListAsync();
            await _distributedCache.SetStringAsync("Userdata1", JsonConvert.SerializeObject(userDetails));

            var client = new ServiceBusClient("Endpoint=sb://userpostservicebus.servicebus.windows.net/;SharedAccessKeyName=UserServiceManager;SharedAccessKey=DOc7JOK8WfK9vRIr6zktXDGpb0o59gS06kKzUqIHgiY=;EntityPath=mymessagequeue");
            var sender = client.CreateSender("mymessagequeue");
            var body = JsonConvert.SerializeObject(user);
            var message = new ServiceBusMessage(body);
            await sender.SendMessageAsync(message);
            return Content("Successfully Updated!");
        }


        [HttpPost]
        //[Route("PostUser")]
        //[Authorize]
        public async Task<ActionResult<UserDetails>> PostUser(UserDetails user)
        {
            UserDetails user1 = user;
            user1.Password = CommonHelpers.Encrypt(user.Password); //BC.HashPassword(user.Password);
            _context.UserData.Add(user1);
            int a = _context.SaveChanges();
            userDetails = await _context.UserData.ToListAsync();
            await _distributedCache.SetStringAsync("Userdata1", JsonConvert.SerializeObject(userDetails));
            if (a != 0)
            {
                var client = new ServiceBusClient("Endpoint=sb://messagebroker.servicebus.windows.net/;SharedAccessKeyName=accesspolicy;SharedAccessKey=DKY2jnjP3NNhk9cgDsfcLYQVFjpONiSbUvqTTtv7prI=;EntityPath=myqueuemessage");
                var sender = client.CreateSender("myqueuemessage");
                var body = JsonConvert.SerializeObject(user);
                var message = new ServiceBusMessage(body);
                await sender.SendMessageAsync(message);
                return CreatedAtAction("GetUser", new { id = user.ID }, user);
            }
            else
            {
                return Content("Failed to Inserted..");
            }
        }
    }
}

using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage.Blob;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using PostService.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace PostService.Controllers
{
    [EnableCors("AllowAll")]
    [Route("[controller]")]
    [ApiController]
    public class PostController : ControllerBase
    {
        private readonly PostDBContext _context;
        private readonly IConfiguration _configuration;
        private readonly IDistributedCache _distributedCache;


        public PostController(PostDBContext context, IConfiguration configuration, IDistributedCache distributedCache)
        {
            _context = context;
            _configuration = configuration;
            _distributedCache = distributedCache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PostDetails>>> GetPost()
        {
            try
            {
                //var list = await _context.PostDetails.ToListAsync();
                //return list;

                List<PostDetails> postDetails = new List<PostDetails>();
                string data = await _distributedCache.GetStringAsync("PostData1");
                if (!string.IsNullOrEmpty(data))
                {
                    postDetails = JsonConvert.DeserializeObject<List<PostDetails>>(data);
                    return postDetails;
                }
                else
                {
                    postDetails = await _context.PostDetails.ToListAsync();
                    if (postDetails != null)
                    {
                        await _distributedCache.SetStringAsync("PostData1", JsonConvert.SerializeObject(postDetails));
                    }
                    return postDetails;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<PostDetails>>> GetPost(int ID)
        {
            var list = await _context.PostDetails.Where(x => x.UserId == ID).ToListAsync();
            return list;
        }

        [HttpGet("{id}/{Category}")]
        public async Task<ActionResult<IEnumerable<PostDetails>>> GetPost(int ID, string Category)
        {
            List<PostDetails> postList = new List<PostDetails>();
            if (ID == 0 && string.IsNullOrEmpty(Category))
                postList = await _context.PostDetails.ToListAsync();
            else if (ID > 0 && string.IsNullOrEmpty(Category))
                postList = await _context.PostDetails.Where(x => x.UserId == ID).ToListAsync();
            else if (ID > 0 && !string.IsNullOrEmpty(Category))
                postList = await _context.PostDetails.Where(x => x.UserId == ID && x.CategoryName == Category).ToListAsync();
            else if (ID == 0 && !string.IsNullOrEmpty(Category))
                postList = await _context.PostDetails.Where(x => x.CategoryName == Category).ToListAsync();
            return postList;
        }

        [HttpPost]
        public async Task<ActionResult<PostDetails>> PostPost(PostDetails post)
        {
            //var filename = GenerateFileName(post.BlobUrl);
            //var filename = GenericUriParser(post.BlobUrl);
            //var fileUrl = "";
            //BlobContainerClient container = new BlobContainerClient("BlobStorage", "posts");
            //try
            //{
            //    BlobClient blob = container.GetBlobClient(post.BlobUrl);
            //    using (Stream stream = post.BlobUrl)
            //    {
            //        blob.Upload(stream);
            //    }
            //    fileUrl = blob.Uri.AbsoluteUri;
            //}
            //catch (Exception ex) { }
            //var result = fileUrl;


            try
            {
                //if postdetaisl containsfile content the use blobstorage and upload that file into posts container
                //you will get url of that file
                //string connection = string.Empty;
                //string containerName = string.Empty;
                //if (post.BlobUrl != string.Empty)
                //    connection = Environment.GetEnvironmentVariable("BlobStorage");
                //    containerName = Environment.GetEnvironmentVariable("posts");
                //    Stream myBlob = new MemoryStream();
                //    var file = post.BlobUrl.FirstOrDefault();
                //    var blobClient = new BlobContainerClient(connection, containerName);
                //    var blob = blobClient.GetBlobClient(file.ToString());
                //    await blob.UploadAsync(myBlob);
                //    return new OkObjectResult("file uploaded");
                _context.PostDetails.Add(post);
                int a = _context.SaveChanges();
                if (a >= 0)
                    return Content("Successfully Posted!");
                return Content("Not Posted");

                //var storagequeue = _configuration.GetValue<string>("StorageQueue");
                //await SendingToStorage(storagequeue,post);

                //var values = _context.UserDetails.Where(x => x.ID == post.UserId).Select(e=>e.Name);
                // var values = _context.UserDetails.Where(x => x.ID == post.UserId);
                //var a=  CreatedAtAction("GetPost", new { id = post.PostId }, post);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    return Content(ex.InnerException.Message);
                else
                    return Content(ex.Message);
            }
            //var userdata = CreatedAtAction("GetUser", new { id = user.ID }, user);
            //UserDetails userDetails = new UserDetails();

            // return CreatedAtAction("GetPost", new { id = post.PostId});

        }

        //public IActionResult Index()
        //{
        //    string connString = "DefaultEndpointsProtocol=https;AccountName=storagetest789;AccountKey=G36mcmEthM****=;EndpointSuffix=core.windows.net";
        //    CloudStorageAccount storageAccount = CloudStorageAccount.Parse(connString);
        //    CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

        //    CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference("pub");
        //    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference("p033tw9j.jpg");

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        cloudBlockBlob.DownloadToStream(ms);
        //        return File(ms.ToArray(), "image/jpeg");
        //    }
        //}




        //public static async Task SendingToStorage(string storagequeue, PostDetails post)
        //{
        //    var storageaccount = CloudStorageAccount.Parse(storagequeue);
        //    storageaccount.CreateCloudQueueClient();
        //    var queueClient = storageaccount.CreateCloudQueueClient();
        //    var queue = queueClient.GetQueueReference("postqueuestorage");
        //    var message = new CloudQueueMessage(post);
        //    await queue.AddMessageAsync(message);
        //}
    }
}

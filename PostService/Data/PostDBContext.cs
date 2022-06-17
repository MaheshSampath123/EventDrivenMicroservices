using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PostService.Data
{
    public class PostDBContext:DbContext
    {
        public PostDBContext(DbContextOptions<PostDBContext> options)
            :base(options)
        {

        }
        public DbSet<PostDetails> PostDetails { get; set; }
        public DbSet<UserDetails> UserDetails{ get; set; }
    }
}

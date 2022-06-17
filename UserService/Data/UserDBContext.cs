using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UserService.Data
{
    public class UserDBContext: DbContext
    {
        public UserDBContext(DbContextOptions<UserDBContext> options)
            :base(options)
        {

        }

        public DbSet<UserDetails> UserData { get; set; }
    }
}

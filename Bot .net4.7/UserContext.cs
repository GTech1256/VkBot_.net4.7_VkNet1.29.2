using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Bot.net4._7
{
    class UserContext :DbContext
    {
        public UserContext() 
            : base("DbContext")
        { }

        //public DbSet<User> User { get; set; }
    }
}

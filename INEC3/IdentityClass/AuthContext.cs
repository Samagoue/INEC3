using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace INEC3.IdentityClass
{
    public class AuthContext : IdentityDbContext<IdentityUser>
    {
        public AuthContext() : base("inecConn")
        {
        }
        
    }

}
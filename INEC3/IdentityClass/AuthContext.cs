using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
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
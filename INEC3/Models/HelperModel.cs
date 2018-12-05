using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace INEC3.Models
{
    public class HelperModel
    {
       
    }
    public class SqlCommandText
    {
        public string vw_UserProfile = "Select * from vw_UserProfile";
        public string vw_UserProfileWhere = "Select top(1) * from vw_UserProfile where ";
    }
}
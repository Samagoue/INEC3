﻿using System;
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
        public string SelectRoleList = "select Id, [Name] from  AspNetRoles";
    }
    static class UserManageRoles
    {
        public static string SuperAdmin { get { return "SuperAdmin"; } }
        public static string ProvinceUser { get { return "Province"; } }
        public static string TerritoireUser { get { return "Territoire"; } }
        public static string CommuneUser { get { return "Commune"; } }
        public static string PollingUser { get { return "PollingStation"; } }
    }
    public class RoleModel
    {
        public string Id { get; set; }
        public string Role { get; set; }
    }
}
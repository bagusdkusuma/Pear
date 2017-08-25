using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels
{
    [Serializable]
    public class UserProfileSessionData
    {
        public int UserId { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public string RedirectUrl { get; set; }
        public int LoginId { get; set; }

        public List<KeyValuePair<int,string>> RolePrivilegeName { get; set; }
    }
}
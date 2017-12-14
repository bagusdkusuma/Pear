using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace DSLNG.PEAR.Web.ViewModels.User
{
    public interface ICustomPrincipal : IPrincipal
    {
        int Id { get; set; }
        string Username { get; set; }
        string Email { get; set; }
        bool IsActive { get; set; }
        int RoleId { get; set; }
        string RoleName { get; set; }
        bool IsSuperAdmin { get; set; }

    }
    public class CustomPrincipal : ICustomPrincipal
    {
        public IIdentity Identity { get; private set; }

        public CustomPrincipal(string email)
        {
            this.Identity = new GenericIdentity(email);
        }

        public bool IsInRole(string role)
        {
            return false;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsSuperAdmin { get; set; }
        public int LoginId { get; set; }
    }
}
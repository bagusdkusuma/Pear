using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace DSLNG.PEAR.Data.Entities
{
    public class UserLogin
    {
        public UserLogin()
        {
            this.AuditUsers = new HashSet<AuditUser>();
        }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public User User { get; set; }
        public string IpAddress { get; set; }
        public string HostName { get; set; }
        public string Browser { get; set; }
        public DateTime LastLogin { get; set; }
        public ICollection<AuditUser> AuditUsers { get; set; }
    }
}

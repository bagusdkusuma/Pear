using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class ResetPassword
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Email { get; set; }
        [Key]
        public string Token { get; set; }
        public string Salt { get; set; }
        public DateTime ExpireDate { get; set; }
        public bool Status { get; set; }
    }
}

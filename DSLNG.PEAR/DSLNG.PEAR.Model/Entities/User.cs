using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Username { get; set; }
        public string Password { get; set; }
        public string PasswordSalt { get; set; }

        [MaxLength(100)]
        [Index(IsUnique = true)]
        public string Email { get; set; }
        public RoleGroup Role { get; set; }
        public int? RoleId { get; set; }
        

        public bool IsActive { get; set; }
        
        public string ChangeModel { get; set; }
        public bool IsSuperAdmin { get; set; }
    }
}

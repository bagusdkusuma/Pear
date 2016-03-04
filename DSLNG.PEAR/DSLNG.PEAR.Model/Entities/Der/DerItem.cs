using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class DerItem 
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public Der Der { get; set; }
        public string Type { get; set; } //text, highlight, artifact, image
        public int? ComponentId { get; set; }
        public string Text { get; set; }
        public string FileLocation { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

    }
}

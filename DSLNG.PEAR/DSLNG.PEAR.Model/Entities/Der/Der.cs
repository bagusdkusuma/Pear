using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Der
{
    public class Der : BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public DateTime Date { get; set; }
        public int Revision { get; set; }
        public string Filename { get; set; }
        public User GenerateBy { get; set; }
        public User RevisionBy { get; set; }
        public bool IsActive { get; set; }
    }
}

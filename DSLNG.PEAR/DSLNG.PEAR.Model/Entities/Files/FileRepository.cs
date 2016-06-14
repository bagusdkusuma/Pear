using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DSLNG.PEAR.Data.Entities.Files
{
    public class FileRepository : BaseEntity
    {
        public FileRepository()
        {

        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int Year { get; set; }
        public int Month { get; set; }

        public string Name { get; set; }
        public string Summary { get; set; }
        public int ExSumDefaultPage { get; set; }

        public string Filename { get; set; }
        public byte[] Data { get; set; }

        public DateTime? LastWriteTime { get; set; }
    }
}

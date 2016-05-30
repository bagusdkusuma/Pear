using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Requests.ProcessBlueprint
{
    public class SaveProcessBlueprintRequest
    {
        public int Id { get; set; }

        public int ParentId { get; set; }

        public string Name { get; set; }

        public bool IsFolder { get; set; }

        public byte[] Data { get; set; }

        public DateTime? LastWriteTime { get; set; }
        public int UserId { get; set; }
    }
}

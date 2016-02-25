using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Responses.EnvironmentScanning
{
    public class GetDropdownConstraintResponse
    {
        public IList<Type> Types { get; set; }
        public IList<Category> Categories { get; set; }

        public class Type
        {
            public string Name { get; set; }
        }


        public class Category
        {
            public string Name { get; set; }
        }
    }
}

using System;

namespace DSLNG.PEAR.Data.Entities
{
    public class BaseAction
    {
        public int UserId { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
    }
}

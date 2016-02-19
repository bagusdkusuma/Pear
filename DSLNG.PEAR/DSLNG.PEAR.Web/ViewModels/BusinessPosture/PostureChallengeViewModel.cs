using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.ViewModels.BusinessPosture
{
    public class PostureChallengeViewModel
    {
        public int Id { get; set; }
        public int PostureId { get; set; }
        public string Definition { get; set; }
        public int[] RelationIds { get; set; }
        public string PostureType { get; set; }
    }


    public class PostureChalengeListViewModel
    {
        public PostureChalengeListViewModel()
        {
            DesiredStates = new List<DesiredState>();
        }
        public int Id { get; set; }
        public List<DesiredState> DesiredStates { get; set; }
        
        public class DesiredState
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
    }
}
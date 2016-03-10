
using DSLNG.PEAR.Data.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DSLNG.PEAR.Data.Entities.Blueprint
{
    public class BusinessPostureIdentification
    {
        public BusinessPostureIdentification(){
            Postures = new List<Posture>();
        }
        [Key]
        public int Id { get; set; }
        public bool IsLocked { get; set; }
        public bool IsBeingReviewed { get; set; }
        public bool IsApproved { get; set; }
        public bool IsRejected { get; set; }
        public string Notes { get; set; }
        public PlanningBlueprint PlanningBlueprint { get; set; }
        public IList<Posture> Postures { get; set; }

        public Posture ConstructionPosture { get {
            return Postures.FirstOrDefault(x => x.Type == PostureType.Construction);
        } }
        public Posture OperationPosture
        {
            get
            {
                return Postures.FirstOrDefault(x => x.Type == PostureType.Operation);
            }
        }
        public Posture DecommissioningPosture
        {
            get
            {
                return Postures.FirstOrDefault(x => x.Type == PostureType.Decommissioning);
            }
        }
    }
}



using System;
using System.Collections.Generic;
namespace DSLNG.PEAR.Web.ViewModels.PlanningBlueprint
{
    public class VoyagePlanViewModel
    {
        public PostureViewModel ConstructionPosture { get; set; }
        public PostureViewModel OperationPosture { get; set; }
        public PostureViewModel DecommissioningPosture { get; set; }
        public IList<UltimateObjectivePointViewModel> ConstructionPhase { get; set; }
        public IList<UltimateObjectivePointViewModel> OperationPhase { get; set; }
        public IList<UltimateObjectivePointViewModel> ReinventPhase { get; set; }
        public IList<ChallengeViewModel> InternalChallenge { get; set; }
        public IList<ChallengeViewModel> ExternalChallenge { get; set; }
        public IList<ConstraintViewModel> Constraints { get; set; }
        public IList<KeyOutputViewModel> EconomicIndicators { get; set; }

        public class ChallengeViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class ConstraintViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class PostureViewModel
        {
            public IList<DesiredStateViewModel> DesiredStates { get; set; }
            public IList<PostureChallengeViewModel> PostureChallenges { get; set; }
            public IList<PostureConstraintViewModel> PostureConstraints { get; set; }
        }

        public class DesiredStateViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }

        public class PostureChallengeViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class PostureConstraintViewModel
        {
            public int Id { get; set; }
            public string Definition { get; set; }
        }

        public class UltimateObjectivePointViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
        }
        public class KeyOutputViewModel
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Actual { get; set; }
            public string Forecast { get; set; }
            public int Order { get; set; }
            public string Measurement { get; set; }
        }

        public IList<MidtermFormulationStageViewModel> MidtermFormulationStages { get; set; }
        public class MidtermFormulationStageViewModel
        {
            public MidtermFormulationStageViewModel()
            {
                Descriptions = new List<MidtermPhaseDescriptionViewModel>();
                KeyDrivers = new List<MidtermPhaseKeyDriverViewModel>();
            }
            public int Id { get; set; }
            public string Title { get; set; }
            public int Order { get; set; }
            public DateTime? StartDate { get; set; }
            public DateTime? EndDate { get; set; }
            public IList<MidtermPhaseDescriptionViewModel> Descriptions { get; set; }
            public IList<MidtermPhaseKeyDriverViewModel> KeyDrivers { get; set; }
        }
        public class MidtermPhaseDescriptionViewModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
        public class MidtermPhaseKeyDriverViewModel
        {
            public int Id { get; set; }
            public string Value { get; set; }
        }
    }
}
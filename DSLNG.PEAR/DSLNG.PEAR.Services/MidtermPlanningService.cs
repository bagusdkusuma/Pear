
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Responses.MidtermPlanning;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Services.Requests.MidtermPlanning;
using DSLNG.PEAR.Data.Entities.Blueprint;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Data.Entities;

namespace DSLNG.PEAR.Services
{
    public class MidtermPlanningService : BaseService, IMidtermPlanningService
    {
        public MidtermPlanningService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public GetMidtermPlanningsResponse GetByStageId(int id)
        {
            var midtermPlannings = DataContext.MidtermStrategyPlannings
                .Include(x => x.Objectives)
                .Include(x => x.Kpis)
                .Include(x => x.Kpis.Select(y => y.Measurement))
                .OrderBy(x => x.Id)
                .Where(x => x.Stage.Id == id).ToList().MapTo<GetMidtermPlanningsResponse.MidtermPlanning>();
            var resp = new GetMidtermPlanningsResponse
            {
                MidtermPlannings = midtermPlannings
            };
            if (midtermPlannings.Count > 0)
            {

                //get kpi target and actual for first-child
                var firstMp = midtermPlannings.First();
                var firstKpiIds = firstMp.Kpis.Select(x => x.Id).ToArray();
                var firstTargets = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => firstKpiIds.Contains(x.Id) && x.PeriodeType == PeriodeType.Monthly
                    && x.Periode.Year == firstMp.StartDate.Value.Year).ToList();
                var firstActuals = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => firstKpiIds.Contains(x.Id) && x.PeriodeType == PeriodeType.Monthly
                   && x.Periode.Year == firstMp.StartDate.Value.Year).ToList();
                foreach(var kpi in firstMp.Kpis){
                    kpi.Target = firstTargets.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                    kpi.Actual = firstActuals.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                };
                if (midtermPlannings.Count > 1) {
                    var lastMp = midtermPlannings.Last();
                    var lastKpiIds = firstMp.Kpis.Select(x => x.Id).ToArray();
                    var lastTargets = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => lastKpiIds.Contains(x.Id) && x.PeriodeType == PeriodeType.Monthly
                        && x.Periode.Year == lastMp.StartDate.Value.Year).ToList();
                    var lastActuals = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => lastKpiIds.Contains(x.Id) && x.PeriodeType == PeriodeType.Monthly
                       && x.Periode.Year == lastMp.StartDate.Value.Year).ToList();
                    foreach (var kpi in lastMp.Kpis)
                    {
                        kpi.Target = lastTargets.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                        kpi.Actual = lastActuals.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                    };
                    if (midtermPlannings.Count > 2) {
                        var kpiIds = midtermPlannings.SelectMany(x => x.Kpis).ToList().Select(x => x.Id).ToArray();
                        var startYear = midtermPlannings[1].StartDate.Value.Year;
                        var endYear = midtermPlannings[midtermPlannings.Count - 2].EndDate.Value.Year;
                        var kpiTargets = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => kpiIds.Contains(x.Id) && x.PeriodeType == PeriodeType.Yearly
                            && x.Periode.Year >= startYear && x.Periode.Year <= endYear).ToList();
                        var kpiActuals = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => kpiIds.Contains(x.Id)
                            && x.PeriodeType == PeriodeType.Yearly
                            && x.Periode.Year >= startYear && x.Periode.Year <= endYear).ToList();
                        for (var i = 0; i < midtermPlannings.Count; i++) {
                            if (i == 0 || i == midtermPlannings.Count - 1) {
                                continue;
                            }
                            foreach (var kpi in midtermPlannings[i].Kpis.ToList()) {
                                kpi.Target = kpiTargets.Where(x => x.Kpi.Id == kpi.Id && x.Periode.Year == midtermPlannings[i].StartDate.Value.Year).ToList().Sum(x => x.Value);
                                kpi.Actual = kpiActuals.Where(x => x.Kpi.Id == kpi.Id && x.Periode.Year == midtermPlannings[i].StartDate.Value.Year).ToList().Sum(x => x.Value);
                            }
                        }
                    }
                }
            }
            return resp;
        }


        public AddObjectiveResponse AddObejctive(AddObjectiveRequest request)
        {
            try
            {
                var objective = request.MapTo<MidtermStrategicPlanningObjective>();
                var midtermPlanning = new MidtermStrategicPlanning { Id = request.MidtermPlanningId };
                DataContext.MidtermStrategyPlannings.Attach(midtermPlanning);
                objective.MidtermStrategicPlanning = midtermPlanning;
                DataContext.MidtermStrategicPlanningObjectives.Add(objective);
                DataContext.SaveChanges();
                return new AddObjectiveResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully add Objective",
                    Id = objective.Id,
                    Value = objective.Value
                };
            }
            catch {
                return new AddObjectiveResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact adminstrator for further information"
                };
            }
        }

        public BaseResponse DeleteObjective(int id) {
            try
            {
                var objective = new MidtermStrategicPlanningObjective { Id = id };
                DataContext.MidtermStrategicPlanningObjectives.Attach(objective);
                DataContext.MidtermStrategicPlanningObjectives.Remove(objective);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully delete the item"
                };
            }
            catch {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact adminstrator for further information"
                };
            }
        }


        public AddPlanningKpiResponse AddKpi(AddPlanningKpiRequest request)
        {
            var response = new AddPlanningKpiResponse();
            var midtermPlanning = DataContext.MidtermStrategyPlannings
                .Include(x => x.Kpis)
                .Include(x => x.Kpis.Select(y => y.Measurement))
                .First(x => x.Id == request.MidtermPlanningId);
            var kpi = DataContext.Kpis.Include(x => x.Measurement).First(x => x.Id == request.KpiId);
            midtermPlanning.Kpis.Add(kpi);
            DataContext.SaveChanges();
            if (midtermPlanning.StartDate.Value.Month == 1 && midtermPlanning.EndDate.Value.Month == 12)
            {
                var target = DataContext.KpiTargets.Where(x => x.Kpi.Id == kpi.Id && x.PeriodeType == PeriodeType.Yearly
                    && x.Periode.Year == midtermPlanning.StartDate.Value.Year).Select(x => x.Value).FirstOrDefault();
                if (target.HasValue) response.Target = target.Value;
                var actual = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpi.Id && x.PeriodeType == PeriodeType.Yearly
                  && x.Periode.Year == midtermPlanning.StartDate.Value.Year).Select(x => x.Value).FirstOrDefault();
                if (actual.HasValue) response.Actual = actual.Value;
            }
            else {
                var qTarget = DataContext.KpiTargets.Where(x => x.Kpi.Id == kpi.Id & x.PeriodeType == PeriodeType.Monthly
                    && x.Periode >= midtermPlanning.StartDate && x.Periode <= midtermPlanning.EndDate);
                var qActual = DataContext.KpiAchievements.Where(x => x.Kpi.Id == kpi.Id & x.PeriodeType == PeriodeType.Monthly
                    && x.Periode >= midtermPlanning.StartDate && x.Periode <= midtermPlanning.EndDate);
                if (kpi.YtdFormula == YtdFormula.Sum)
                {
                    response.Actual = qActual.Sum(x => x.Value.Value);
                    response.Target = qTarget.Sum(x => x.Value.Value);
                }
                else {
                    response.Actual = qActual.Average(x => x.Value.Value);
                    response.Target = qTarget.Average(x => x.Value.Value);
                }
            }
            response.Id = kpi.Id;
            response.Name = kpi.Name;
            response.Measurement = kpi.Measurement.Name;
            return response;
        }


        public BaseResponse Delete(int id)
        {
            try
            {
                var planning = new MidtermStrategicPlanning { Id = id };
                DataContext.MidtermStrategyPlannings.Attach(planning);
                DataContext.MidtermStrategyPlannings.Remove(planning);
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully delete the item"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact adminstrator for further information"
                };
            }
        }

        public BaseResponse DeleteKpi(int id, int midTermId)
        {
            try
            {
                var midtermPlanning = DataContext.MidtermStrategyPlannings
                .Include(x => x.Kpis)
                .Include(x => x.Kpis.Select(y => y.Measurement))
                .First(x => x.Id == midTermId);
                midtermPlanning.Kpis.Remove(midtermPlanning.Kpis.First(x => x.Id == id));
                DataContext.SaveChanges();
                return new BaseResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully delete the item"
                };
            }
            catch
            {
                return new BaseResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact adminstrator for further information"
                };
            }
        }


        public AddMidtermPlanningResponse Add(AddMidtermPlanningRequest request)
        {
            try
            {
                var midtermPlanning = request.MapTo<MidtermStrategicPlanning>();
                var midtermStage = new MidtermPhaseFormulationStage { Id = request.MidtermStageId };
                DataContext.MidtermPhaseFormulationStages.Attach(midtermStage);
                midtermPlanning.Stage = midtermStage;
                DataContext.MidtermStrategyPlannings.Add(midtermPlanning);
                DataContext.SaveChanges();
                return new AddMidtermPlanningResponse
                {
                    Id = midtermPlanning.Id,
                    StartDate = midtermPlanning.StartDate,
                    EndDate = midtermPlanning.EndDate,
                    Title = midtermPlanning.Title,
                    IsSuccess = true,
                    Message = "You have been successfully add new item"
                };
            }
            catch {
                return new AddMidtermPlanningResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully add new item"
                };
            }
        }
    }
}

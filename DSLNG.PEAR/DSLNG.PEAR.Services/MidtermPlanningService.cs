
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
            var midtermPlannings = DataContext.MidtermStrategicPlannings
                .Include(x => x.Objectives)
                .Include(x => x.Kpis)
                .Include(x => x.Kpis.Select(y => y.Kpi))
                .Include(x => x.Kpis.Select(y => y.Kpi.Measurement))
                .OrderBy(x => x.Id)
                .Where(x => x.Stage.Id == id).ToList().MapTo<GetMidtermPlanningsResponse.MidtermPlanning>();
            var resp = new GetMidtermPlanningsResponse
            {
                MidtermPlannings = midtermPlannings
            };
            if (midtermPlannings.Count > 0)
            {
                //get kpi from early periode to end periode
                var kpiIds = midtermPlannings.SelectMany(x => x.Kpis).ToList().Select(x => x.Id).ToArray();
                var start = midtermPlannings.First().StartDate;
                var end = midtermPlannings.Last().EndDate;

                var targets = DataContext.KpiTargets.Where(x => kpiIds.Contains(x.Kpi.Id) &&
                    PeriodeType.Yearly == x.PeriodeType && 
                    x.Periode.Year >= start.Value.Year && x.Periode.Year<= end.Value.Year
                    && x.Value.HasValue).ToList();

                foreach (var midtermPlanning in midtermPlannings) {
                    foreach (var kpi in midtermPlanning.Kpis) {
                        var kpiData = new GetMidtermPlanningsResponse.KpiData { Kpi = kpi };
                        //user yearly
                        //if (midtermPlanning.StartDate.Value.Month == 1 && midtermPlanning.EndDate.Value.Month == 12)
                        //{
                            var sTargets = targets.Where(x => x.Kpi.Id == kpi.Id &&
                                x.Periode.Year == midtermPlanning.StartDate.Value.Year &&
                                x.PeriodeType == PeriodeType.Yearly).Select(x => x.Value).ToList();
                            if (kpi.YtdFormula == YtdFormula.Sum)
                            {
                                kpiData.Target = sTargets.Count() > 0 ? sTargets.Sum() : null;
                            }
                            else
                            {
                                kpiData.Target = sTargets.Count() > 0 ? sTargets.Average() : null;
                            }
                        //}
                        //else {
                        //    var sTargets = targets.Where(x => x.Kpi.Id == kpi.Id &&
                        //       x.Periode >= midtermPlanning.StartDate && x.Periode <= midtermPlanning.EndDate &&
                        //       x.PeriodeType == PeriodeType.Monthly).Select(x => x.Value).ToList();
                        //    if (kpi.YtdFormula == YtdFormula.Sum)
                        //    {
                        //        kpiData.Target = sTargets.Count() > 0? sTargets.Sum():null;
                        //    }
                        //    else
                        //    {
                        //        kpiData.Target = sTargets.Count() > 0? sTargets.Average() : null;
                        //    }
                        //}
                        midtermPlanning.KpiDatas.Add(kpiData);
                    }
                }

                //economics
                var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard && x.IsActive);
                if (scenario != null){
                var economics = DataContext.KeyOperationDatas.Where(x => kpiIds.Contains(x.Kpi.Id) &&
                     (PeriodeType.Yearly == x.PeriodeType || PeriodeType.Monthly == x.PeriodeType) &&
                    x.Periode >= start && x.Periode <= end &&
                    x.Scenario.Id == scenario.Id).ToList();

                foreach (var midtermPlanning in midtermPlannings)
                {
                    foreach (var kpi in midtermPlanning.Kpis)
                    {
                        var kpiData = midtermPlanning.KpiDatas.First(x => x.Kpi.Id == kpi.Id);
                        //user yearly
                        //if (midtermPlanning.StartDate.Value.Month == 1 && midtermPlanning.EndDate.Value.Month == 12)
                        //{
                            var sEconomics = economics.Where(x => x.Kpi.Id == kpi.Id &&
                                x.Periode.Year == midtermPlanning.StartDate.Value.Year &&
                                x.PeriodeType == PeriodeType.Yearly).Select(x => x.Value).ToList();
                            if (kpi.YtdFormula == YtdFormula.Sum)
                            {
                                kpiData.Economic = sEconomics.Count() > 0 ? sEconomics.Sum() : null;
                            }
                            else
                            {
                                kpiData.Economic = sEconomics.Count() > 0 ? sEconomics.Average() : null;
                            }
                        //}
                        //else
                        //{
                        //    var sEconomics = economics.Where(x => x.Kpi.Id == kpi.Id &&
                        //       x.Periode >= midtermPlanning.StartDate && x.Periode <= midtermPlanning.EndDate &&
                        //       x.PeriodeType == PeriodeType.Monthly).Select(x => x.Value).ToList();
                        //    if (kpi.YtdFormula == YtdFormula.Sum)
                        //    {
                        //        kpiData.Economic = sEconomics.Count() > 0 ? sEconomics.Sum() : null;
                        //    }
                        //    else
                        //    {
                        //        kpiData.Economic = sEconomics.Count() > 0 ? sEconomics.Average() : null;
                        //    }
                        //}
                    }
                }

                }

                ////get kpi target and economic for first-child
                //var firstMp = midtermPlannings.First();
                //var firstKpiIds = firstMp.Kpis.Select(x => x.Id).ToArray();
                //var firstTargets = DataContext.KpiTargets
                //    .Include(x => x.Kpi)
                //    .Where(x => firstKpiIds.Contains(x.Kpi.Id) && x.PeriodeType == PeriodeType.Monthly
                //     && x.Periode >= firstMp.StartDate && x.Periode <= firstMp.EndDate ).ToList();
                //var scenario = DataContext.Scenarios.FirstOrDefault(x => x.IsDashboard && x.IsActive);
                //if (scenario != null)
                //{
                //    var firstEconomics = DataContext.KeyOperationDatas.Include(x => x.Kpi).Where(x => firstKpiIds.Contains(x.Kpi.Id) && x.PeriodeType == PeriodeType.Monthly
                //        && x.Periode >= firstMp.StartDate && x.Periode <= firstMp.EndDate  && x.Scenario.Id == scenario.Id).ToList();
                //    foreach (var kpi in firstMp.Kpis)
                //    {
                //        if (kpi.YtdFormula == YtdFormula.Sum)
                //        {
                //            kpi.Economic = firstEconomics.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                //        }
                //        else {
                //            kpi.Economic = firstEconomics.Where(x => x.Kpi.Id == kpi.Id).Average(x => x.Value);
                //        }
                //    };
                //}
                //foreach(var kpi in firstMp.Kpis){
                //    if (kpi.YtdFormula == YtdFormula.Sum)
                //    {
                //        kpi.Target = firstTargets.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                //    }
                //    else
                //    {
                //        kpi.Target = firstTargets.Where(x => x.Kpi.Id == kpi.Id).Average(x => x.Value);
                //    }
                //};
                //if (midtermPlannings.Count > 1) {
                //    var lastMp = midtermPlannings.Last();
                //    var lastKpiIds = firstMp.Kpis.Select(x => x.Id).ToArray();
                //    var lastTargets = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => lastKpiIds.Contains(x.Kpi.Id) && x.PeriodeType == PeriodeType.Monthly
                //         && x.Periode >= lastMp.StartDate && x.Periode <= lastMp.EndDate ).ToList();
                //    if (scenario != null)
                //    {
                //        var lastEconomics = DataContext.KeyOperationDatas.Include(x => x.Kpi).Where(x => lastKpiIds.Contains(x.Kpi.Id) && x.PeriodeType == PeriodeType.Monthly
                //          && x.Periode >= lastMp.StartDate && x.Periode <= lastMp.EndDate  && x.Scenario.Id == scenario.Id).ToList();
                //        foreach (var kpi in lastMp.Kpis)
                //        {
                //            kpi.Economic = lastEconomics.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                //        };
                //    }
                //    foreach (var kpi in lastMp.Kpis)
                //    {
                //        kpi.Target = lastTargets.Where(x => x.Kpi.Id == kpi.Id).Sum(x => x.Value);
                //    };
                //    if (midtermPlannings.Count > 2) {
                //        var kpiIds = midtermPlannings.SelectMany(x => x.Kpis).ToList().Select(x => x.Id).ToArray();
                //        var startYear = midtermPlannings[1].StartDate.Value.Year;
                //        var endYear = midtermPlannings[midtermPlannings.Count - 2].EndDate.Value.Year;
                //        var kpiTargets = DataContext.KpiTargets.Include(x => x.Kpi).Where(x => kpiIds.Contains(x.Kpi.Id) && x.PeriodeType == PeriodeType.Yearly
                //            && x.Periode.Year >= startYear && x.Periode.Year <= endYear ).ToList();
                //        if (scenario != null)
                //        {
                //            var kpiEconomics = DataContext.KpiAchievements.Include(x => x.Kpi).Where(x => kpiIds.Contains(x.Kpi.Id)
                //                && x.PeriodeType == PeriodeType.Yearly
                //                && x.Periode.Year >= startYear && x.Periode.Year <= endYear).ToList();
                //            for (var i = 0; i < midtermPlannings.Count; i++)
                //            {
                //                if (i == 0 || i == midtermPlannings.Count - 1)
                //                {
                //                    continue;
                //                }
                //                foreach (var kpi in midtermPlannings[i].Kpis.ToList())
                //                {
                //                    kpi.Economic = kpiEconomics.Where(x => x.Kpi.Id == kpi.Id && x.Periode.Year == midtermPlannings[i].StartDate.Value.Year).ToList().Sum(x => x.Value);
                //                }
                //            }
                //        }
                //        for (var i = 0; i < midtermPlannings.Count; i++)
                //        {
                //            if (i == 0 || i == midtermPlannings.Count - 1)
                //            {
                //                continue;
                //            }
                //            foreach (var kpi in midtermPlannings[i].Kpis.ToList())
                //            {
                //                kpi.Target = kpiTargets.Where(x => x.Kpi.Id == kpi.Id && x.Periode.Year == midtermPlannings[i].StartDate.Value.Year).ToList().Sum(x => x.Value);
                //            }
                //        }
                        
                //    }
                //}
            }
            resp.MidtermPlannings = resp.MidtermPlannings.OrderBy(x => x.StartDate).ToList();
            return resp;
        }


        public AddObjectiveResponse AddObejctive(AddObjectiveRequest request)
        {
            try
            {
                var objective = request.MapTo<MidtermStrategicPlanningObjective>();
                if (request.Id == 0)
                {
                    var midtermPlanning = new MidtermStrategicPlanning { Id = request.MidtermPlanningId };
                    DataContext.MidtermStrategicPlannings.Attach(midtermPlanning);
                    objective.MidtermStrategicPlanning = midtermPlanning;
                    DataContext.MidtermStrategicPlanningObjectives.Add(objective);
                }
                else { 
                    objective = DataContext.MidtermStrategicPlanningObjectives.First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<MidtermStrategicPlanningObjective>(objective);
                }
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
            var midtermPlanning = DataContext.MidtermStrategicPlannings
                .Include(x => x.Kpis)
                .Include(x => x.Kpis.Select(y => y.Kpi))
                .Include(x => x.Kpis.Select(y => y.Kpi.Measurement))
                .First(x => x.Id == request.MidtermPlanningId);
            Kpi kpi = null;
            if (request.OldKpiId != 0 )
            {
                if (request.OldKpiId != request.KpiId)
                {
                    var oldRelation = midtermPlanning.Kpis.First(x => x.Kpi.Id == request.OldKpiId);
                    oldRelation.Kpi = DataContext.Kpis.Include(x => x.Measurement).First(x => x.Id == request.KpiId);
                }
                else {
                    kpi = midtermPlanning.Kpis.First(x => x.Kpi.Id == request.KpiId).Kpi;
                }
            }
            else
            {
                kpi = DataContext.Kpis.Include(x => x.Measurement).First(x => x.Id == request.KpiId);
                DataContext.MidtermPlanningKpis.Add(new MidtermPlanningKpi { 
                    Kpi = kpi,
                    MidtermStrategicPlanning = midtermPlanning
                });
            }
            DataContext.SaveChanges();
            var activeScenario = DataContext.Scenarios.FirstOrDefault(x => x.IsActive && x.IsDashboard);
            //if (midtermPlanning.StartDate.Value.Month == 1 && midtermPlanning.EndDate.Value.Month == 12)
            //{
                var target = DataContext.KpiTargets.Where(x => x.Kpi.Id == kpi.Id && x.PeriodeType == PeriodeType.Yearly
                    && x.Periode.Year == midtermPlanning.StartDate.Value.Year).Select(x => x.Value).FirstOrDefault();
                if (target.HasValue) response.Target = target.Value;
                if(activeScenario != null){
                var economic = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpi.Id && x.PeriodeType == PeriodeType.Yearly
                  && x.Periode.Year == midtermPlanning.StartDate.Value.Year && x.Scenario.Id == activeScenario.Id).Select(x => x.Value).FirstOrDefault();
                if (economic.HasValue) response.Economic = economic.Value;
                }
            //}
            //else {
            //    var qTarget = DataContext.KpiTargets.Where(x => x.Kpi.Id == kpi.Id & x.PeriodeType == PeriodeType.Monthly
            //        && x.Periode >= midtermPlanning.StartDate && x.Periode <= midtermPlanning.EndDate ).Select(x => x.Value);
            //    if (activeScenario != null)
            //    {
            //        var qEconomic = DataContext.KeyOperationDatas.Where(x => x.Kpi.Id == kpi.Id & x.PeriodeType == PeriodeType.Monthly
            //            && x.Periode >= midtermPlanning.StartDate && x.Periode <= midtermPlanning.EndDate  && x.Scenario.Id == activeScenario.Id).Select(x => x.Value);
            //        if (qEconomic.Count() > 0)
            //        {
            //            if (kpi.YtdFormula == YtdFormula.Sum)
            //            {
            //                response.Economic = qEconomic.Sum(x => x.Value);
            //            }
            //            else
            //            {
            //                response.Economic = qEconomic.Average(x => x.Value);
            //            }
            //        }
            //    }
            //    if (qTarget.Count() > 0)
            //    {
            //        if (kpi.YtdFormula == YtdFormula.Sum)
            //        {
            //            response.Target = qTarget.Sum(x => x.Value);
            //        }
            //        else
            //        {
            //            response.Target = qTarget.Average(x => x.Value);
            //        }
            //    }
            //}
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
                DataContext.MidtermStrategicPlannings.Attach(planning);
                DataContext.MidtermStrategicPlannings.Remove(planning);
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
                var midtermPlanningKpi = DataContext.MidtermPlanningKpis.First(x => x.Kpi.Id == id &&
                    x.MidtermStrategicPlanning.Id == midTermId);
                DataContext.MidtermPlanningKpis.Remove(midtermPlanningKpi);
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
                DataContext.MidtermStrategicPlannings.Add(midtermPlanning);
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


        public SubmitMidtermPlanningResponse SubmitMidtermPlanning(int id)
        {
            try
            {
                var midtermPlanning = DataContext.MidtermStrategyPlannings.First(x => x.Id == id);
                midtermPlanning.IsLocked = true;
                midtermPlanning.IsBeingReviewed = true;
                midtermPlanning.IsRejected = false;
                DataContext.SaveChanges();
                return new SubmitMidtermPlanningResponse
                {
                    IsSuccess = true,
                    Message = "You have been sucessfully sabmit the item"
                };
            }
            catch
            {
                return new SubmitMidtermPlanningResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the adminstrator for further information"
                };
            }
        }


        public bool IsValid(int id)
        {
            return !DataContext.MidtermPhaseFormulations.Where(x => x.PlanningBlueprint.Id == id)
                .SelectMany(x => x.MidtermPhaseFormulationStages).Any(y => y.MidtermStrategicPlannings.Count == 0) &&
                !DataContext.MidtermPhaseFormulations.Where(x => x.PlanningBlueprint.Id == id)
                .SelectMany(x => x.MidtermPhaseFormulationStages).SelectMany(x => x.MidtermStrategicPlannings)
                .Any(x => x.Objectives.Count == 0) &&
                 !DataContext.MidtermPhaseFormulations.Where(x => x.PlanningBlueprint.Id == id)
                .SelectMany(x => x.MidtermPhaseFormulationStages).SelectMany(x => x.MidtermStrategicPlannings)
                .Any(x => x.Kpis.Count == 0);
        }
    }
}

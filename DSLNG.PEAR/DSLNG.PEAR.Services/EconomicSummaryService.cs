using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.EconomicSummary;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.EconomicSummary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class EconomicSummaryService : BaseService, IEconomicSummaryService
    {
        private IOutputConfigService _outputConfigService;
        public EconomicSummaryService(IDataContext context, IOutputConfigService outputConfigService)
            : base(context)
        {
            _outputConfigService = outputConfigService;
        }

        public GetEconomicSummariesResponse GetEconomicSummaries(GetEconomicSummariesRequest request)
        {
            if (request.OnlyCount)
            {
                var query = DataContext.EconomicSummaries.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Term.ToLower()));
                }
                return new GetEconomicSummariesResponse { Count = query.Count() };
            }
            else
            {
                var query = DataContext.EconomicSummaries.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Term.ToLower()));
                }
                query = query.OrderByDescending(x => x.Id);
                if (request.Skip != 0)
                {
                    query = query.Skip(request.Skip);
                }
                if (request.Take != 0)
                {
                    query = query.Take(request.Take);
                }
                return new GetEconomicSummariesResponse
                {
                    EconomicSummaries = query.ToList()
                        .MapTo<GetEconomicSummariesResponse.EconomicSummary>()
                };
            }
        }

        public GetEconomicSummariesResponse GetEconomicSummariesForGrid(GetEconomicSummariesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetEconomicSummariesResponse
            {
                TotalRecords = totalRecords,
                EconomicSummaries = data.ToList().MapTo<GetEconomicSummariesResponse.EconomicSummary>()
            };
        }


        public SaveEconomicSummaryResponse SaveEconomicSummary(SaveEconomicSummaryRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var economicSummary = request.MapTo<EconomicSummary>();
                    foreach (var scenarioId in request.Scenarios.Select(x => x.Id))
                    {
                        var scenario = new Scenario { Id = scenarioId };
                        DataContext.Scenarios.Attach(scenario);
                        economicSummary.Scenarios.Add(scenario);
                    }
                    DataContext.EconomicSummaries.Add(economicSummary);
                }
                else
                {
                    var economicSummary = DataContext.EconomicSummaries.Include(x => x.Scenarios).First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<EconomicSummary>(economicSummary);
                    foreach (var scenario in economicSummary.Scenarios.ToList())
                    {
                        economicSummary.Scenarios.Remove(scenario);
                    }
                    foreach (var scenario in request.Scenarios)
                    {
                        var theScenario = DataContext.Scenarios.Local.FirstOrDefault(x => x.Id == scenario.Id);
                        if (theScenario == null)
                        {
                            theScenario = new Scenario { Id = scenario.Id };
                            DataContext.Scenarios.Attach(theScenario);
                        }
                        economicSummary.Scenarios.Add(theScenario);
                    }
                }
                DataContext.SaveChanges();
                return new SaveEconomicSummaryResponse
                {
                    IsSuccess = true,
                    Message = "Economic Summary Config has been Saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveEconomicSummaryResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }


        public GetEconomicSummaryResponse GetEconomicSummary(GetEconomicSummaryRequest request)
        {
            return DataContext.EconomicSummaries
                .Include(x => x.Scenarios)
                .Single(x => x.Id == request.Id).MapTo<GetEconomicSummaryResponse>();
        }


        public DeleteEconomicSummaryResponse DeleteEconomicSummary(DeleteEconomicSummaryRequest request)
        {
            var checkId = DataContext.EconomicSummaries.FirstOrDefault(x => x.Id == request.Id);
            if (checkId != null)
            {
                DataContext.EconomicSummaries.Attach(checkId);
                DataContext.EconomicSummaries.Remove(checkId);
                DataContext.SaveChanges();
            }
            return new DeleteEconomicSummaryResponse
            {
                IsSuccess = true,
                Message = "Economic Summary has deleted successfully"
            };
        }
        public GetEconomicSummaryReportResponse GetEconomicSummaryReport()
        {
            return GetEconomicSummaryReport(false);
        }

        public GetEconomicSummaryReportResponse GetEconomicSummaryReport(bool updateResult)
        {
            var response = new GetEconomicSummaryReportResponse();
            var activeEconomicSummary = DataContext.EconomicSummaries
                .Include(x => x.Scenarios)
                .FirstOrDefault(x => x.IsActive);
            if (activeEconomicSummary != null)
            {
                foreach (var scenario in activeEconomicSummary.Scenarios)
                {
                    response.Scenarios.Add(new GetEconomicSummaryReportResponse.Scenario() { Id = scenario.Id, Name = scenario.Name });

                    var output = _outputConfigService.CalculateOputput(new CalculateOutputRequest { ScenarioId = scenario.Id, UpdateResult=updateResult });

                    foreach (var category in output.OutputCategories)
                    {
                        var group = new GetEconomicSummaryReportResponse.Group();
                        group.Name = category.Name;
                        foreach (var keyOutput in category.KeyOutputs.OrderBy(x => x.Order))
                        {
                            group.KeyOutputs.Add(new GetEconomicSummaryReportResponse.KeyOutput
                                {
                                    Measurement = keyOutput.Measurement,
                                    Name = keyOutput.Name,
                                    OutputResult = new GetEconomicSummaryReportResponse.OutputResult { Actual = keyOutput.Actual, Forecast = keyOutput.Forecast },
                                    //OutputResult = new GetEconomicSummaryReportResponse.OutputResult { Actual = "actual " + scenario.Id + "_" + keyOutput.Name, Forecast = "forecast " + scenario.Id + "_" + keyOutput.Name },
                                    Scenario = new GetEconomicSummaryReportResponse.Scenario { Id = scenario.Id, Name = scenario.Name }
                                });
                        }

                        response.Groups.Add(group);
                    }
                }

                /*foreach (var scenario in activeEconomicSummary.Scenarios)
                {
                    var output = _outputConfigService.CalculateOputput(new CalculateOutputRequest { ScenarioId = scenario.Id });
                    foreach (var category in output.OutputCategories)
                    {
                        var group = new GetEconomicSummaryReportResponse.Group
                            {
                                Id = category.Id,
                                Name = category.Name
                            };

                        foreach (var keyOutput in category.KeyOutputs)
                        {
                            if (group.KeyOutputs.Count() == 0)
                            {
                                var outputResults = new List<GetEconomicSummaryReportResponse.OutputResult>();
                                outputResults.Add(new GetEconomicSummaryReportResponse.OutputResult
                                    {
                                        Actual = keyOutput.Actual,
                                        Forecast = keyOutput.Forecast,
                                        Scenario = new GetEconomicSummaryReportResponse.Scenario { Id = scenario.Id }
                                    });
                                group.KeyOutputs.Add(new GetEconomicSummaryReportResponse.KeyOutput
                                    {
                                        Measurement = keyOutput.Measurement,
                                        Name = keyOutput.Name,
                                        OutputResults = outputResults
                                    });
                            }

                        }
                        response.Groups.Add(group);
                    }
                }*/
            }

            return response;
        }

        private IEnumerable<EconomicSummary> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.EconomicSummaries
                .Include(x => x.Scenarios)
                .AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name)
                            : data.OrderByDescending(x => x.Name);
                        break;
                    case "Description":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Desc)
                            : data.OrderByDescending(x => x.Desc);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public void UpdateEconomicSummary()
        {
            GetEconomicSummaryReport(true);
        }
    }
}

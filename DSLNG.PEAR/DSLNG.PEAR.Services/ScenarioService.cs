using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Scenario;
using DSLNG.PEAR.Services.Responses.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.EconomicModel;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class ScenarioService : BaseService, IScenarioService
    {
        public ScenarioService(IDataContext context) : base(context) { }

        public GetScenariosResponse GetScenarios(GetScenariosRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }


            return new GetScenariosResponse
            {
                TotalRecords = totalRecords,
                Scenarios = data.ToList().MapTo<GetScenariosResponse.Scenario>()
            };
            //if (request.OnlyCount)
            //{
            //    return new GetScenariosResponse { Count = DataContext.Scenarios.Count() };
            //}
            //else
            //{
            //    return new GetScenariosResponse
            //    {
            //        Scenarios = DataContext.Scenarios.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take).ToList().MapTo<GetScenariosResponse.Scenario>()
            //    };
            //}
        }


        public SaveScenarioResponse SaveScenario(SaveScenarioRequest request)
        {
            if (request.Id == 0)
            {
                var Scenario = request.MapTo<Scenario>();
                DataContext.Scenarios.Add(Scenario);
            }
            else
            {
                var checkId = DataContext.Scenarios.FirstOrDefault(x => x.Id == request.Id);
                if (checkId != null)
                {
                    request.MapPropertiesToInstance<Scenario>(checkId);
                }
            }
            DataContext.SaveChanges();
            return new SaveScenarioResponse
            {
                IsSuccess = true,
                Message = "Scenario has been save"
            };
        }


        public GetScenarioResponse GetScenario(GetScenarioRequest request)
        {
            return DataContext.Scenarios.FirstOrDefault(x => x.Id == request.Id).MapTo<GetScenarioResponse>();
        }


        public DeleteScenarioResponse DeleteScenario(DeleteScenarioRequest request)
        {
            var scenario = DataContext.Scenarios.Where(x => x.Id == request.Id).FirstOrDefault();
            if (scenario != null)
            {
                DataContext.Scenarios.Attach(scenario);
                DataContext.Scenarios.Remove(scenario);
                DataContext.SaveChanges();
            }
            return new DeleteScenarioResponse
            {
                IsSuccess = true,
                Message = "Scenario has been deleted successfully"
            };
        }

        public IEnumerable<Scenario> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Scenarios.AsQueryable();
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
                            ? data.OrderBy(x => x.Name).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.Name).ThenBy(x => x.Order);
                        break;
                    case "Order":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Order)
                            : data.OrderByDescending(x => x.Order);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.IsActive).ThenBy(x => x.Order);
                        break;
                    case "IsDashboard":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsDashboard).ThenBy(x => x.Order)
                            : data.OrderByDescending(x => x.IsDashboard).ThenBy(x => x.Order);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }
    }
}

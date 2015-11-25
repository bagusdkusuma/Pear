

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;

namespace DSLNG.PEAR.Services
{
    public class OutputConfigService : BaseService,IOutputConfigService
    {

        public OutputConfigService(IDataContext dataContext) : base(dataContext) { }

        public GetKpisResponse GetKpis(GetKpisRequest request)
        {
            return new GetKpisResponse
            {
                KpiList = DataContext.Kpis.Where(x => x.Name.Contains(request.Term) && x.IsEconomic == true).Take(20).ToList().MapTo<GetKpisResponse.Kpi>()
            };
        }


        public GetKeyAssumptionsResponse GetKeyAssumptions(GetKeyAssumptionsRequest request)
        {
            return new GetKeyAssumptionsResponse
            {
                KeyAssumptions = DataContext.KeyAssumptionConfigs.Where(x => x.Name.Contains(request.Term)).Take(20).ToList().MapTo<GetKeyAssumptionsResponse.KeyAssumption>()
            };
        }
    }
}

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services
{
    public class ProcessBlueprintService : BaseService, IProcessBlueprintService
    {
        public ProcessBlueprintService(IDataContext dataContext):base(dataContext)
        {

        }
        public GetProcessBlueprintsResponse Gets(GetProcessBlueprintsRequest request)
        {
            throw new NotImplementedException();
        }

        public GetProcessBlueprintResponse Get(GetProcessBlueprintRequest request)
        {
            throw new NotImplementedException();
        }

        public GetProcessBlueprintResponse Save(SaveProcessBlueprintRequest request)
        {
            throw new NotImplementedException();
        }


        public GetProcessBlueprintResponse Delete(int Id)
        {
            throw new NotImplementedException();
        }
    }
}

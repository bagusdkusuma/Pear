using DSLNG.PEAR.Services.Requests.ProcessBlueprint;
using DSLNG.PEAR.Services.Responses.ProcessBlueprint;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IProcessBlueprintService
    {
        GetProcessBlueprintsResponse Gets(GetProcessBlueprintsRequest request);
        GetProcessBlueprintResponse Get(GetProcessBlueprintRequest request);
        GetProcessBlueprintResponse Save(SaveProcessBlueprintRequest request);
        GetProcessBlueprintResponse Delete(int Id);
    }
}

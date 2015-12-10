using DSLNG.PEAR.Services.Requests.Scenario;
using DSLNG.PEAR.Services.Responses.Scenario;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IScenarioService
    {
        GetScenariosResponse GetScenarios(GetScenariosRequest request);
        GetScenariosResponse GetScenariosForGrid(GetScenariosRequest request);
        SaveScenarioResponse SaveScenario(SaveScenarioRequest request);
        GetScenarioResponse GetScenario(GetScenarioRequest request);
        DeleteScenarioResponse DeleteScenario(DeleteScenarioRequest request);
        int GetActiveScenario();
    }
}

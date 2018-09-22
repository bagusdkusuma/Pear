
using DSLNG.PEAR.Common.Helpers;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.OutputConfig;
using DSLNG.PEAR.Services.Requests.Scenario;
using DSLNG.PEAR.Services.Responses.OutputConfig;
using DSLNG.PEAR.Web.DependencyResolution;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
namespace DSLNG.PEAR.Web.Scheduler
{
    public class ScenarioScheduler : Registry
    {
        public ScenarioScheduler() {
            Schedule(() =>
            {
                using (var dataContext = new DataContext())
                {
                    try
                    {
                        IScenarioService scenarioService = new ScenarioService(dataContext);
                        IOutputConfigService outputService = new OutputConfigService(dataContext);
                        var scenarios = scenarioService.GetScenarios(new GetScenariosRequest { Take = 0 }).Scenarios;
                        //var outputResults = new List<CalculateOutputResponse>();
                        foreach (var scenario in scenarios)
                        {
                            var outputResult = outputService.CalculateOputput(new CalculateOutputRequest { ScenarioId = scenario.Id });
                            SerializationHelper.SerializeObject<CalculateOutputResponse>(outputResult, "output-scenario-" + scenario.Id);
                        }
                    }
                    catch
                    {
                        //logging here
                    }
                }
            }).ToRunEvery(1).Months().OnTheFirst(DayOfWeek.Monday).At(0, 0);
            
            
            //
        }
    }
}
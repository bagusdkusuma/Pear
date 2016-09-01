using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Responses;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IDropdownService
    {
        IEnumerable<Dropdown> GetScoringTypes();
        IEnumerable<Dropdown> GetPillars();
        IEnumerable<Dropdown> GetPillars(int pmsSummaryId);
        IEnumerable<Dropdown> GetKpis(int pillarId);
        IEnumerable<Dropdown> GetYears();
        IEnumerable<Dropdown> GetYears(int startYear, int endYear);
        IEnumerable<Dropdown> GetYearsForOperationData();
        IEnumerable<Dropdown> GetYearsForPmsSummary();        
        IEnumerable<Dropdown> GetMonths();
        IEnumerable<Dropdown> GetKpisForPmsConfigDetails(int pmsConfigId);
        IEnumerable<Dropdown> GetLevels();
        IEnumerable<Dropdown> GetRoleGroups();
        IEnumerable<Dropdown> GetTypes();
        IEnumerable<Dropdown> GetGroups();
        IEnumerable<Dropdown> GetMethods();
        IEnumerable<Dropdown> GetMeasurement();
        IEnumerable<Dropdown> GetYtdFormulas();
        IEnumerable<Dropdown> GetPeriodeTypes();
        IEnumerable<Dropdown> GetKpis();
        IEnumerable<Dropdown> GetPeriodeTypesForKpiTargetAndAchievement();
        IEnumerable<Dropdown> GetPeriodeTypesDailyMonthlyYearly();
        IEnumerable<Dropdown> GetKpisForPmsConfigDetailsUpdate(int pmsConfigId, int id);
        IEnumerable<Dropdown> GetUsers();
        IEnumerable<Dropdown> GetESConstraintCategories();
        IEnumerable<Dropdown> GetESChallengeCategories();

        IEnumerable<Dropdown> GetConfigTypes();
        IEnumerable<Dropdown> GetDerItemTypes();
        IEnumerable<Dropdown> GetConfigTypes(bool isDer);
    }

    public class Dropdown
    {
        public string Text { get; set; }
        public string Value { get; set; }
    }


}

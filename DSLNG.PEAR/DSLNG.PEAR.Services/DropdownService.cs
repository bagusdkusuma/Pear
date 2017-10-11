using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Data.Entities;
using DSLNG.PEAR.Data.Enums;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;

namespace DSLNG.PEAR.Services
{
    public class DropdownService : BaseService, IDropdownService
    {
        public DropdownService(IDataContext dataContext)
            : base(dataContext)
        {
        }

        public IEnumerable<Dropdown> GetScoringTypes()
        {
            return new List<Dropdown>()
                {
                    new Dropdown {Text = ScoringType.Boolean.ToString(), Value = ScoringType.Boolean.ToString()},
                    new Dropdown {Text = ScoringType.Positive.ToString(), Value = ScoringType.Positive.ToString()},
                    new Dropdown {Text = ScoringType.Negative.ToString(), Value = ScoringType.Negative.ToString()}
                };
        }

        public IEnumerable<Dropdown> GetPillars()
        {
            return DataContext.Pillars.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetPillars(int pmsSummaryId)
        {
            var notAvailablePillars = DataContext.PmsConfigs
                                                 .Include(x => x.PmsSummary)
                                                 .Where(x => x.PmsSummary.Id == pmsSummaryId).Select(x => x.Pillar);

            return DataContext.Pillars.Except(notAvailablePillars).Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetKpis(int pillarId)
        {
            return DataContext.Kpis.Include(x => x.Pillar).Where(x => x.Pillar.Id == pillarId)
                .Select(x => new Dropdown
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
        }

        public IEnumerable<Dropdown> GetKpisForPmsConfigDetails(int pmsConfigId)
        {
            var pmsConfig = DataContext.PmsConfigs.Include(x => x.Pillar)
                                       .Include(x => x.PmsConfigDetailsList.Select(y => y.Kpi))
                                       .Single(x => x.Id == pmsConfigId);
            var kpiIds = pmsConfig.PmsConfigDetailsList.Select(x => x.Kpi.Id);

            return
                DataContext.Kpis.Where(
                    x => x.Type.Code.ToLower() == Constants.Type.Corporate && x.Pillar.Id == pmsConfig.Pillar.Id
                         && !kpiIds.Contains(x.Id))
                           .Select(x => new Dropdown
                           {
                               Text = x.Name + " (" + x.Measurement.Name + ")",
                               Value = x.Id.ToString()
                           }).ToList();
        }

        public IEnumerable<Dropdown> GetYearsForPmsSummary()
        {
            return DataContext.PmsSummaries.Select(x => new Dropdown
            {
                Text = x.Year.ToString(),
                Value = x.Year.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetMonths()
        {
            return DateTimeFormatInfo
                   .InvariantInfo
                   .MonthNames
                   .Where(m => !String.IsNullOrEmpty(m))
                   .Select((monthName, index) => new Dropdown()
                   {
                       Value = (index + 1).ToString(),
                       Text = monthName
                   });
        }

        public IEnumerable<Dropdown> GetYears()
        {
            var years = new List<int>();
            for (int i = 2015; i <= 2030; i++)
            {
                years.Add(i);
            }
            return years.Select(x => new Dropdown
            {
                Value = x.ToString(),
                Text = x.ToString()
            });
        }

        public IEnumerable<Dropdown> GetYearsForOperationData()
        {
            var years = new List<int>();
            for (int i = 2011; i <= 2030; i++)
            {
                years.Add(i);
            }
            return years.Select(x => new Dropdown
            {
                Value = x.ToString(),
                Text = x.ToString()
            });
        }


        public IEnumerable<Dropdown> GetLevels()
        {
            return DataContext.Levels.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<Dropdown> GetRoleGroups()
        {
            return DataContext.RoleGroups.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<Dropdown> GetTypes()
        {
            return DataContext.Types.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<Dropdown> GetGroups()
        {
            return DataContext.Groups.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<Dropdown> GetMethods()
        {
            return DataContext.Methods.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetMeasurement()
        {
            return DataContext.Measurements.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<Dropdown> GetYtdFormulas()
        {
            var ytd = Enum.GetValues(typeof(DSLNG.PEAR.Data.Enums.YtdFormula)).Cast<DSLNG.PEAR.Data.Enums.YtdFormula>();
            return ytd.Select(x => new Dropdown { Text = x.ToString(), Value = x.ToString() }).ToList();
        }

        public IEnumerable<Dropdown> GetPeriodeTypes()
        {
            var periode = Enum.GetValues(typeof(DSLNG.PEAR.Data.Enums.PeriodeType)).Cast<DSLNG.PEAR.Data.Enums.PeriodeType>();
            var ytd = Enum.GetValues(typeof(DSLNG.PEAR.Data.Enums.YtdFormula)).Cast<DSLNG.PEAR.Data.Enums.YtdFormula>();
            return periode.Select(x => new Dropdown { Text = x.ToString(), Value = x.ToString() }).ToList();
        }


        public IEnumerable<Dropdown> GetKpis()
        {
            return DataContext.Kpis.Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetPeriodeTypesForKpiTargetAndAchievement()
        {
            return new List<Dropdown>()
                {
                    new Dropdown {Text = PeriodeType.Monthly.ToString(), Value = PeriodeType.Monthly.ToString()},
                    new Dropdown {Text = PeriodeType.Yearly.ToString(), Value = PeriodeType.Yearly.ToString()}
                };
        }

        public IEnumerable<Dropdown> GetPeriodeTypesDailyMonthlyYearly()
        {
            return new List<Dropdown>()
                {
                new Dropdown {Text = PeriodeType.Daily.ToString(), Value = PeriodeType.Daily.ToString()},
                    new Dropdown {Text = PeriodeType.Monthly.ToString(), Value = PeriodeType.Monthly.ToString()},
                    new Dropdown {Text = PeriodeType.Yearly.ToString(), Value = PeriodeType.Yearly.ToString()}
                };
        }

        public IEnumerable<Dropdown> GetKpisForPmsConfigDetailsUpdate(int pmsConfigId, int id)
        {
            var pmsConfig = DataContext.PmsConfigs.Include(x => x.Pillar)
                                      .Include(x => x.PmsConfigDetailsList.Select(y => y.Kpi))
                                      .Single(x => x.Id == pmsConfigId);
            var kpiIds = pmsConfig.PmsConfigDetailsList.Where(x => x.Kpi.Id != id).Select(x => x.Kpi.Id);

            return DataContext.Kpis.Where(x => x.Type.Code.ToLower() == Constants.Type.Corporate && x.Pillar.Id == pmsConfig.Pillar.Id
                && !kpiIds.Contains(x.Id))
                .Select(x => new Dropdown
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                }).ToList();
        }

        public IEnumerable<Dropdown> GetConfigTypes()
        {
            var config = Enum.GetValues(typeof(DSLNG.PEAR.Data.Enums.ConfigType)).Cast<DSLNG.PEAR.Data.Enums.ConfigType>();
            return config.Select(x => new Dropdown { Text = x.ToString(), Value = x.ToString() }).ToList();
        }


        public IEnumerable<Dropdown> GetUsers()
        {
            return DataContext.Users.Select(x => new Dropdown
            {
                Text = x.Username,
                Value = x.Id.ToString()
            }).ToList();
        }


        public IEnumerable<Dropdown> GetESConstraintCategories()
        {
            return DataContext.ESCategories.Where(x => x.Type == EnvirontmentType.constraint).Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetESChallengeCategories()
        {
            return DataContext.ESCategories.Where(x => x.Type == EnvirontmentType.challenge).Select(x => new Dropdown
            {
                Text = x.Name,
                Value = x.Id.ToString()
            }).ToList();
        }

        public IEnumerable<Dropdown> GetDerItemTypes()
        {
            var dropdowns =  new List<Dropdown>()
                {
                    new Dropdown {Text = "Highlight", Value = "highlight"},
                    new Dropdown {Text = "Multi Axis", Value = "multiaxis"},
                    new Dropdown {Text = "Speedometer", Value = "speedometer"},
                    new Dropdown {Text = "Barmeter", Value = "barmeter"},
                    new Dropdown {Text = "Pie", Value = "pie"},
                    new Dropdown {Text = "Tank", Value = "tank"},
                    new Dropdown {Text = "Weather", Value = "weather"},
                    new Dropdown {Text = "Temperature", Value = "temperature"},
                    new Dropdown {Text = "Alert", Value = "alert"},
                    new Dropdown {Text = "Wave", Value = "wave"},
                    new Dropdown {Text = "Avg Ytd-Key Statistic", Value = "avg-ytd-key-statistic"},
                    new Dropdown {Text = "Safety", Value = "safety"},
                    new Dropdown {Text = "Security Incident Type", Value = "security"},
                    new Dropdown {Text = "LNG And CDS Table", Value = "lng-and-cds"},
                    new Dropdown {Text = "DAFWC and LOPC", Value = "dafwc"},
                    new Dropdown {Text = "JOB PMTS", Value = "job-pmts"},
                    new Dropdown {Text = "MGDP", Value = "mgdp" },
                    new Dropdown {Text = "HHV", Value = "hhv" },
                    new Dropdown {Text = "LNG And CDS Production", Value = "lng-and-cds-production" },
                    new Dropdown {Text = "Total Feed Gas", Value= "total-feed-gas" },
                    new Dropdown {Text = "Table Tank", Value = "table-tank" },
                    new Dropdown {Text = "Weekly Maintenance Backlog", Value = "weekly-maintenance" },
                    new Dropdown {Text = "Critical PM", Value = "critical-pm" },
                    new Dropdown {Text = "Next Loading Schedule", Value = "nls" },
                    new Dropdown {Text = "Procurement", Value = "procurement" },
                    new Dropdown {Text = "Indicative Commercial Price", Value = "indicative-commercial-price" },
                    new Dropdown {Text = "Plant Availability", Value = "plant-availability" },
                    new Dropdown {Text = "Economic Indicator", Value = "economic-indicator" },
                    new Dropdown {Text = "Key Equipment Status", Value = "key-equipment-status" },
                    new Dropdown {Text = "Global Stock Market", Value = "global-stock-market" },
                    new Dropdown {Text = "Prepared By", Value = "prepared-by" },
                    new Dropdown {Text = "Reviewed By", Value = "reviewed-by" },
                    new Dropdown {Text = "Termometer", Value = "termometer" },
                    new Dropdown {Text = "Loading Duration", Value = "loading-duration" },
                    new Dropdown {Text = "Person On Board", Value = "person-on-board" },
                    new Dropdown {Text = "Flare", Value = "flare"},
                    new Dropdown {Text = "JCC Monthly Trend", Value = "jcc-monthly-trend"},
                    new Dropdown {Text = "Total Commitment", Value = "total-commitment"},
                    new Dropdown {Text = "NO2", Value = "no2"},
                    new Dropdown {Text = "SO2", Value = "so2"},
                    new Dropdown {Text = "pH", Value = "ph"},
                    new Dropdown {Text = "Oil & Grease", Value = "oil-grease"},
                    new Dropdown {Text = "Particulate", Value = "particulate"},
                };

            return dropdowns.OrderBy(x => x.Text);
        }

        public IEnumerable<Dropdown> GetConfigTypes(bool isDer)
        {
            if (isDer)
            {
                var config = Enum.GetValues(typeof(ConfigType)).Cast<ConfigType>();
                return (from configType in config.ToList() where configType == ConfigType.KpiAchievement || configType == ConfigType.KpiTarget select new Dropdown { Text = configType.ToString(), Value = configType.ToString() }).ToList();
            }
            return GetConfigTypes();
        }

        public IEnumerable<Dropdown> GetYears(int startYear, int endYear)
        {
            var years = new List<int>();
            for (int i = startYear; i <= endYear; i++)
            {
                years.Add(i);
            }
            return years.Select(x => new Dropdown
            {
                Value = x.ToString(),
                Text = x.ToString()
            });
        }
    }
}

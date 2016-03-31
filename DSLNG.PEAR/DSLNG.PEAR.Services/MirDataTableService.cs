using DSLNG.PEAR.Data.Entities.Mir;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.MirDataTable;
using DSLNG.PEAR.Services.Responses.MirDataTable;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;

namespace DSLNG.PEAR.Services
{
    public class MirDataTableService : BaseService, IMirDataTableService
    {
        public MirDataTableService(IDataContext dataContext) : base(dataContext) { }

        public SaveMirDataTableResponse SaveMirDataTableRespons(SaveMirDataTableRequest request)
        {
            var MirDataTable = request.MapTo<MirDataTable>();
            if (request.MirDataId == 0)
            {
                MirDataTable.MirConfiguration = DataContext.MirConfigurations.FirstOrDefault(x => x.Id == request.MirConfigurationId);
                MirDataTable.Kpis = new List<Kpi>();
                foreach(var kpiId in request.KpiIds)
                {
                    var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId);
                    if (kpi == null )
                    {
                        kpi = new Kpi { Id = kpiId };
                        DataContext.Kpis.Attach(kpi);
                    }
                    MirDataTable.Kpis.Add(kpi);
                }
                DataContext.MirDataTables.Add(MirDataTable);
            }
            else
            {
                MirDataTable = DataContext.MirDataTables.FirstOrDefault(x => x.Id == request.MirDataId);
                request.MapPropertiesToInstance<MirDataTable>(MirDataTable);
                MirDataTable.Kpis = new List<Kpi>();
                foreach(var kpiId in request.KpiIds)
                {
                    var kpi = DataContext.Kpis.Local.FirstOrDefault(x => x.Id == kpiId);
                    if(kpi == null)
                    {
                        kpi = new Kpi { Id = kpiId };
                        DataContext.Kpis.Attach(kpi);
                    }
                    MirDataTable.Kpis.Add(kpi);
                }
            }

            DataContext.SaveChanges();

            return new SaveMirDataTableResponse
            {
                IsSuccess = true,
                Message = "Mir Data Table has been saved succesfully"
            };
        }


        //public DeleteKpiMirDataTableResponse DeleteKpi(DeleteKpiMirDataTableRequest request)
        //{
        //    var mirDataTable = DataContext.MirDataTables.FirstOrDefault(x => x.Id == request.MirDataTableId);
        //    return null;
        //}
    }
}

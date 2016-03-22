using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.PopInformation;
using DSLNG.PEAR.Services.Responses.PopInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.Pop;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class PopInformationService : BaseService, IPopInformationService
    {
        public PopInformationService(IDataContext dataContext) : base(dataContext) { }


        public GetPopInformationResponse GetPopInformation(GetPopInformationRequest request)
        {
            return DataContext.PopInformations.Where(x => x.Id == request.Id).Include(x => x.PopDashboardHost).FirstOrDefault().MapTo<GetPopInformationResponse>();
        }




        public SavePopInformationResponse SavePopInformation(SavePopInformationRequest request)
        {
            var popInformation = request.MapTo<PopInformation>();
            if (request.Id == 0)
            {
                popInformation.PopDashboardHost = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request.DashboardId);
                DataContext.PopInformations.Add(popInformation);
            }
            else
            {
                popInformation = DataContext.PopInformations.Where(x => x.Id == request.Id).FirstOrDefault();
                popInformation.PopDashboardHost = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request.DashboardId);
                request.MapPropertiesToInstance<PopInformation>(popInformation);
            }

            DataContext.SaveChanges();

            return new SavePopInformationResponse
            {
                IsSuccess = true,
                Message = "Pop Information has been saved successfully",
                Id = popInformation.Id,
                Title = popInformation.Title,
                Type = popInformation.Type,
                Value = popInformation.Value
            };
        }

    }
}

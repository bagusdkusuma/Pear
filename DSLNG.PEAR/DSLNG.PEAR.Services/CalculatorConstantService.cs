
using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Services.Responses.CalculatorConstant;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using System.Linq;
using DSLNG.PEAR.Data.Persistence;

namespace DSLNG.PEAR.Services
{
    public class CalculatorConstantService : BaseService, ICalculatorConstantService
    {
        public CalculatorConstantService(IDataContext dataContext) : base(dataContext) { }

        public GetCalculatorConstantsResponse GetCalculatorConstants(GetCalculatorConstantsRequest request)
        {
            if (request.OnlyCount)
            {
                var query = DataContext.CalculatorConstants.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.Contains(request.Term));
                }
                return new GetCalculatorConstantsResponse { Count = query.Count() };
            }
            else {
                var query = DataContext.CalculatorConstants.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.Contains(request.Term));
                }
                return new GetCalculatorConstantsResponse
                {
                    CalculatorConstants = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take)
                        .MapTo<GetCalculatorConstantsResponse.CalculatorConstantResponse>()
                };
            }
        }

        public GetCalculatorConstantResponse GetCalculatorConstant(GetCalculatorConstantRequest request)
        {
            return DataContext.CalculatorConstants.FirstOrDefault(x => x.Id == request.Id).MapTo<GetCalculatorConstantResponse>();
        }

        public SaveCalculatorConstantResponse SaveCalculatorConstant(SaveCalculatorConstantRequest request)
        {
            throw new System.NotImplementedException();
        }
    }
}


using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Services.Responses.CalculatorConstant;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using System.Linq;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;

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
                    query = query.Where(x => x.Name.ToLower().Contains(request.Term.ToLower()));
                }
                return new GetCalculatorConstantsResponse { Count = query.Count() };
            }
            else {
                var query = DataContext.CalculatorConstants.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Term.ToLower()));
                }
                query = query.OrderByDescending(x => x.Id);
                if (request.Skip != 0) {
                    query = query.Skip(request.Skip);
                }
                if (request.Take != 0) {
                    query = query.Take(request.Take);
                }
                return new GetCalculatorConstantsResponse
                {
                    CalculatorConstants = query.ToList()
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
            try
            {
                if (request.Id == 0)
                {
                    var calculatorConstant = request.MapTo<CalculatorConstant>();
                    DataContext.CalculatorConstants.Add(calculatorConstant);
                }
                else
                {
                    var calculatorConstant = DataContext.CalculatorConstants.First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<CalculatorConstant>(calculatorConstant);
                }
                DataContext.SaveChanges();
                return new SaveCalculatorConstantResponse
                {
                    IsSuccess = true,
                    Message = "Calculator Constant Has been saved"
                };
            }
            catch (InvalidOperationException ex) {
                return new SaveCalculatorConstantResponse
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }
    }
}


using DSLNG.PEAR.Services.Requests.CalculatorConstant;
using DSLNG.PEAR.Services.Responses.CalculatorConstant;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Common.Extensions;
using System.Data.Entity;
using System.Linq;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

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
            else
            {
                var query = DataContext.CalculatorConstants.AsQueryable();
                if (!string.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.ToLower().Contains(request.Term.ToLower()));
                }
                query = query.OrderByDescending(x => x.Id);
                if (request.Skip != 0)
                {
                    query = query.Skip(request.Skip);
                }
                if (request.Take != 0)
                {
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
                if (request.IsAjaxRequest)
                {
                    var calculatorConstant = DataContext.CalculatorConstants.First(x => x.Id == request.Id);
                    calculatorConstant.Value = request.Value;
                }
                else
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



        public IEnumerable<CalculatorConstant> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.CalculatorConstants.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search) || x.DisplayName.Contains(search) || x.Measurement.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Name":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Name)
                            : data.OrderByDescending(x => x.Name);
                        break;
                    case "DisplayName":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.DisplayName)
                            : data.OrderByDescending(x => x.DisplayName);
                        break;
                    case "Value":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Value)
                            : data.OrderByDescending(x => x.Value);
                        break;
                    case "Measurement":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Measurement)
                            : data.OrderByDescending(x => x.Measurement);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public DeleteCalculatorConstantResponse DeleteCalculatorConstant(DeleteCalculatorConstantRequest request)
        {
            var calConstant = DataContext.CalculatorConstants.FirstOrDefault(x => x.Id == request.Id);
            if (calConstant != null)
            {
                DataContext.CalculatorConstants.Attach(calConstant);
                DataContext.CalculatorConstants.Remove(calConstant);
                DataContext.SaveChanges();
            }
            return new DeleteCalculatorConstantResponse
            {
                IsSuccess = true,
                Message = "Calculator Constant has been deleted successfully"
            };
        }


        public GetCalculatorConstantsForGridRespone GetCalculatorConstantsForGrid(GetCalculatorConstantForGridRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetCalculatorConstantsForGridRespone
            {
                TotalRecords = totalRecords,
                CalculatorConstantsForGrids = data.ToList().MapTo<GetCalculatorConstantsForGridRespone.CalculatorConstantsForGrid>()
            };
        }
    }
}

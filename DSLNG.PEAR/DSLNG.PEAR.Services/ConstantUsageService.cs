
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.ConstantUsage;
using DSLNG.PEAR.Services.Responses.ConstantUsage;
using DSLNG.PEAR.Common.Extensions;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class ConstantUsageService : BaseService, IConstantUsageService
    {
        public ConstantUsageService(IDataContext dataContext) : base(dataContext) { }

        public GetConstantUsagesResponse GetConstantUsages(GetConstantUsagesRequest request)
        {
            var query = DataContext.ConstantUsages.Include(x => x.Constants);
            if (!string.IsNullOrEmpty(request.Term))
            {
                query = query.Where(x => x.Role.ToLower().Contains(request.Term.ToLower()) || x.Group.ToLower().Contains(request.Term.ToLower()));
            }
            query.OrderByDescending(x => x.Id);
            if (request.Skip != 0)
            {
                query = query.Skip(request.Skip);
            }
            if (request.Take != 0)
            {
                query = query.Take(request.Take);
            }
            if (request.OnlyCount)
            {
                return new GetConstantUsagesResponse { Count = query.Count() };
            }
            else
            {
                return new GetConstantUsagesResponse
                {
                    ConstantUsages = query.ToList().MapTo<GetConstantUsagesResponse.ConstantUsageResponse>()
                };
            }
        }

        public GetConstantUsagesResponse GetConstantUsagesForGrid(GetConstantUsagesRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetConstantUsagesResponse
            {
                TotalRecords = totalRecords,
                ConstantUsages = data.ToList().MapTo<GetConstantUsagesResponse.ConstantUsageResponse>()
            };
        }

        public GetConstantUsageResponse GetConstantUsage(GetConstantUsageRequest request)
        {
            return DataContext.ConstantUsages.Include(x => x.Constants).First(x => x.Id == request.Id).MapTo<GetConstantUsageResponse>();
        }

        public SaveConstantUsageResponse SaveConstantUsage(SaveConstantUsageRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var constantUsage = request.MapTo<ConstantUsage>();
                    foreach (var constantId in request.CalculatorConstantIds)
                    {
                        var constant = new CalculatorConstant { Id = constantId };
                        DataContext.CalculatorConstants.Attach(constant);
                        constantUsage.Constants.Add(constant);
                    }
                    DataContext.ConstantUsages.Add(constantUsage);
                }
                else
                {
                    var constantUsage = DataContext.ConstantUsages.Include(x => x.Constants).First(x => x.Id == request.Id);
                    request.MapPropertiesToInstance<ConstantUsage>(constantUsage);
                    foreach (var constant in constantUsage.Constants.ToList())
                    {
                        constantUsage.Constants.Remove(constant);
                    }
                    foreach (var constantId in request.CalculatorConstantIds)
                    {
                        var constant = DataContext.CalculatorConstants.Local.FirstOrDefault(x => x.Id == constantId);
                        if (constant == null)
                        {
                            constant = new CalculatorConstant { Id = constantId };
                            DataContext.CalculatorConstants.Attach(constant);
                        }
                        constantUsage.Constants.Add(constant);
                    }
                }
                DataContext.SaveChanges();
                return new SaveConstantUsageResponse
                {
                    IsSuccess = true,
                    Message = "Calculator Constant Usage has been saved"
                };
            }
            catch (InvalidOperationException e) {
                return new SaveConstantUsageResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }



        public IEnumerable<ConstantUsage> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.ConstantUsages.Include(x => x.Constants).AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Role.Contains(search) || x.Group.Contains(search));
            }

            foreach (var sortOrder in sortingDictionary)
            {
                switch (sortOrder.Key)
                {
                    case "Role":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Role)
                            : data.OrderByDescending(x => x.Role);
                        break;
                    case "Group":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Group)
                            : data.OrderByDescending(x => x.Group);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public DeleteConstantUsageResponse DeleteConstantUsage(DeleteConstantUsageRequest request)
        {
            var constantUsage = DataContext.ConstantUsages.FirstOrDefault(x => x.Id == request.Id);
            if (constantUsage != null)
            {
                DataContext.ConstantUsages.Attach(constantUsage);
                DataContext.ConstantUsages.Remove(constantUsage);
                DataContext.SaveChanges();
            }
            return new DeleteConstantUsageResponse
            {
                IsSuccess = true,
                Message = "Calculator Constant Usage has been Deleted"
            };
        }
    }
}

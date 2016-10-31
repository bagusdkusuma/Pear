using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Buyer;
using DSLNG.PEAR.Services.Responses.Buyer;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Data.Entity.Infrastructure;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace DSLNG.PEAR.Services
{
    public class BuyerService : BaseService, IBuyerService
    {
        public BuyerService(IDataContext dataContext) : base(dataContext) { }

        public GetBuyerResponse GetBuyer(GetBuyerRequest request)
        {
            return DataContext.Buyers.FirstOrDefault(x => x.Id == request.Id).MapTo<GetBuyerResponse>();
        }

        public GetBuyersResponse GetBuyers(GetBuyersRequest request)
        {
            //int totalRecords;
            //var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            //if (request.Take != -1)
            //{
            //    data = data.Skip(request.Skip).Take(request.Take);
            //}

            //return new GetBuyersResponse
            //{
            //    TotalRecords = totalRecords,
            //    Buyers = data.ToList().MapTo<GetBuyersResponse.BuyerResponse>()
            //};
            if (request.OnlyCount)
            {
                return new GetBuyersResponse { Count = DataContext.Buyers.Count() };
            }
            else
            {
                var query = DataContext.Buyers.AsQueryable();
                if (!String.IsNullOrEmpty(request.Term))
                {
                    query = query.Where(x => x.Name.Contains(request.Term));
                }
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetBuyersResponse
                {
                    Buyers = query.ToList().MapTo<GetBuyersResponse.BuyerResponse>()
                };
            }
        }

        public SaveBuyerResponse SaveBuyer(SaveBuyerRequest request)
        {
            try
            {
                var buyer = request.MapTo<Buyer>();
                if (request.Id == 0)
                {
                    DataContext.Buyers.Add(buyer);
                }
                else
                {
                    buyer = DataContext.Buyers.FirstOrDefault(x => x.Id == request.Id);
                    if (buyer != null)
                    {
                        request.MapPropertiesToInstance<Buyer>(buyer);
                    }
                }

                DataContext.SaveChanges();
                
                return new SaveBuyerResponse
                {
                    Id = buyer.Id,
                    IsSuccess = true,
                    Message = "Buyer has been saved"
                };
            }
            catch (InvalidOperationException e)
            {
                return new SaveBuyerResponse
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }


        public DeleteBuyerResponse Delete(DeleteBuyerRequest request)
        {
            try
            {
                var buyer = new Buyer { Id = request.Id };
                DataContext.Buyers.Attach(buyer);
                DataContext.Buyers.Remove(buyer);
                DataContext.SaveChanges();
                return new DeleteBuyerResponse
                {
                    IsSuccess = true,
                    Message = "Buyer has been Deleted successfully"
                };
            }
            catch (DbUpdateException e)
            {
                if (e.InnerException.InnerException.Message.Contains("dbo.VesselSchedule")) {
                    return new DeleteBuyerResponse
                    {
                        IsSuccess = false,
                        Message = "This item is being used by Vessel Schedule"
                    };
                }
                return new DeleteBuyerResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while tyring to save this item"
                };
            }
            catch (InvalidOperationException) {
                return new DeleteBuyerResponse
                {
                    IsSuccess = false,
                    Message = "An error occured while tyring to save this item"
                };
            }
            
        }

        public IEnumerable<Buyer> SortData(string search, IDictionary<string, SortOrder> sortingDictionary, out int TotalRecords)
        {
            var data = DataContext.Buyers.AsQueryable();
            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.Name.Contains(search));
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
                    case "Address":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.Address)
                            : data.OrderByDescending(x => x.Address);
                        break;
                    case "IsActive":
                        data = sortOrder.Value == SortOrder.Ascending
                            ? data.OrderBy(x => x.IsActive)
                            : data.OrderByDescending(x => x.IsActive);
                        break;
                }
            }

            TotalRecords = data.Count();
            return data;
        }


        public GetBuyerForGridResponse GetBuyersForGrid(GetBuyerForGridRequest request)
        {
            int totalRecords;
            var data = SortData(request.Search, request.SortingDictionary, out totalRecords);
            if (request.Take != -1)
            {
                data = data.Skip(request.Skip).Take(request.Take);
            }

            return new GetBuyerForGridResponse
            {
                TotalRecords = totalRecords,
                BuyerForGrids = data.ToList().MapTo<GetBuyerForGridResponse.BuyerForGrid>()
            };
        }
    }
}

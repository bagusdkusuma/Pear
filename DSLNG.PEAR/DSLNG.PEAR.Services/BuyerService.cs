using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Buyer;
using DSLNG.PEAR.Services.Responses.Buyer;
using System.Linq;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;
using System.Data.Entity.Infrastructure;

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
            if (request.OnlyCount)
            {
                return new GetBuyersResponse { Count = DataContext.Buyers.Count() };
            }
            else
            {
                var query = DataContext.Buyers.AsQueryable();
                if(!String.IsNullOrEmpty(request.Term)){
                    query = query.Where(x => x.Name.Contains(request.Term));
                }
                query = query.OrderByDescending(x => x.Id).Skip(request.Skip).Take(request.Take);
                return new GetBuyersResponse
                {
                    Buyers =query.ToList().MapTo<GetBuyersResponse.BuyerResponse>()
                };
            }
        }

        public SaveBuyerResponse SaveBuyer(SaveBuyerRequest request)
        {
            try
            {
                if (request.Id == 0)
                {
                    var buyer = request.MapTo<Buyer>();
                    DataContext.Buyers.Add(buyer);
                }
                else
                {
                    var buyer = DataContext.Buyers.FirstOrDefault(x => x.Id == request.Id);
                    if (buyer != null)
                    {
                        request.MapPropertiesToInstance<Buyer>(buyer);
                    }
                }
                DataContext.SaveChanges();
                return new SaveBuyerResponse
                {
                    IsSuccess = true,
                    Message = "Highlight has been saved"
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
                    Message = "You have been deleted this itme successfully"
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
    }
}

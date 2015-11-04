

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.HighlightOrder;
using DSLNG.PEAR.Services.Responses.HighlightOrder;
using System.Linq;
using System.Data.Entity;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities;
using System;

namespace DSLNG.PEAR.Services
{
    public class HighlightOrderService : BaseService, IHighlightOrderService
    {
        public HighlightOrderService(IDataContext dataContext) :base(dataContext) { 
        }

        public GetHighlightOrdersResponse GetHighlights(GetHighlightOrdersRequest request)
        {
            var exception = new string[] { "alert" };
            var higlightOptions = DataContext.SelectOptions
                .Where(x => x.Select.Name == "highlight-types" && !exception.Contains(x.Value)).ToList();
            return new GetHighlightOrdersResponse
            {
                HighlightOrders = higlightOptions.MapTo<GetHighlightOrdersResponse.HighlightOrderResponse>()
            };
        }

        public SaveHighlightOrderResponse SaveHighlight(SaveHighlightOrderRequest request)
        {
            try
            {
                var selectOption = new SelectOption { Id = request.Id };
                DataContext.SelectOptions.Attach(selectOption);
                selectOption.Order = request.Order;
                DataContext.SaveChanges();
                return new SaveHighlightOrderResponse
                {
                    IsSuccess = true,
                    Message = "You have been successfully save highlight order"
                };
            }
            catch (InvalidOperationException e) {
                return new SaveHighlightOrderResponse
                {
                    IsSuccess = false,
                    Message = "An error occured, please contact the administrator for further information"
                };
            }
        }
    }
}

using DSLNG.PEAR.Services.Requests.PopDashboard;
using DSLNG.PEAR.Services.Responses.PopDashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPopDashboardService
    {
        GetPopDashboardsResponse GetPopDashboards(GetPopDashboardsRequest request);
        SavePopDashboardResponse SavePopDashboard(SavePopDashboardRequest request);
        GetPopDashboardResponse GetPopDashboard(GetPopDashboardRequest request);
        DeletePopDashboardResponse DeletePopDashboard(int request);

    }
}

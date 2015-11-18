using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Services.Requests.Periode;
using DSLNG.PEAR.Services.Responses.Periode;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPeriodeService
    {
        GetPeriodeResponse GetPeriode(GetPeriodeRequest request);
        GetPeriodesResponse GetPeriodes(GetPeriodesRequest request);
        CreatePeriodeResponse Create(CreatePeriodeRequest request);
        UpdatePeriodeResponse Update(UpdatePeriodeRequest request);
        DeletePeriodeResponse Delete(int id);
        GetPeriodesResponse GetPeriodesForGrid(GetPeriodesRequest request);
    }
}

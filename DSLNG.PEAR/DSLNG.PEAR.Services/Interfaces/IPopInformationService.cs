using DSLNG.PEAR.Services.Requests.PopInformation;
using DSLNG.PEAR.Services.Responses.PopInformation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IPopInformationService
    {
        GetPopInformationResponse GetPopInformation(GetPopInformationRequest request);
        SavePopInformationResponse SavePopInformation(SavePopInformationRequest request);
    }
}

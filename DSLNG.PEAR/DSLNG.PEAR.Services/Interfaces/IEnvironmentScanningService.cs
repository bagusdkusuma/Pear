using DSLNG.PEAR.Services.Requests.EnvironmentScanning;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.EnvironmentScanning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IEnvironmentScanningService
    {
        GetEnvironmentsScanningResponse GetEnvironmentsScanning(GetEnvironmentsScanningRequest request);
        SaveEnvironmentScanningResponse SaveEnvironmentScanning(SaveEnvironmentScanningRequest request);
        DeleteEnvironmentScanningResponse DeleteEnvironmentScanning(DeleteEnvironmentScanningRequest request);
        DeleteEnvironmentScanningResponse DeleteEnvironmentalScanning(DeleteEnvironmentScanningRequest request);
        SaveEnvironmentalScanningResponse SaveEnvironmentalScanning(SaveEnvironmentalScanningRequest request);
        DeleteConstraintResponse DeleteConstraint(DeleteConstraintRequest request);
        DeleteChallengeResponse DeleteChallenge(DeleteChallengeRequest request);
        SaveConstraintResponse SaveConstraint(SaveConstraintRequest request);
        SaveChallengeResponse SaveChallenge(SaveChallengeRequest request);
        SubmitEnvironmentsScanningResponse SubmitEnvironmentsScanning(int id);
        GetConstraintResponse GetConstraint(GetConstraintRequest request);
        GetChallengeResponse GetChallenge(GetChallengeRequest request);
        
    }
}

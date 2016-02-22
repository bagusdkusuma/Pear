

using DSLNG.PEAR.Services.Requests.BusinessPosture;
using DSLNG.PEAR.Services.Responses.BusinessPosture;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IBusinessPostureIdentificationService
    {
        GetBusinessPostureResponse Get(GetBusinessPostureRequest request);
        SaveDesiredStateResponse SaveDesiredState(SaveDesiredStateRequest request);
        DeleteDesiredStateResponse DeleteDesiredState(DeleteDesiredStateRequest request);
        SavePostureChallengeResponse SavePostureChallenge(SavePostureChallengeRequest request);
        DeletePostureChallengeResponse DeletePostureChallenge(DeletePostureChallengeRequest request);
        SavePostureConstraintResponse SavePostureConstraint(SavePostureConstraintRequest request);
        DeletePostureConstraintResponse DeletePostureConstraint(DeletePostureConstraintRequest request);
        SubmitBusinessPostureResponse SubmitBusinessPosture(int id);
        GetPostureChallengeResponse GetPostureChallenge(GetPostureChallengeRequest request);
        GetPostureConstraintResponse GetPostureConstraint(GetPostureConstraintRequest requet);
    }
}

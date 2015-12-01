

using DSLNG.PEAR.Services.Requests.HighlightGroup;
using DSLNG.PEAR.Services.Responses.HighlightGroup;
namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IHighlightGroupService
    {
        GetHighlightGroupsResponse GetHighlightGroups(GetHighlightGroupsRequest request);
        GetHighlightGroupResponse GetHighlightGroup(GetHighlightGroupRequest request);
        SaveHighlightGroupResponse Save(SaveHighlightGroupRequest request);
        DeleteHighlightGroupResponse DeleteHighlightGroup(DeleteHighlightGroupRequest request);
    }
}

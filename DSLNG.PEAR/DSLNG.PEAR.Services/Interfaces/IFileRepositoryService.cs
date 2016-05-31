using DSLNG.PEAR.Services.Requests.Files;
using DSLNG.PEAR.Services.Responses;
using DSLNG.PEAR.Services.Responses.Files;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface IFileRepositoryService
    {
        GetFileRepositoryResponse GetFile(GetFileRequest request);
        GetFilesRepositoryResponse GetFiles(GetFilesRequest request);
        BaseResponse Save(SaveFileRepositoryRequest request);
        BaseResponse Delete(int Id);
    }
}

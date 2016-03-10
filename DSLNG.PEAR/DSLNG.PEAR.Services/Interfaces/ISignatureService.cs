using DSLNG.PEAR.Services.Requests.Signature;
using DSLNG.PEAR.Services.Responses.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSLNG.PEAR.Services.Interfaces
{
    public interface ISignatureService
    {
        SaveSignatureResponse SaveSignature(SaveSignatureRequest request);
        ApproveSignatureResponse ApproveSignature(ApproveSignatureRequest request);
    }
}

using DSLNG.PEAR.Data.Persistence;
using DSLNG.PEAR.Services.Interfaces;
using DSLNG.PEAR.Services.Requests.Signature;
using DSLNG.PEAR.Services.Responses.Signature;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSLNG.PEAR.Common.Extensions;
using DSLNG.PEAR.Data.Entities.Pop;
using System.Data.Entity;

namespace DSLNG.PEAR.Services
{
    public class SignatureService : BaseService, ISignatureService
    {
        public SignatureService(IDataContext dataContext) : base(dataContext) { }



        public SaveSignatureResponse SaveSignature(SaveSignatureRequest request)
        {
            var signature = request.MapTo<Signature>();
            if (request.Id == 0)
            {
                signature.User = DataContext.Users.FirstOrDefault(x => x.Id == request.UserId);
                signature.PopDashboard = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request.DashboardId);
                DataContext.Signatures.Add(signature);
            }
            else
            {
                signature = DataContext.Signatures.FirstOrDefault(x => x.Id == request.Id);
                signature.User = DataContext.Users.FirstOrDefault(x => x.Id == request.UserId);
                signature.PopDashboard = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request.DashboardId);
                request.MapPropertiesToInstance<Signature>(signature);
            }
            DataContext.SaveChanges();

            return new SaveSignatureResponse
            {
                Id = signature.Id,
                UserId = signature.User.Id,
                Username = signature.User.Username,
                TypeSignature = signature.Type,
                SignatureImage = signature.User.SignatureImage
            };
        }




        public ApproveSignatureResponse ApproveSignature(ApproveSignatureRequest request)
        {
            var signature = request.MapTo<Signature>();
            signature = DataContext.Signatures.Include(x => x.User).Include(x => x.PopDashboard).FirstOrDefault(x => x.Id == request.Id);
            signature.User = DataContext.Users.FirstOrDefault(x => x.Id == request.UserId);
            signature.PopDashboard = DataContext.PopDashboards.FirstOrDefault(x => x.Id == request.DashboardId);
            request.MapPropertiesToInstance<Signature>(signature);
            DataContext.SaveChanges();

            return new ApproveSignatureResponse
            {
                Id = signature.Id,
                UserId = signature.User.Id,
                DashboardId = signature.PopDashboard.Id,
                Type = signature.Type,
                Approve = signature.IsApprove,
                Reject = signature.IsReject,
                SignatureImage = signature.User.SignatureImage,
                Note = signature.Note
            };
        }
    }
}

using Common.Models.Common;
using Common.Models.Users;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface ICommonService
    {
        Task LogClientError(ClientErrorModel clientErrorModel);
        
    }
}

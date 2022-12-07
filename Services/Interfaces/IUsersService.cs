using Common.Models.Common;
using Common.Models.Users;
using System.Threading.Tasks;

namespace Services.Interfaces
{
    public interface IUsersService
    {
        Task<UserModel> SignInUser(SignInModel signInModel);
        Task<LookupsModel> GetLookups();
        Task<ListModel<UserModel>> Search(PagingSortingModel<SearchModel> searchModel);
        Task<UserModel> Get(string id);
        Task<BaseModel> Add(UserModel userModel);
        Task<BaseModel> Edit(UserModel userModel);
        Task<BaseModel> Delete(string id);
        Task<BaseModel> ValidateToken(string token);
    }
}

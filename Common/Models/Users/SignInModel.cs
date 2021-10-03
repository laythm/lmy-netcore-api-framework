using Common.Models.Common;

namespace Common.Models.Users
{
    public class SignInModel : BaseModel
    {
        public SignInModel()
        {
      
        }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}

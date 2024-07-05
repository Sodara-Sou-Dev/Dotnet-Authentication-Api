using cube_gaming_store_back.DTOs.Posts;
using static cube_gaming_store_back.DTOs.GeneralResponse;

namespace cube_gaming_store_back.Interfaces
{
    public interface IAuth
    {
        Task<SignUpResponse> SignUp(SignUpPostDTO user);
        Task<SignInResponse> SignIn(SignInPostDTO user);
        Task<bool> SignOut();
        Task<bool> FindUserById(string userId);
        Task<bool> FindUserByEmail(string email);
        Task<bool> CreateRole(string role);
    }
}
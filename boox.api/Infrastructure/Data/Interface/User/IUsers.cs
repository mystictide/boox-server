using boox.api.Infrasructure.Models.Users;
using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Infrastructure.Data.Interface.User
{
    public interface IUsers
    {
        Task<bool> CheckEmail(string Email, int? UserID);
        Task<bool> CheckUsername(string Username, int? UserID);
        Task<Users>? Login(Users entity);
        Task<Users>? Register(Users entity);
        Task<Users>? Get(int? ID, string? Username);
        Task<IEnumerable<UserAddresses>> ManageAddresses(UserAddresses entity, int userID);
        Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword);
        Task<bool>? UpdateEmail(int ID, string Email);
        Task<bool>? DeactivateAccount(int ID);
    }
}

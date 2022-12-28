using MongoDB.Bson;
using boox.api.Infrasructure.Models.Users;
using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Infrastructure.Data.Interface.User
{
    public interface IUsers
    {
        Task<bool> CheckEmail(string Email, ObjectId? UserID);
        Task<bool> CheckUsername(string Username, ObjectId? UserID);
        Task<Users>? Login(Users entity);
        Task<Users>? Register(Users entity);
        Task<Users>? Get(ObjectId? ID, string? Username);
        Task<UserAddresses> ManageAddresses(UserAddresses entity, ObjectId userID);
        Task<bool>? ChangePassword(ObjectId UserID, string currentPassword, string newPassword);
        Task<bool>? UpdateEmail(ObjectId ID, string Email);
        Task<bool>? DeactivateAccount(ObjectId ID);
    }
}

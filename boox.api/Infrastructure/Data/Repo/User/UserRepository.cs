using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.Extensions.Options;
using boox.api.Infrasructure.Models.Users;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Data.Repo.Helpers;
using boox.api.Infrastructure.Data.Interface.User;
using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Infrastructure.Data.Repo.User
{
    public class UserRepository : AppSettings, IUsers
    {
        private readonly IOptions<AppSettings> _settings;

        public UserRepository(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        private static IMongoCollection<Users> collection = GetDB.GetCollection<Users>("users");
        public async Task<Users>? Register(Users entity)
        {
            try
            {
                await collection.InsertOneAsync(entity);
                return entity;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<Users>? Login(Users entity)
        {
            try
            {
                var result = await collection.FindAsync(m => m.Email == entity.Email);
                return result.SingleAsync().Result;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<bool> CheckEmail(string Email, ObjectId? UserID)
        {
            try
            {
                bool result;
                IAsyncCursor<Users> response;
                if (UserID != null)
                {
                    response = await collection.FindAsync(m => m.Email == Email && m.ID != UserID);
                }
                else
                {
                    response = await collection.FindAsync(m => m.Email == Email);
                }

                if (response.SingleOrDefault() == null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return true;
            }
        }

        public async Task<bool> CheckUsername(string Username, ObjectId? UserID)
        {
            try
            {
                bool result;
                IAsyncCursor<Users> response;
                if (UserID != null)
                {
                    response = await collection.FindAsync(m => m.Username == Username && m.ID != UserID);
                }
                else
                {
                    response = await collection.FindAsync(m => m.Username == Username);
                }

                if (response.SingleOrDefault() == null)
                {
                    result = false;
                }
                else
                {
                    result = true;
                }
                return result;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return true;
            }
        }

        public async Task<bool>? DeactivateAccount(ObjectId ID)
        {
            try
            {
                var filter = Builders<Users>.Filter.Eq(m => m.ID, ID);
                var update = Builders<Users>.Update.Set("IsActive", 0);
                var result = await collection.FindOneAndUpdateAsync(filter, update);
                return true;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<Users>? Get(ObjectId? ID, string? Username)
        {
            try
            {
                bool result;
                IAsyncCursor<Users> response;
                if (ID != null)
                {
                    response = await collection.FindAsync(m => m.ID == ID);
                }
                else
                {
                    response = await collection.FindAsync(m => m.Username == Username);
                }
                return response.SingleOrDefault();
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<bool>? UpdateEmail(ObjectId ID, string Email)
        {
            try
            {
                var filter = Builders<Users>.Filter.Eq(m => m.ID, ID);
                var update = Builders<Users>.Update.Set(m => m.Email, Email);
                var result = await collection.FindOneAndUpdateAsync(filter, update);
                return true;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<bool>? ChangePassword(ObjectId UserID, string currentPassword, string newPassword)
        {
            try
            {
                var filter = Builders<Users>.Filter.Eq(m => m.ID, UserID) & Builders<Users>.Filter.Eq(m => m.Password, currentPassword);
                var update = Builders<Users>.Update.Set(m => m.Password, newPassword);
                var result = await collection.FindOneAndUpdateAsync(filter, update);
                return true;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<bool>? UpdateUsername(ObjectId ID, string Username)
        {
            try
            {
                var filter = Builders<Users>.Filter.Eq(m => m.ID, ID);
                var update = Builders<Users>.Update.Set(m => m.Username, Username);
                var result = await collection.FindOneAndUpdateAsync(filter, update);
                return true;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<UserAddresses> ManageAddresses(UserAddresses entity, ObjectId userID)
        {
            try
            {
                var filter = Builders<Users>.Filter.Eq(m => m.ID, userID);
                var update = Builders<Users>.Update.Set("Addresses", entity);
                var result = await collection.FindOneAndUpdateAsync(filter, update);
                return entity;
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }
    }
}
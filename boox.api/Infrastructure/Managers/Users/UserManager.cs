using System.Text;
using MongoDB.Bson;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Data.Repo.User;
using boox.api.Infrastructure.Data.Interface.User;
using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Infrastructure.Managers.Users
{
    public class UserManager : IUsers
    {
        private readonly IUsers _repo;
        private readonly IOptions<AppSettings> _settings;

        public UserManager(IOptions<AppSettings> settings)
        {
            _repo = new UserRepository(settings);
            _settings = settings;
        }

        private string generateToken(Infrasructure.Models.Users.Users user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Value.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("id", user.ID.ToString()),
                    new Claim("authType", user.AuthType.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(14),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public async Task<Infrasructure.Models.Users.Users>? Register(Infrasructure.Models.Users.Users entity)
        {
            if (entity.Username == null || entity.Email == null || entity.Password == null)
            {
                throw new Exception("User information missing");
            }

            bool userExists = await CheckEmail(entity.Email, null);
            if (userExists)
            {
                throw new Exception("Email address already registered");
            }

            bool usernameExists = await CheckUsername(entity.Username, null);
            if (usernameExists)
            {
                throw new Exception("Username already exists");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(entity.Password, salt);
            entity.Password = hashedPassword;
            entity.AuthType = 1;

            var result = await _repo.Register(entity);
            if (result != null)
            {
                result.AuthType = entity.AuthType;
                var user = new Infrasructure.Models.Users.Users();
                user.ID = entity.ID;
                user.Username = entity.Username;
                user.Email = entity.Email;
                user.Token = generateToken(result);
                return user;
            }
            throw new Exception("Server error.");
        }

        public async Task<Infrasructure.Models.Users.Users>? Login(Infrasructure.Models.Users.Users entity)
        {
            if (entity.Email == null || entity.Password == null)
            {
                throw new Exception("User information missing");
            }

            var result = await _repo.Login(entity);

            if (result != null && BCrypt.Net.BCrypt.Verify(entity.Password, result.Password))
            {
                var user = new Infrasructure.Models.Users.Users();
                user.ID = result.ID;
                user.Username = result.Username;
                user.Email = result.Email;
                user.Token = generateToken(result);
                return user;
            }

            throw new Exception("Invalid credentials");
        }

        public async Task<bool> CheckEmail(string Email, ObjectId? UserID)
        {
            return await _repo.CheckEmail(Email, UserID);
        }

        public async Task<bool> CheckUsername(string Username, ObjectId? UserID)
        {
            return await _repo.CheckUsername(Username, UserID);
        }

        public async Task<bool>? DeactivateAccount(ObjectId ID)
        {
            return await _repo.DeactivateAccount(ID);
        }

        public async Task<Infrasructure.Models.Users.Users>? Get(ObjectId? ID, string? Username)
        {
            return await _repo.Get(ID, Username);
        }

        public async Task<bool>? UpdateEmail(ObjectId ID, string Email)
        {
            return await _repo.UpdateEmail(ID, Email);
        }

        public async Task<bool>? ChangePassword(ObjectId UserID, string currentPassword, string newPassword)
        {
            var salt = BCrypt.Net.BCrypt.GenerateSalt(10);
            currentPassword = BCrypt.Net.BCrypt.HashPassword(currentPassword, salt);
            newPassword = BCrypt.Net.BCrypt.HashPassword(newPassword, salt);
            return await _repo.ChangePassword(UserID, currentPassword, newPassword);
        }

        public async Task<UserAddresses> ManageAddresses(UserAddresses entity, ObjectId userID)
        {
            return await _repo.ManageAddresses(entity, userID);
        }
    }
}

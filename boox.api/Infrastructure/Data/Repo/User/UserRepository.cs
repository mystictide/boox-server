using Dapper;
using boox.api.Infrasructure.Models.Users;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Data.Repo.Helpers;
using boox.api.Infrastructure.Data.Interface.User;
using boox.api.Infrasructure.Models.Users.Settings;
using boox.api.Infrasructure.Models.Users.Helpers;

namespace boox.api.Infrastructure.Data.Repo.User
{
    public class UserRepository : AppSettings, IUsers
    {
        public async Task<Users>? Register(Users entity)
        {
            ProcessResult result = new ProcessResult();
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Email", entity.Email);
                param.Add("@Username", entity.Username);
                param.Add("@Password", entity.Password);
                param.Add("@AuthType", entity.AuthType);

                string query = $@"
                DECLARE  @result table(ID Int, Email nvarchar(MAX), Username nvarchar(100), Password nvarchar(MAX), AuthType Int, IsActive bit)
	                    INSERT INTO Users (Email, Username, Password, AuthType, IsActive)
	                        OUTPUT INSERTED.* INTO @result
	                        VALUES (@Email, @Username, @Password, 1, 1)
                SELECT *
                FROM @result t";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.State = ProcessState.Error;
                return null;
            }
        }

        public async Task<Users>? Login(Users entity)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Email", entity.Email);

                string WhereClause = @" WHERE (t.Email like '%' + @Email + '%')";

                string query = $@"
                SELECT *
                FROM Users t
                {WhereClause}";

                string addQuery = $@"
                SELECT *
                FROM UserAddresses t
                WHERE UserID = @ID";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query, param);
                    param.Add("@ID", res.ID);
                    res.Addresses = await con.QueryAsync<UserAddresses>(addQuery, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<bool> CheckEmail(string Email, int? UserID)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserID", UserID);
            param.Add("@Email", Email);

            string Query;
            if (UserID.HasValue)
            {
                Query = @"
                SELECT CASE WHEN COUNT(ID) > 0 THEN 1 ELSE 0 END
                FROM Users 
                WHERE Email = @Email AND NOT (ID = @UserID)";
            }
            else
            {
                Query = @"
                SELECT CASE WHEN COUNT(ID) > 0 THEN 1 ELSE 0 END
                FROM Users 
                WHERE Email = @Email";
            }

            using (var con = GetConnection)
            {
                var res = await con.QueryAsync<bool>(Query, param);
                return res.FirstOrDefault();
            }
        }

        public async Task<bool> CheckUsername(string Username, int? UserID)
        {
            DynamicParameters param = new DynamicParameters();
            param.Add("@UserID", UserID);
            param.Add("@Username", Username);

            string Query;
            if (UserID.HasValue)
            {
                Query = @"
                SELECT CASE WHEN COUNT(ID) > 0 THEN 1 ELSE 0 END
                FROM Users 
                WHERE Username = @Username AND NOT (ID = @UserID)";
            }
            else
            {
                Query = @"
                SELECT CASE WHEN COUNT(ID) > 0 THEN 1 ELSE 0 END
                FROM Users 
                WHERE Username = @Username";
            }

            using (var con = GetConnection)
            {
                var res = await con.QueryAsync<bool>(Query, param);
                return res.FirstOrDefault();
            }
        }

        public async Task<bool>? DeactivateAccount(int ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);

                string query = $@"
                UPDATE Users
                SET IsActive = 0
                WHERE ID = @ID";

                using (var connection = GetConnection)
                {
                    await connection.QueryAsync<ProcessResult>(query, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<Users>? Get(int? ID, string? Username)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@Username", Username);

                string WhereClause = @" WHERE t.ID = @ID OR (t.Username like '%' + @Username + '%')";

                string query = $@"
                SELECT *
                FROM Users t
                {WhereClause}";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Users>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<bool>? UpdateEmail(int ID, string Email)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@Mail", Email);

                string query = $@"
                UPDATE Users
                SET Email = @Email
                WHERE ID = @ID";

                using (var connection = GetConnection)
                {
                    await connection.QueryFirstOrDefaultAsync<ProcessResult>(query, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", UserID);
                param.Add("@currentPassword", currentPassword);
                param.Add("@Password", newPassword);

                string query = $@"
                IF EXISTS(SELECT * from Users WHERE ID = @ID AND Password = @currentPassword)
                UPDATE Users
                SET Password = @Password
                WHERE ID = @ID";

                using (var connection = GetConnection)
                {
                    await connection.QueryFirstOrDefaultAsync<ProcessResult>(query, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<bool>? UpdateUsername(int ID, string Username)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@Username", Username);

                string query = $@"
                UPDATE Users
                SET Username = @Username
                WHERE ID = @ID";

                using (var connection = GetConnection)
                {
                    await connection.QueryFirstOrDefaultAsync<ProcessResult>(query, param);
                    return true;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<IEnumerable<UserAddresses>> ManageAddresses(UserAddresses entity, int userID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@userID", userID);
                param.Add("@ID", entity.ID);
                param.Add("@Country", entity.Country);
                param.Add("@City", entity.City);
                param.Add("@Address", entity.Address);
                param.Add("@Phone", entity.Phone);
                param.Add("@Postal", entity.Postal);
                param.Add("@Title", entity.Title);

                string query = $@"
                IF EXISTS(SELECT * from Users WHERE ID = @ID AND Password = @currentPassword)
                UPDATE Users
                SET Password = @Password
                WHERE ID = @ID";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync<UserAddresses>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }
    }
}
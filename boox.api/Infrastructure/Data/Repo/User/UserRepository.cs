using Dapper;
using static Dapper.SqlMapper;
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
                INSERT INTO users (email, username, password, authType)
	                VALUES (@Email, @Username, @Password, 1)
                RETURNING *;";

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
                string WhereClause = $"WHERE (t.email like '%{entity.Email}%');";

                string query = $@"
                SELECT *
                FROM users t
                {WhereClause};";

                string addQuery = $@"
                SELECT *
                FROM useraddresses t
                WHERE userid = @ID;";

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
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE email = @Email AND NOT (id = @UserID);";
            }
            else
            {
                Query = @"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE email = @Email;";
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
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE username = @Username AND NOT (id = @UserID);";
            }
            else
            {
                Query = @"
                SELECT CASE WHEN COUNT(id) > 0 THEN 1 ELSE 0 END
                FROM users 
                WHERE username = @Username;";
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
                UPDATE users
                SET isactive = 0
                WHERE id = @ID;";

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

                string WhereClause = $" WHERE t.id = @ID OR (t.username like '%{Username}%')";

                string query = $@"
                SELECT *
                FROM users t
                {WhereClause};";

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

        public async Task<string>? UpdateEmail(int ID, string Email)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@Email", Email);

                string query = $@"
                UPDATE users
                SET email = @Email
                WHERE id = @ID
                RETURNING email;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<string>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<bool>? ChangePassword(int UserID, string currentPassword, string newPassword)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", UserID);

                string query = $@"
                UPDATE users
                SET password = '{newPassword}'
                WHERE id = @ID;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync(query, param);
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
                UPDATE users
                SET username = @Username
                WHERE id = @ID;";

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
                dynamic identity = entity.ID.HasValue ? entity.ID.Value : "default";
                param.Add("@UserID", userID);
                param.Add("@Country", entity.Country);
                param.Add("@City", entity.City);
                param.Add("@Address", entity.Address);
                param.Add("@Phone", entity.Phone);
                param.Add("@Postal", entity.Postal);
                param.Add("@Title", entity.Title);

                string query = $@"
                INSERT INTO useraddresses (id, userid, title, phone, country, city, address, postal)
	 	                VALUES ({identity}, @UserID, @Title, @Phone, @Country, @City, @Address, @Postal)
                ON CONFLICT (id, userid) DO UPDATE 
                SET title = @Title,
	      	             phone = @Phone,
	      	             country = @Country,
	      	             city = @City,
	      	             address = @Address,
	      	             postal = @Postal;
                SELECT *
                FROM useraddresses t
                WHERE t.userid = @UserID;";

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
        public async Task<IEnumerable<UserAddresses>> DeleteAddress(int ID, int userID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@UserID", userID);

                string query = $@"
                DELETE FROM useraddresses t where t.id = @ID and t.userid = @UserID;
                SELECT *
                FROM useraddresses t
                WHERE t.userid = @UserID;";

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
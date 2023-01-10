using Dapper;
using boox.api.Infrasructure.Models.Helpers;
using boox.api.Infrasructure.Models.Listings;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Models.Listings;
using boox.api.Infrastructure.Data.Repo.Helpers;
using boox.api.Infrastructure.Data.Interface.Listings;

namespace boox.api.Infrastructure.Data.Repo.Listings
{
    public class ListingRepository : AppSettings, IListings
    {
        public async Task<bool>? ToggleListing(int ID, int UserID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@UserID", UserID);

                string query = $@"
                UPDATE listing
                SET isactive = NOT isactive
                WHERE id = @ID AND userid = @UserID
                RETURNING *;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<bool>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return false;
            }
        }

        public async Task<FilteredList<Listing>> FilteredList(FilteredList<Listing> request)
        {
            try
            {
                FilteredList<Listing> result = new FilteredList<Listing>();
                DynamicParameters param = new DynamicParameters();
                param.Add("@PageSize", request.filter.pageSize);

                string WhereClause = $" WHERE t.title like '%{request.filter.Keyword}%)";
                string query_count = $@"  Select Count(t.id) from listing t {WhereClause}";

                string query = $@"
                SELECT *
                FROM listing t
                {WhereClause} 
                ORDER BY t.id ASC 
                OFFSET @StartIndex ROWS
                FETCH NEXT @PageSize ROWS ONLY";

                using (var con = GetConnection)
                {
                    result.totalItems = await con.QueryFirstOrDefaultAsync<int>(query_count, param);
                    request.filter.pager = new Page(result.totalItems, request.filter.pageSize, request.filter.page);
                    param.Add("@StartIndex", request.filter.pager.StartIndex);
                    result.data = await con.QueryAsync<Listing>(query, param);
                    result.filter = request.filter;
                    result.filterModel = request.filterModel;
                    return result;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<Listing>? Get(int? ID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);

                string WhereClause = $" WHERE t.id = @ID";

                string query = $@"
                SELECT *
                FROM listing t
                {WhereClause};";

                string pQuery = $@"
                SELECT *
                FROM photos t
                WHERE t.listingid = @ID;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryFirstOrDefaultAsync<Listing>(query, param);
                    res.Photos = await con.QueryAsync<Photos>(pQuery, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<Listing> ManageListing(Listing entity, int UserID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                dynamic identity = entity.ID.HasValue ? entity.ID.Value : "default";
                param.Add("@UserID", UserID);
                param.Add("@Country", entity.Country);
                param.Add("@City", entity.City);
                param.Add("@Author", entity.Author);
                param.Add("@Edition", entity.Edition);
                param.Add("@Title", entity.Title);
                param.Add("@Date", DateTime.Now);
                param.Add("@Notes", entity.Notes);
                param.Add("@Year", entity.Year);

                string query = $@"
                INSERT INTO listing (id, userid, title, author, country, city, edition, isactive, date, notes, year)
	 	                VALUES ({identity}, @UserID, @Title, @Author, @Country, @City, @Edition, true, @Date, @Notes, @Year)
                ON CONFLICT (id) DO UPDATE 
                SET title = @Title,
	      	             author = @Author,
	      	             country = @Country,
	      	             city = @City,
	      	             edition = @Edition,
                         date = @Date,
                         notes = @Notes,
                         year = @Year
                RETURNING *";

                string gdQuery = $@"
                DELETE FROM listinggenresjunction t WHERE t.listingid = @ID;";

                string gQuery = $@"
                INSERT INTO listinggenresjunction (id, listingid, genreid)
	 	                VALUES (default, @ID, @GenreID);
                SELECT *,
                (select name from genres where id = t.genreid)name
                FROM listinggenresjunction t
                WHERE t.listingid = @ID;";

                string pQuery = $@"
                SELECT *
                FROM photos t
                WHERE t.listingid = @ID;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryFirstOrDefaultAsync<Listing>(query, param);
                    await connection.ExecuteAsync(gdQuery, param);
                    param.Add("@ID", res.ID);
                    foreach (var item in entity.Genre)
                    {
                        param.Add("@GenreID", item.ID);
                        res.Genre = await connection.QueryAsync<Genres>(gQuery, param);
                    }
                    res.Photos = await connection.QueryAsync<Photos>(pQuery, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }
        public async Task<IEnumerable<Photos>> ManagePhotos(Photos entity, int UserID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@Path", entity.Path);
                param.Add("@ListingID", entity.ListingID);
                param.Add("@UserID", UserID);

                string query = $@"
                INSERT INTO listing (id, path, listingid, userid)
	 	                VALUES (default, @Path, @ListingID, @UserID)
                SELECT *
                FROM photos t
                WHERE t.listingid = @ListingID and t.userid = @UserID;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync<Photos>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Photos>> DeletePhoto(int ID, int ListingID, int UserID)
        {
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@ID", ID);
                param.Add("@ListingID", ListingID);
                param.Add("@UserID", UserID);

                string query = $@"
                DELETE FROM photos t where t.id = @ID and t.listingid = @ListingID and t.userid = @UserID;
                SELECT *
                FROM photos t
                WHERE t.listingid = @ListingID and t.userid = @UserID;";

                using (var connection = GetConnection)
                {
                    var res = await connection.QueryAsync<Photos>(query, param);
                    return res;
                }
            }
            catch (Exception ex)
            {
                LogsRepository.CreateLog(ex);
                return null;
            }
        }

        public async Task<IEnumerable<Genres>>? GetGenres()
        {
            try
            {
                string query = $@"
                SELECT *
                FROM genres t;";

                using (var con = GetConnection)
                {
                    var res = await con.QueryAsync<Genres>(query);
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

using System.Diagnostics;
using Dapper.Contrib.Extensions;
using boox.api.Infrasructure.Models.Helpers;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Data.Interface.Helpers;

namespace boox.api.Infrastructure.Data.Repo.Helpers
{
    public class LogsRepository : AppSettings, ILogs
    {
        public async Task<int> Add(Logs entity)
        {
            try
            {
                using (var con = GetConnection)
                {
                    return await con.InsertAsync(entity);
                }
            }
            catch (Exception exception)
            {
                string a = exception.Message;
                return 0;
            }
        }

        public async static void CreateLog(Exception ex, int UserId = 0)
        {
            try
            {
                var st = new StackTrace(ex, true);
                if (st != null)
                {
                    st.GetFrames().Where(k => k.GetFileLineNumber() > 0).ToList().ForEach(async k =>
                    {
                        await new LogsRepository().Add(new Logs()
                        {
                            CreatedDate = DateTime.Now,
                            UserID = UserId,
                            Message = ex.Message,
                            Source = ex.Source + " | " + k,
                            Line = k.GetFileLineNumber()
                        });
                    });
                }
                else
                {
                    await new LogsRepository().Add(new Logs()
                    {
                        CreatedDate = DateTime.Now,
                        UserID = UserId,
                        Message = ex.Message,
                        Source = ex.Source,
                        Line = 0
                    });
                }
            }
            catch (Exception exception)
            {
                await new LogsRepository().Add(new Logs()
                {
                    CreatedDate = DateTime.Now,
                    UserID = 0,
                    Message = exception.Message,
                    Source = exception.Source + " - Error logging",
                    Line = 0
                });
            }
        }
    }
}

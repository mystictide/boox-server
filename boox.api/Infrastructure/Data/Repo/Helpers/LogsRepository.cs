using MongoDB.Driver;
using System.Diagnostics;
using boox.api.Infrasructure.Models.Helpers;
using boox.api.Infrastructure.Models.Helpers;
using boox.api.Infrastructure.Data.Interface.Helpers;
using Microsoft.Extensions.Options;

namespace boox.api.Infrastructure.Data.Repo.Helpers
{
    public class LogsRepository : AppSettings, ILogs
    {
        private readonly IOptions<AppSettings> _settings;

        public LogsRepository(IOptions<AppSettings>? settings = null)
        {
            _settings = settings;
        }

        private static IMongoCollection<Logs> collection = GetDB.GetCollection<Logs>("logs");

        public async Task<int> Add(Logs entity)
        {
            try
            {
                await collection.InsertOneAsync(entity);
                return 1;
            }
            catch (Exception ex)
            {
                string a = ex.Message;
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

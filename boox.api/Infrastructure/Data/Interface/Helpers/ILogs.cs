using boox.api.Infrasructure.Models.Helpers;

namespace boox.api.Infrastructure.Data.Interface.Helpers
{
    public interface ILogs
    {
        Task<int> Add(Logs entity);
    }
}

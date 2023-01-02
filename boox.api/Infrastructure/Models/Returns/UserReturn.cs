using boox.api.Infrasructure.Models.Users.Settings;

namespace boox.api.Infrasructure.Models.Returns
{
    public class UserReturn
    {
        public int ID { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public IEnumerable<UserAddresses>? Addresses { get; set; }
    }
}

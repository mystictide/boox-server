﻿using Dapper.Contrib.Extensions;
using Newtonsoft.Json;

namespace boox.api.Infrasructure.Models.Users.Settings
{
    [Table("UserAddresses")]
    public class UserAddresses
    {
        [Key]
        [JsonProperty("id")]
        public int ID { get; set; }
        [JsonProperty("title")]
        public string? Title { get; set; }
        [JsonProperty("phone")]
        public string? Phone { get; set; }
        [JsonProperty("country")]
        public Countries? Country { get; set; }
        [JsonProperty("city")]
        public string? City { get; set; }
        [JsonProperty("address")]
        public string? Address { get; set; }
        [JsonProperty("postal")]
        public string? Postal { get; set; }

        public class Countries
        {
            public string? label { get; set; }
            public string? value { get; set; }
        }
    }
}
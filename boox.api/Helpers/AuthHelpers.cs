using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace boox.api.Helpers
{
    public class AuthHelpers
    {
        public static bool Authorize(HttpContext context, int AuthorizedAuthType, string secret)
        {
            return ValidateToken(ReadBearerToken(context), AuthorizedAuthType, secret);
        }
        public static ObjectId CurrentUserID(HttpContext context, string secret)
        {
            return ValidateUser(ReadBearerToken(context), secret);
        }
        public static string? ReadBearerToken(HttpContext context)
        {
            try
            {
                string header = (string)context.Request.Headers["Authorization"];
                if (header != null)
                {
                    return header.Substring(7);
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static bool ValidateToken(string? encodedToken, int AuthorizedAuthType, string secret)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secret);
                tokenHandler.ValidateToken(encodedToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var authType = int.Parse(jwtToken.Claims.First(x => x.Type == "authType").Value);
                if (authType >= AuthorizedAuthType)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ObjectId ValidateUser(string? encodedToken, string secret)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(secret);
                tokenHandler.ValidateToken(encodedToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                return new ObjectId(jwtToken.Claims.First(x => x.Type == "id").Value);
            }
            catch (Exception)
            {
                return new ObjectId();
            }
        }
    }
}
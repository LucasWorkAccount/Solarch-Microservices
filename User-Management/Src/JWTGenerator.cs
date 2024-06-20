

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace User_Management.Model;

public class JWTGenerator
{
    public static String Generate(User user)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var key = new SymmetricSecurityKey("m4EzzUWJMBs34K&GaXyiawwj&mdvFniisEDZD&i26bCtykpd4g7!zgyHm#GpNMt*7#LuU*xC&Q4"u8.ToArray());
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken( claims: claims, expires: DateTime.Now.AddMinutes(60),signingCredentials: cred );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return jwt;
    }
}

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NomadicBook.Utils
{
    public static class JWT
    {
        public static void AddJWT(this IServiceCollection services)
        {
            services.AddAuthentication(option =>
            {
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
           .AddJwtBearer(option =>
           {
               option.RequireHttpsMetadata = false;
               option.Events = new JwtBearerEvents()
               {
                   OnAuthenticationFailed = context =>
                   {
                       //Token expired
                       if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                       {
                           context.Response.Headers.Add("Token-Expired", "true");
                       }
                       return Task.CompletedTask;
                   },
               };

               option.TokenValidationParameters = new TokenValidationParameters
               {
                   // 是否驗證失效時間
                   ValidateLifetime = true,
                   ClockSkew = TimeSpan.FromSeconds(30),
                   ValidateAudience = false,//驗證是否是可以使用的Client
                   // 這裡採用動態驗證的方式，在重新登陸時，重新整理token，舊token就強制失效了
                   AudienceValidator = AudienceValidator,
                   ValidateIssuer = false,// 是否驗證Issuer(驗證是誰核發的?)
                   // 是否驗證SecurityKey
                   ValidateIssuerSigningKey = true,
                   IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(IConst.SecurityKey))
                   //用來雜湊打亂的關鍵數值
               };
           });
        }
        private static bool AudienceValidator(IEnumerable<string> audiences, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            return Audiences.IsNewestAudience(audiences.FirstOrDefault());
        }
    }
}

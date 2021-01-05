using System;

namespace IdentityApp.Infrastructure.Options
{
    public static class HerokuConfiguration
    {
        public static string GetHerokuConnectionString()
        {
            string connectionUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
            string issuer = Environment.GetEnvironmentVariable("Issuer");
            string audience = Environment.GetEnvironmentVariable("Audience");
            string key = Environment.GetEnvironmentVariable("Key");
            int lifetime = int.Parse(Environment.GetEnvironmentVariable("Lifetime"));

            var databaseUri = new Uri(connectionUrl);

            string db = databaseUri.LocalPath.TrimStart('/');
            string[] userInfo = databaseUri.UserInfo.Split(':', StringSplitOptions.RemoveEmptyEntries);

            return $"User ID={userInfo[0]};Password={userInfo[1]};Host={databaseUri.Host};Port={databaseUri.Port};Database={db};Pooling=true;SSL Mode=Require;Trust Server Certificate=True;";
        }
    }
}

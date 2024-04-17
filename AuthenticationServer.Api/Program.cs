using AuthenticationServer.Data;
using AuthenticationServer.Data.Repositories;
using AuthenticationServer.Models;
using AuthenticationServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace AuthenticationServer.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AuthenticationDbContext>(option =>
               option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultDb")));
            builder.Services.AddTransient<IAppUserRepository, AppUserRepository>();

            var authenticationConfiguration = new AuthenticationConfiguration();
            builder.Configuration.Bind("Authentication", authenticationConfiguration);
            builder.Services.AddSingleton(authenticationConfiguration);

            builder.Services.AddTransient<ITokenGenerator, JwtTokenGenerator>();
            builder.Services.AddTransient<IPasswordHasher, BcryptPasswordHasher>();
            builder.Services.AddTransient<IAuthenticationService, AuthenticationService>();

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

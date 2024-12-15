
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MoviesAPI.Models;

namespace MoviesAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddCors();

            builder.Services.AddSwaggerGen(options => 
            {
                options.SwaggerDoc(name: "v1", info: new OpenApiInfo
                {
                    Version = "v1",
                    Title = "TestApi",
                    Description="My First API",
                    TermsOfService= new Uri(uriString: "https://www.google.com"),
                    Contact =  new OpenApiContact
                    {
                        Name = "Hady",
                        Email = "test@domain.com",
                        Url= new Uri(uriString: "https://www.google.com")
                    },
                   License = new OpenApiLicense
                   {
                       Name = "My License",
                       Url = new Uri(uriString: "https://www.google.com")
                   }
                });
                options.AddSecurityDefinition(name:"Bearer", securityScheme: new OpenApiSecurityScheme
                {
                    Name="Authorization",
                    Type= SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter Your JWT Key"
                });

                options.AddSecurityRequirement(securityRequirement: new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference =new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id= "Bearer"
                            },
                            Name = "Bearer",
                             In = ParameterLocation.Header
                        },
                        new List<string>()
                    }
                });

            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(c => c.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}

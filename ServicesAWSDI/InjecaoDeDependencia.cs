/*using Amazon.Rekognition;
using Amazon.Runtime;
using Amazon.S3;
using Curso.ProjetoAWS.Data.Repositorios;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjetoAWS.Application.Services;
using ProjetoAWS.Lib.Data;
using ProjetoAWS.ServicesAWS;
using ServicesAWS;

namespace ProjetoAWS.ServicesAWSDI
{
    public static class InjecaoDeDependencia
    {
        private static string[] args;
        public static IServiceCollection AddConfig( this IServiceCollection services, IConfiguration configuration)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            services.AddDbContext<ProjetoAWSContext>(
                       conn => conn.UseNpgsql(configuration.GetConnectionString("ProjetoAWSDB"))
                       .UseSnakeCaseNamingConvention());

            services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            services.AddScoped<IUsuarioApplication, UsuarioApplication>();
            services.AddScoped<IServicesDaAws, ServicesDaAws>();
            
            var awsOptions = configuration.GetAWSOptions();
            awsOptions.Credentials = new EnvironmentVariablesAWSCredentials();
            services.AddDefaultAWSOptions(awsOptions);
            services.AddAWSService<IAmazonS3>();
            services.AddScoped<AmazonRekognitionClient>();
            return services;
        }
    }
}


       
/*builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();*/



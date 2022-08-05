using Amazon.Rekognition;
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

namespace ServicesAWSDI
{
    public class InjecaoDeDependeciaProgram
    {
        public void InjetarDependenciaProgram(WebApplicationBuilder builder)
        {            

            builder.Services.AddDbContext<ProjetoAWSContext>(conn => conn.UseNpgsql
            (builder.Configuration.GetConnectionString("ProjetoAWSDB")).UseSnakeCaseNamingConvention());

            builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();

            builder.Services.AddScoped<IUsuarioApplication, UsuarioApplication>();

            builder.Services.AddScoped<IServicesDaAws, ServicesDaAws>();

           
            var awsOptions = builder.Configuration.GetAWSOptions();
            awsOptions.Credentials = new EnvironmentVariablesAWSCredentials();
            builder.Services.AddDefaultAWSOptions(awsOptions);

            builder.Services.AddAWSService<IAmazonS3>();   

            builder.Services.AddScoped<AmazonRekognitionClient>();      

                 
        }
    }
}
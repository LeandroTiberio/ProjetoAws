using Amazon.Rekognition;
using Amazon.Runtime;
using Amazon.S3;
using Curso.ProjetoAWS.Data.Repositorios;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjetoAWS.Lib.Data;

namespace ProjetoAWS.Application.Services
{
    public static class InjecaoDeDependecia
    {
        public static void InjetarDependencias( IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddDbContext<ProjetoAWSContext>(
                       conn => conn.UseNpgsql(configuration.GetConnectionString("ProjetoAWSDB"))
                       .UseSnakeCaseNamingConvention());

            collection.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
            collection.AddScoped<IUsuarioApplication, UsuarioApplication>();
            
            var awsOptions = configuration.GetAWSOptions();
            awsOptions.Credentials = new EnvironmentVariablesAWSCredentials();
            collection.AddDefaultAWSOptions(awsOptions);
            collection.AddAWSService<IAmazonS3>();
            collection.AddScoped<AmazonRekognitionClient>();
        }
    }
}

       
/*builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();*/



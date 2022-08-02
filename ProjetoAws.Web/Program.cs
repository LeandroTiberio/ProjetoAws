using Amazon.Rekognition;
using Amazon.Runtime;
using Amazon.S3;
using Curso.ProjetoAWS.Data.Repositorios;
using Curso.ProjetoAWS.Lib.Data.Repositorios.Interface;
using Microsoft.EntityFrameworkCore;
using ProjetoAws.Web.Controllers.Middlewares;
using ProjetoAWS.Application.Services;
using ProjetoAWS.Lib.Data;
using static System.Net.Mime.MediaTypeNames;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
                .AddNewtonsoftJson(x => x.SerializerSettings.ReferenceLoopHandling =
                Newtonsoft.Json.ReferenceLoopHandling.Ignore);

/*InjecaoDeDependecia(builder.Services, builder.Configuration);*/

builder.Services.AddConfig(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors (p => p.AddPolicy("corsdodev",cors => 
{
    cors.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors("corsdodev");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


/*builder.Services.AddDbContext<ProjetoAWSContext>(
        conn => conn.UseNpgsql(builder.Configuration.GetConnectionString("ProjetoAWSDB"))
        .UseSnakeCaseNamingConvention());

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var awsOptions = builder.Configuration.GetAWSOptions();
awsOptions.Credentials = new EnvironmentVariablesAWSCredentials();
builder.Services.AddDefaultAWSOptions(awsOptions);
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<AmazonRekognitionClient>();

builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();*/

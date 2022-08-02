using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    
    public class RekognitionController : ControllerBase
    {
        private readonly AmazonRekognitionClient _rekognitionClient;
        public RekognitionController(AmazonRekognitionClient rekognitionClient)
        {
            _rekognitionClient = rekognitionClient;
        }

        [HttpPost("Comparar")]
        public async Task<IActionResult> CompararRostoAsync(string nomeArquivoS3, IFormFile fotoLogin)
        { 
            using (var memoryStream = new MemoryStream())
            {
                var request = new CompareFacesRequest();
                var requestsourceImagem = new Image()
                {
                    S3Object = new S3Object()
                    {
                        Bucket = "imagem-aulas",
                        Name = nomeArquivoS3
                    }
                };
                    
                await fotoLogin.CopyToAsync(memoryStream);

                var requesttargetImagem = new Image()
                {
                    Bytes = memoryStream
                };
                request.SourceImage = requestsourceImagem;  
                request.TargetImage = requesttargetImagem;

                var resposta = await _rekognitionClient.CompareFacesAsync(request);
                return Ok(resposta);
            }
                
        }
                
        [HttpGet]
        public async Task<IActionResult> AnalisarRostoAsync(string nomeArquivo)
        {
            var entrada = new DetectFacesRequest();
            var imagem = new Image();

            var s3Object = new S3Object()
            {
                Bucket = "imagem-Aulas",
                Name = nomeArquivo
            };

            imagem.S3Object = s3Object;
            entrada.Image = imagem;
            entrada.Attributes = new List<string>(){"ALL"};
            
            var resposta = await _rekognitionClient.DetectFacesAsync(entrada);
            
            if(resposta.FaceDetails.Count == 1 && resposta.FaceDetails.First().Eyeglasses.Value == false)
            {
                return Ok(resposta);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}

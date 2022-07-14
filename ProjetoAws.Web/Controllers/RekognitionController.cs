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
        public async Task<IActionResult> CompararRosto(string nomeArquivoS3, IFormFile faceLogin)
        { 
            using (var memoryStream = new MemoryStream())
            {
                var sourceImage = new Image();
                var Request = new CompareFacesRequest();
                var s3Object = new S3Object()
                {
                    Bucket = "imagem-Aulas",
                    Name = nomeArquivoS3
                };

                var targetImage = new Image();
                var bytes = new MemoryStream();
                {
                    return Ok();
                }
            
                
            }
        }
                
        [HttpGet]
        public async Task<IActionResult> AnalisarRosto(string nomeArquivo)
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

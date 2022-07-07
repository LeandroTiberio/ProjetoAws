using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace ProjetoAws.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ImagensController : ControllerBase
    {
        private readonly IAmazonS3 _amazonS3;
        public ImagensController(IAmazonS3 amazonS3)
        {
            _amazonS3 = amazonS3;
        }

        [HttpPost]
        /*public async Task<IActionResult> CriarImagem()
        {
            var resposta = await   
        }*/

        [HttpGet("bucket")]
        public async Task<IActionResult> ListarBuckets()
        {
            var resposta = await _amazonS3.ListBucketsAsync();
            
            return Ok(resposta.Buckets);
        }

        [HttpPost("bucket")]
        public async Task<IActionResult> CriarBucket(string nomeBucket){
            var resposta = await _amazonS3.PutBucketAsync(nomeBucket);
            return Ok(resposta);
        }
    }
}
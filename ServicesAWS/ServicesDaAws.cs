namespace ServicesAWS
{
    public class ServicesDaAws : IServicesDaAws
    {
        private readonly IAmazonS3 _amazonS3;
        private static readonly List<string> _extensoesImagem =
        new List<string>() { "image/jpeg", "image/png", "image/jpg"};
        private readonly AmazonRekognitionClient _rekognitionClient;
        public ServicesDaAws(IAmazonS3 amazonS3, AmazonRekognitionClient rekognitionClient )
        {
            _IAmazonS3 _amazonS3;
            _RekognitionClient rekognitionClient;
        }
        public async Task CadastrarImagem(int id, IFormFile imagem)
        {
            var nomeArquivo = await SalvarNoS3(imagem);
            var imagemValida = await ValidarImagem(nomeArquivo);
            if (imagemValida)
            {
                await _repositorio.AtualizarUrlImagemCadastro(id, nomeArquivo);
            }
            else
            {
                var response = await _amazonS3.DeleteObjectAsync("imagens-aulas", nomeArquivo);
                throw new ErroDeValidacaoException("Imagem inválida!");
            }
        }
        private async Task<string> SalvarNoS3(IFormFile imagem)
        {
            if (!_extensoesImagem.Contains(imagem.ContentType))
            {
                throw new ErroDeValidacaoException("Tipo de foto inválido!");
            }
            using (var streamDaImagem = new MemoryStream())
            {
                await imagem.CopyToAsync(streamDaImagem);
                var request = new PutObjectRequest();
                request.Key = "reconhecimento" + imagem.FileName;
                request.BucketName = "imagem-aulas";
                request.InputStream = streamDaImagem;

                var resposta = await _amazonS3.PutObjectAsync(request);
                return request.Key;
            }
        }
        private async Task<bool> ValidarImagem(string nomeArquivo)
        {
            var request = new DetectFacesRequest();
            var imagem = new Image();

            var s3Object = new Amazon.Rekognition.Model.S3Object()
            {
                Bucket = "imagem-aulas",
                Name = nomeArquivo

            };

            imagem.S3Object = s3Object;
            request.Image = imagem;
            request.Attributes = new List<string>() { "ALL" };

            var response = await _rekognitionClient.DetectFacesAsync(request);

            if (response.FaceDetails.Count == 1 && response.FaceDetails.First().Eyeglasses.Value == false)
            {
                return true;
            }
            return false;
        }
        public async Task LoginImagem(int id, IFormFile imagem)
        {
            var usuario = await _repositorio.BuscarPorId(id);
            var verificacao = await VerificarImagem(usuario.UrlImagemCadastro, imagem);
            if (verificacao == false)
            {
                throw new ErroDeValidacaoException("Face não compativel com cadastro!");
            }
        }
        private async Task<bool> VerificarImagem(string nomeArquivoS3, IFormFile fotoLogin)
        {
            using (var memStream = new MemoryStream())
            {
                var request = new CompareFacesRequest();

                var requestSource = new Image()
                {
                    S3Object = new Amazon.Rekognition.Model.S3Object()
                    {
                        Bucket = "imagem-aulas",
                        Name = nomeArquivoS3
                    }
                };

                await fotoLogin.CopyToAsync(memStream);
                var requestTarget = new Image()
                {
                    Bytes = memStream
                };

                request.SourceImage = requestSource;
                request.TargetImage = requestTarget;

                var response = await _rekognitionClient.CompareFacesAsync(request);
                if (response.FaceMatches.Count == 1 && response.FaceMatches.First().Similarity >= 90)
                {
                    return true;
                }
                return false;
            }
        }
    }
}
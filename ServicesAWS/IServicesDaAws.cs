using Microsoft.AspNetCore.Http;

namespace ServicesAWS
{
    public interface IServicesDaAws
    {
        Task<string> SalvarNoS3(IFormFile imagem);
        Task<bool> ValidarImagem(string nomeArquivo);
        Task<bool> VerificarImagem(string nomeArquivoS3, IFormFile fotoLogin);
        Task DeletarImagemNoS3(string nomeBucket, string nomeArquivo);

    }
}
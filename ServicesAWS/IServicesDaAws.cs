namespace ServicesAWS
{
    public interface IServicesDaAws
    {
        Task<string> SalvarNoS3(IFormFile imagem);
        Task<bool> ValidarImagem(string nomeArquivo);
        Task LoginImagem(int id, IFormFile imagem);
        Task<bool> VerificarImagem(string nomeArquivoS3, IFormFile fotoLogin);

    }
}
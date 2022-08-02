namespace ServicesAWS
{
    public class IServicesDaAws
    {
        Task CadastrarImagem(int id, IFormFile imagem);
        Task<bool> ValidarImagem(string nomeArquivo);
        Task LoginImagem(int id, IFormFile imagem);
    }
}
namespace ProjetoAWS.Lib.Models
{
    public class ModelBase
    {
        public int Id { get; private set; }

        public ModelBase(int id)
        {
            Id = id;
        }
        protected ModelBase ()
        {

        }
       
    }
}
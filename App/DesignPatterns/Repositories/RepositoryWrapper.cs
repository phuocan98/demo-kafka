using Project.App.Databases;

namespace Repository
{
    public interface IRepositoryWrapper
    {
        void SaveChanges();
    }
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private readonly MariaDBContext DbContext;
        public RepositoryWrapper(MariaDBContext dbContext)
        {
            DbContext = dbContext;
        }


        public void SaveChanges()
        {
            DbContext.SaveChanges();
        }
    }
}

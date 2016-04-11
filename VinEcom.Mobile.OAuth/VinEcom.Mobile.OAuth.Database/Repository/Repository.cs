using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinEcom.Mobile.OAuth.Database.Repository
{
    public class Repository<T> : IDisposable, IRepository<T> where T : class
    {
        protected OAuthDbContext dbContext;

        public Repository(Func<OAuthDbContext> dbContextFactory)
        {
            this.dbContext = dbContextFactory.Invoke();
        }

        public virtual void Save(T aggregate)
        {
            var entry = this.dbContext.Entry(aggregate);

            if (entry.State == System.Data.Entity.EntityState.Detached)
                this.dbContext.Set<T>().Add(aggregate);
        }

        public T Find(int id)
        {
            return this.dbContext.Set<T>().Find(id);
        }

        public T Find(Guid id)
        {
            return this.dbContext.Set<T>().Find(id);
        }

        public T Find(string email)
        {
            return this.dbContext.Set<T>().Find(email);
        }

        public int SaveChanges()
        {
            return this.dbContext.SaveChanges();
        }

        public void Dispose()
        {
            this.dbContext.Dispose();
        }
    }
}

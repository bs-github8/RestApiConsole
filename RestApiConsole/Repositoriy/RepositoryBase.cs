using Microsoft.EntityFrameworkCore;
using RestApiConsole.DataBase;
using RestApiConsole.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RestApiConsole.Repository
{
    /// <summary>
    /// Реалиация обобщённого класса для работы с моделями
    /// </summary>
 
    public class RepositoryBase<T> : IRepository<T> where T : BaseModel
    {
        private DatabaseContext Context { get; set; }
        public RepositoryBase(DatabaseContext context)
        {
            Context = context;
        }

        public long Create(T model)
        {
            Context.Set<T>().Add(model);
            Context.SaveChanges();
            return model.Id;
        }

        public void Delete(long id)
        {
            var toDelete = Context.Set<T>().FirstOrDefault(m => m.Id == id);
            Context.Set<T>().Remove(toDelete);
            Context.SaveChanges();
        }

        public List<T> GetAll()
        {
            return Context.Set<T>().ToList();
        }

        public bool Update(T model)
        {
            var tmp = Context.Set<T>().FirstOrDefault(m => m.Id == model.Id);

            if (tmp != null)
            {
                tmp.setData(model);
                Context.Update(tmp);
                Context.SaveChanges();

                return true;
            }
            
            return false;
        }

        public T Get(long id)
        {
            return Context.Set<T>().FirstOrDefault(m => m.Id == id);
        }
    }
}

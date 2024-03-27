using RestApiConsole.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Repository
{
    /// <summary>
    /// Интерфейс для работы с моделями в обощённом виде.
    /// </summary>
    internal interface IRepository<T> where T : BaseModel
    {
        public List<T> GetAll();
        public T Get(long id);
        public long Create(T model);
        public bool Update(T model);
        public void Delete(long id);
    }

}

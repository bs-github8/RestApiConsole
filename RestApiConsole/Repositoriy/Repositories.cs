using RestApiConsole.DataBase;
using RestApiConsole.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Repository
{
    /// <summary>
    /// Класс для доступа ко всем данным
    /// </summary>
    public class Repositories
    {
        private static DatabaseContext databaseContext = new DatabaseContext();

        public RepositoryBase<DrillBlock> DrillBlock = new RepositoryBase<DrillBlock>(databaseContext);
        public RepositoryBase<DrillBlockPoints> DrillBlockPoints = new RepositoryBase<DrillBlockPoints>(databaseContext);
        public RepositoryBase<Hole> Hole = new RepositoryBase<Hole>(databaseContext);
        public RepositoryBase<HolePoints> HolePoints = new RepositoryBase<HolePoints>(databaseContext);
    }
}

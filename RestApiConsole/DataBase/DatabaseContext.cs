using Microsoft.EntityFrameworkCore;
using RestApiConsole.DataBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.DataBase
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext() : base()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (Program.settings.posgresConnectionString.Length == 0)
            {
                throw new Exception("Пустая стока подключения к PosgreeSql");
            }

            optionsBuilder.UseNpgsql(Program.settings.posgresConnectionString);
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        public DbSet<DrillBlock> DrillBlocks { get; set; }
        public DbSet<DrillBlockPoints> DrillBlockPoints { get; set; }
        public DbSet<Hole> holes { get; set; }
        public DbSet<HolePoints> HolePoints { get; set; }
    }
}

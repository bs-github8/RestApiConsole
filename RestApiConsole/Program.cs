using RestApiConsole.DataBase;
using RestApiConsole.Repository;

namespace RestApiConsole
{
    /*
Задание:
Реализовать веб-службу REST API. В качестве БД использовать PostreSQL + EntityFramework Есть несколько сущностей:

Блок обуривания (DrillBlock), поля: Id, Name, UpdateDate
Скважина (Hole), поля: Id, Name, DrillBlockId, DEPTH
Точки блока (DrillBlockPoints), соединяются последовательно, являются географическим контуром блока обуривания. Поля: Id, DrillBlockId, Sequence, X, Y, Z
Точки скважин (HolePoints) - координаты скважин. Поля: Id, HoleId, X, Y, Z
Реализовать CRUD для перечисленных сущностей. Данные передаются в формате JSON.

Сделать на основе консоли и .NET 5-8.
     */
    internal class Program
    {
        public static Settings settings;
        static void Main(string[] args)
        {
            settings = new Settings();

            while (settings.anySettingEmpty)
            {
                Console.WriteLine("Необходимы настройки.");

                if (settings.debugTcpPort == 0)
                {
                    Console.WriteLine("Необходимо задать TCP порт для сервера для отладки. Введите досустимый порт:");
                    ushort port;
                    if (ushort.TryParse(Console.ReadLine(), out port))
                    {
                        settings.debugTcpPort = port;
                        settings.saveSetting();
                    }
                }

                if (settings.restTcpPort == 0)
                {
                    Console.WriteLine("Необходимо задать TCP порт для REST сервера. Введите досустимый порт:");
                    ushort port;
                    if (ushort.TryParse(Console.ReadLine(), out port))
                    {
                        settings.restTcpPort = port;
                        settings.saveSetting();
                    }
                }

                if (settings.posgresConnectionString == String.Empty)
                {
                    Console.WriteLine("Задайте строку подключения к PosgreeSql.\nПример строки подключения: Host=localhost;Port=5432;Database=SomeBase;Username=SomeUser;Password=SomePassword");

                    settings.posgresConnectionString = Console.ReadLine();
                    settings.saveSetting();
                }

                Console.WriteLine("Настройки хроняться в файле Settings.xml рядом с исполняемым файлом.");
            }

            RestServer rs = new RestServer(settings.restTcpPort);
            rs.startServer();

            DebugServer ds = new DebugServer(settings.debugTcpPort);
            ds.startServer(rs.restUri);

            Console.ReadKey();

            rs.stopServer();
            ds.stopServer();
        }
    }
}

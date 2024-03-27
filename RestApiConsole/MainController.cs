using RestApiConsole.Controllers;
using RestApiConsole.DataBase.Models;
using RestApiConsole.Repository;
using System.ComponentModel;
using System.Runtime;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RestApiConsole
{
    /// <summary>
    /// Обрабатывает полученные запросы от RestServer
    /// </summary>
    sealed class MainController
    {
        private Repositories repositories = new Repositories();
        private string restUri;
        private JsonSerializerOptions options = new JsonSerializerOptions() { WriteIndented = true };

        private List<BaseController> controllers = new List<BaseController>();

        private string acceptedQueries = string.Empty;

        public MainController(string restUri)
        {
            this.restUri = restUri;
            options.Converters.Add(new CustomDateTimeConverter("yyyy-MM-dd HH:mm"));

            // Пожалуй, разумнее снаружи их добавлять, но для простоты будут тут
            controllers.Add(new DrillBlockController(repositories));
            controllers.Add(new DrillBlockPointsController(repositories));
            controllers.Add(new HoleController(repositories));
            controllers.Add(new HolePointsController(repositories));

            acceptedQueries = "Список поддерживаемых запросов:\n";
            foreach (var c in controllers)
            {
                acceptedQueries += $"GET {restUri}{c.GetType().Name}\n";
            }
        }

        public string onQuery(string verb, string controll, Dictionary<string, string> parameters)
        {
            ToResponce toResponce = null;

            if (controll == string.Empty)
            {
                toResponce = new ToResponce();
                toResponce.actions = acceptedQueries;
            }
            else
            {
                controll += "Controller";

                foreach (var c in controllers)
                {
                    if (c.GetType().Name.Equals(controll))
                    {
                        toResponce = c.onQuery(verb, parameters);
                        toResponce.actions = c.getSupportedQueries(restUri);
                        break;
                    }
                }

                if (toResponce == null)
                {
                    toResponce = new ToResponce();
                    toResponce.actions = acceptedQueries;
                    toResponce.error = "Такой сущности не существует";
                }
            }

            return JsonSerializer.Serialize(toResponce, options);
        }
    }

    sealed class CustomDateTimeConverter : JsonConverter<DateTime>
    {
        private readonly string Format;
        public CustomDateTimeConverter(string format)
        {
            Format = format;
        }
        public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options)
        {
            writer.WriteStringValue(date.ToString(Format));
        }
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.ParseExact(reader.GetString(), Format, null);
        }
    }
}


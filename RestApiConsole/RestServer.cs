using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Text.Json;

namespace RestApiConsole
{
    /// <summary>
    /// Класс выполняет функции RestServer'a: принимает HTTP запросы и получив необходимые данные передаёт их в MainController, который реализует бизнес-логику.
    /// </summary>
    sealed class RestServer
    {
        private MainController mainController;        

        public readonly string restUri;
        public RestServer(ushort port)
        {
            restUri = $"{host}:{port}/{apiPath}";
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(restUri);
            mainController  = new MainController(restUri);
        }


        /// <summary>
        /// Обработка входлящих HTTP запросов
        /// </summary>
        private Thread incomeQueriesThread;

        public bool startServer()
        {
            bool success = false;

            try
            {
                httpListener.Start();
                incomeQueriesThread = new Thread(incomeQueries);
                incomeQueriesThread.Start();

                Console.WriteLine("Успешно запущен севрер для " + restUri);

                success = true;
                run = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return success;
        }

        private HttpListener httpListener;
        private ushort port;
        private bool run;
        private string host = "http://localhost";
        private string apiPath = "api/";



        public async Task stopServer()
        {
            run = false;

            using (var client = new HttpClient())
            {
                using var result = client.GetAsync(restUri);
            }
        }

        /// <summary>
        /// Разбор входящих запросов
        /// </summary>
        private async void incomeQueries()
        {
            while (run)
            {
                var context = await httpListener.GetContextAsync();

                Console.WriteLine($"Пришёл запрос {context.Request.Url}");
                var request = context.Request;
                string verb = context.Request.HttpMethod;
                string tmp = context.Request.Url.AbsolutePath.Substring(apiPath.Length).Trim('/');
                string controller;
                Dictionary<string, string> parameters = new Dictionary<string, string>();

                if (verb == "OPTIONS")
                {
                    // Заплатка для AJAX запросов. Браузер проверяет, может ли он слать DELETE и PUT запросы.
                    context.Response.StatusCode = 200;
                    byte[] buffer = Encoding.UTF8.GetBytes("");
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, DELETE, PUT");
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    context.Response.ContentLength64 = buffer.Length;
                    using Stream output = context.Response.OutputStream;
                    await output.WriteAsync(buffer);
                    await output.FlushAsync();

                }
                else
                {
                    if (tmp.IndexOf('/') == -1)
                    {
                        controller = tmp;
                    }
                    else
                    {
                        var splited = tmp.Split('/');
                        controller = splited[0];
                        parameters["Id"] = splited[1];
                    }

                    Console.WriteLine($"{verb} {controller}");


                    if ((request.HttpMethod == "POST" || request.HttpMethod == "PUT") && request.ContentType != null && request.ContentType.StartsWith("application/x-www-form-urlencoded"))
                    {
                        NameValueCollection paramsCollection = null;

                        System.IO.Stream body = request.InputStream;
                        System.Text.Encoding encoding = request.ContentEncoding;
                        System.IO.StreamReader reader = new System.IO.StreamReader(body, encoding);

                        string formData = reader.ReadToEnd();

                        if (formData != null)
                        {
                            paramsCollection = HttpUtility.ParseQueryString(formData);

                            if (paramsCollection != null)
                            {
                                for (int i = 0; i < paramsCollection.AllKeys.Length; i++)
                                {
                                    parameters[paramsCollection.GetKey(i)] = paramsCollection.GetValues(i).Length == 1 ? paramsCollection.GetValues(i)[0] : null;
                                }
                            }
                        }
                    }

                    var responseText = mainController.onQuery(verb, controller, parameters);

                    context.Response.StatusCode = 200;
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    context.Response.Headers.Add("Access-Control-Allow-Origin", "*");
                    context.Response.Headers.Add("Access-Control-Allow-Methods", "POST, GET, DELETE, PUT");
                    context.Response.ContentType = "text/plain; charset=utf-8";
                    context.Response.ContentLength64 = buffer.Length;
                    using Stream output = context.Response.OutputStream;
                    await output.WriteAsync(buffer);
                    await output.FlushAsync();
                }
            }
        }
    }
}

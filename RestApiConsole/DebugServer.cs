using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace RestApiConsole
{
    /// <summary>
    /// Нужен для отладки. Запускает браузер и открывает нужную страницу
    /// </summary>
    internal class DebugServer
    {
        private HttpListener httpListener;
        private ushort port;
        private bool success;
        private bool run;
        private string host = "http://localhost";
        private Thread incomeQueriesThread;
        private const string debugPath = "Debug/";
        private string restUri;
        public readonly string debugUri;

        public DebugServer(ushort port)
        {
            debugUri = $"{host}:{port}/{debugPath}";
            httpListener = new HttpListener();
            httpListener.Prefixes.Add(debugUri);
        }

        public bool startServer(string restUri)
        {
            this.restUri = restUri;
            bool success = false;

            run = false;

            try
            {
                httpListener.Start();

                incomeQueriesThread = new Thread(incomeQueries);
                incomeQueriesThread.Start();

                Console.WriteLine("Запущен отладочный сервер " + debugUri);
                run = true;
                success = true;

                System.Diagnostics.Process.Start(
                    new ProcessStartInfo
                    {
                        FileName = debugUri,
                        UseShellExecute = true
                    }
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine("Отладочный сервер не был запущен " + ex.Message);
            }

            return success;
        }

        public async Task stopServer()
        {
            run = false;

            using (var client = new HttpClient())
            {
                using var result = client.GetAsync(debugUri);
            }
        }

        private async void incomeQueries()
        {
            while (run)
            {
                var context = await httpListener.GetContextAsync();
                var request = context.Request;
                var response = context.Response;

                context.Response.StatusCode = 200;

                string responseText = "";

                switch (request.RawUrl)
                {
                    case $"/{debugPath}":
                        responseText = new StreamReader(ReadResource("RestApiConsole.Resources.Index.html")).ReadToEnd();
                        responseText = responseText.Replace("API_URL", restUri).Replace("DEBUG_URI", debugUri); ;
                        break;
                    case $"/{debugPath}demo-rest-api.png":
                        var stream = ReadResource("RestApiConsole.Resources.demo-rest-api.png");


                        response.ContentType = "application/octet-stream";
                        response.ContentLength64 = stream.Length;
                        response.AddHeader(
                            "Content-Disposition",
                            "Attachment; filename=\"demo-rest-api.png\"");
                        stream.CopyTo(response.OutputStream);

                        break;
                }

                if (responseText.Length != 0)
                { 
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);
                    context.Response.ContentLength64 = buffer.Length;
                    using Stream output = context.Response.OutputStream;

                    await output.WriteAsync(buffer);
            
                    await output.FlushAsync();
                }
            }
        }

        public Stream ReadResource(string name)
        {
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            if (!name.StartsWith(nameof(RestApiConsole)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            return assembly.GetManifestResourceStream(resourcePath);
        }
    }
}

using RestApiConsole.DataBase.Models;
using RestApiConsole.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Controllers
{
    public class HolePointsController : BaseController
    {
        public HolePointsController(Repositories repositories) : base(repositories)
        {
        }

        public override string getSupportedQueries(string restUri)
        {
            return $"GET {restUri}HolePoints\n" +
                $"GET {restUri}HolePoints/id/\n" +
                $"POST {restUri}HolePoints\n" +
                $"PUT {restUri}HolePoints\n" +
                $"DELETE {restUri}HolePoints/id/";
        }

        public override ToResponce onQuery(string verb, Dictionary<string, string> parameters)
        {
            ToResponce toResponce = new ToResponce();

            switch (verb)
            {
                case "GET":
                    if (parameters.ContainsKey("Id"))
                    {
                        long id;
                        if (long.TryParse(parameters["Id"], out id))
                        {
                            try
                            {
                                var row = repositories.HolePoints.Get(id);
                                toResponce.model = new object[] { row };
                            }
                            catch (Exception e)
                            {
                                toResponce.error = e.InnerException.Message;
                            }
                        }
                        else
                        {
                            toResponce.error = "Не валидный идентификатор";
                        }
                    }
                    else
                    {
                        toResponce.model = repositories.HolePoints.GetAll().ToArray();
                    }
                    break;
                case "POST":
                    {
                        HolePoints holePoints = new HolePoints();

                        parameters["Id"] = "0";

                        if (tryParce(parameters, ref holePoints))
                        {
                            try
                            {
                                var res = repositories.Hole.Get(holePoints.HoleId);
                                repositories.HolePoints.Create(holePoints);
                                toResponce.model = new object[] { holePoints };
                            }
                            catch (Exception ex)
                            {
                                toResponce.error = ex.InnerException.Message;
                            }

                        }
                        else
                        {
                            toResponce.error = "Ошибка в данных для записи";
                        }
                    }
                    break;
                case "PUT":
                    {
                        if (parameters.ContainsKey("Id"))
                        {
                            HolePoints holePoints = new HolePoints();

                            if (tryParce(parameters, ref holePoints))
                            {
                                try
                                {
                                    repositories.HolePoints.Update(holePoints);
                                    toResponce.model = new object[] { holePoints };
                                }
                                catch (Exception ex)
                                {
                                    toResponce.error = ex.InnerException.Message;
                                }
                            }
                            else
                            {
                                toResponce.error = "Ошибка в данных для записи";
                            }
                        }
                        else
                        {
                            toResponce.error = "Ошибка в данных обновления";
                        }
                    }
                    break;
                case "DELETE":
                    if (parameters.ContainsKey("Id"))
                    {
                        long id;

                        if (long.TryParse(parameters["Id"], out id))
                        {
                            try
                            {
                                repositories.HolePoints.Delete(id);
                            }
                            catch (Exception ex)
                            {
                                toResponce.error = ex.InnerException.Message;
                            }
                        }
                        else
                        {
                            toResponce.error = "Не валидный идентификатор";
                        }
                    }
                    else
                    {
                        toResponce.error = "Ошибка в данных для удаления";
                    }

                    break;
                default:
                    toResponce.error = $"Такой запрос не поддерживается";

                    break;
            }

            return toResponce;
        }
    }
}

using RestApiConsole.DataBase.Models;
using RestApiConsole.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Controllers
{
    public class DrillBlockPointsController : BaseController
    {
        public DrillBlockPointsController(Repositories repositories) : base(repositories)
        {
        }

        public override string getSupportedQueries(string restUri)
        {
            return $"GET {restUri}DrillBlock\n" +
                $"GET {restUri}DrillBlockPoints/id/\n" +
                $"POST {restUri}DrillBlockPoints\n" +
                $"PUT {restUri}DrillBlockPoints\n" +
                $"DELETE {restUri}DrillBlockPoints/id/";
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
                                var row = repositories.DrillBlockPoints.Get(id);
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
                        toResponce.model = repositories.DrillBlockPoints.GetAll().ToArray();
                    }
                    break;
                case "POST":
                    {
                        DrillBlockPoints drillBlockPoint = new DrillBlockPoints();

                        parameters["Id"] = "0";

                        if (tryParce(parameters, ref drillBlockPoint))
                        {
                            try
                            {
                                var res = repositories.DrillBlock.Get(drillBlockPoint.DrillBlockId);

                                repositories.DrillBlockPoints.Create(drillBlockPoint);
                                toResponce.model = new object[] { drillBlockPoint };
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
                            DrillBlockPoints drillBlockPoint = new DrillBlockPoints();

                            if (tryParce(parameters, ref drillBlockPoint))
                            {
                                try
                                {
                                    repositories.DrillBlockPoints.Update(drillBlockPoint);
                                    toResponce.model = new object[] { drillBlockPoint };
                                }
                                catch (Exception e)
                                {
                                    toResponce.error = e.Message;
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
                                repositories.DrillBlockPoints.Delete(id);
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

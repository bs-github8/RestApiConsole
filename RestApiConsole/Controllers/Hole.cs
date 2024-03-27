using RestApiConsole.DataBase.Models;
using RestApiConsole.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Controllers
{
    public class HoleController : BaseController
    {
        public HoleController(Repositories repositories) : base(repositories)
        {
        }

        public override string getSupportedQueries(string restUri)
        {
            return $"GET {restUri}Hole\n" +
                $"GET {restUri}Hole/id/\n" +
                $"POST {restUri}Hole\n" +
                $"PUT {restUri}Hole\n" +
                $"DELETE {restUri}Hole/id/";
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
                                var row = repositories.Hole.Get(id);
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
                        toResponce.model = repositories.Hole.GetAll().ToArray();
                    }
                    break;
                case "POST":
                    {
                        Hole hole = new Hole();

                        parameters["Id"] = "0";

                        if (tryParce(parameters, ref hole))
                        {
                            try
                            {
                                var drillBlock = repositories.DrillBlock.Get(hole.DrillBlockId);
                                repositories.Hole.Create(hole);
                                toResponce.model = new object[] { hole };
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
                            Hole hole = new Hole();

                            if (tryParce(parameters, ref hole))
                            {
                                try
                                {
                                    repositories.Hole.Update(hole);
                                    toResponce.model = new object[] { hole };
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
                                repositories.Hole.Delete(id);
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

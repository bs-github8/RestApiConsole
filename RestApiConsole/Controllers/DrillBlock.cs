using RestApiConsole.DataBase.Models;
using RestApiConsole.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Controllers
{
    public class DrillBlockController : BaseController
    {
        public DrillBlockController(Repositories repositories) : base(repositories)
        {
        }

        public override string getSupportedQueries(string restUri)
        {
            return $"GET {restUri}DrillBlock\n" +
                $"GET {restUri}DrillBlock/id/\n" +
                $"POST {restUri}DrillBlock\n" +
                $"PUT {restUri}DrillBlock\n" +
                $"DELETE {restUri}DrillBlock/id/";
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
                                var row = repositories.DrillBlock.Get(id);
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
                        toResponce.model = repositories.DrillBlock.GetAll().ToArray();
                    }
                    break;
                case "POST":
                    {
                        DrillBlock drillBlock = new DrillBlock();

                        parameters["Id"] = "0";

                        if (tryParce(parameters, ref drillBlock))
                        {
                            try
                            {
                                try
                                {
                                    repositories.DrillBlock.Create(drillBlock);
                                    toResponce.model = new object[] { drillBlock };
                                }
                                catch (Exception e)
                                {
                                    toResponce.error = e.InnerException.Message;
                                }
                            }
                            catch (Exception e)
                            {
                                toResponce.error = e.InnerException.Message;
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
                            DrillBlock drillBlock = new DrillBlock();

                            if (tryParce(parameters, ref drillBlock))
                            {
                                try
                                {
                                    if (repositories.DrillBlock.Update(drillBlock))
                                    {
                                        toResponce.model = new object[] { drillBlock };
                                    }
                                    else
                                    {
                                        toResponce.error = "Записи с таким идентификатором не существует";
                                    }
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
                                repositories.DrillBlock.Delete(id);
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

using RestApiConsole.DataBase.Models;
using RestApiConsole.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.Controllers
{
    public class ToResponce
    {
        public string actions { get; set; } = "";
        public object[] model { get; set; } = new object[0];
        public string error { get; set; } = "";
    }

    public abstract class BaseController
    {
        protected Repositories repositories;

        public BaseController(Repositories repositories)
        {
            this.repositories = repositories;
        }

        public abstract ToResponce onQuery(string verb, Dictionary<string, string> parameters);

        public abstract string getSupportedQueries(string restUri);        

        protected static bool tryParce<T>(Dictionary<string, string> data, ref T result) where T : BaseModel
        {
            var _data = data.ToDictionary(entry => entry.Key,
                                               entry => entry.Value);
            var qry = new List<System.Reflection.PropertyInfo>();

            List<String> acceptebleType = new List<string>() { "Int64", "String", "DateTime", "Single", "Int32" };

            foreach (var i in typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite).ToList())
            {
                if (acceptebleType.Contains(i.PropertyType.Name))
                {
                    qry.Add(i);
                }
            }

            if (qry.Count != data.Count)
                return false;

            foreach (var i in qry)
            {
                bool finded = false;
                string key = null;

                foreach (var d in _data.Keys)
                {
                    if (i.Name.Equals(d))
                    {
                        finded = true;
                        key = d;

                        switch (i.PropertyType.Name)
                        {
                            case "Int64":
                                long vall;
                                if (!long.TryParse(_data[d], out vall))
                                {
                                    return false;
                                }

                                i.SetValue(result, vall);
                                break;
                            case "String":
                                i.SetValue(result, _data[d]);
                                break;
                            case "DateTime":
                                DateTime vald;

                                if (!DateTime.TryParse(_data[d], out vald))
                                {
                                    return false;
                                }
                                i.SetValue(result, vald);
                                break;
                            case "Single":
                                float valf;
                                if (!float.TryParse(_data[d], out valf))
                                {
                                    return false;
                                }

                                i.SetValue(result, valf);
                                break;
                            case "Int32":
                                int vali;
                                if (!int.TryParse(_data[d], out vali))
                                {
                                    return false;
                                }

                                i.SetValue(result, vali);
                                break;
                            default:
                                throw new ArgumentException("Не все типы данных разобраны");
                        }
                    }
                }

                if (!finded)
                {
                    return false;
                }
                else
                {
                    _data.Remove(key);
                }
            }

            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.DataBase.Models
{
    public class DrillBlock : BaseModel
    {
        public string Name { get; set; }
        public DateTime UpdateDate { get; set; }

        public override void setData(BaseModel model)
        {
            if (model is DrillBlock drillBlock)
            {
                this.Name = drillBlock.Name;
                this.UpdateDate = drillBlock.UpdateDate;
            }
        }
    }
}

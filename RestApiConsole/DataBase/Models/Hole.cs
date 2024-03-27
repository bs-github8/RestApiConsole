using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.DataBase.Models
{
    public class Hole : BaseModel
    {
        public string Name { get; set; }
        public long DrillBlockId { get; set; }
        public float Depth { get; set; }
        [ForeignKey("DrillBlockId")]
        public virtual DrillBlock drillBlock { get; set; }

        public override void setData(BaseModel model)
        {
            if (model is Hole hole)
            {
                this.DrillBlockId = hole.DrillBlockId;
                this.Depth = hole.Depth;
                this.Name = hole.Name;
            }
        }
    }
}

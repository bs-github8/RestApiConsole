using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.DataBase.Models
{
    public class DrillBlockPoints : BaseModel
    {
        public long DrillBlockId { get; set; }
        public int Sequence { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
        [ForeignKey("DrillBlockId")]
        public virtual DrillBlock drillBlock { get; set; }

        public override void setData(BaseModel model)
        {
            if (model is DrillBlockPoints drillBlockPoints)
            {
                this.DrillBlockId = drillBlockPoints.DrillBlockId;
                this.Sequence = drillBlockPoints.Sequence;
                this.X = drillBlockPoints.X;
                this.Y = drillBlockPoints.Y;
                this.Z = drillBlockPoints.Z;
            }
        }
    }
}

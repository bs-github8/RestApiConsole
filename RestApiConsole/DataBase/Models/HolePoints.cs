using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestApiConsole.DataBase.Models
{
    public class HolePoints : BaseModel
    {
        public long HoleId { get; set; }
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }

        [ForeignKey("HoleId")]
        public virtual Hole hole { get; set; }

        public override void setData(BaseModel model)
        {
            if (model is HolePoints holePoints)
            {
                this.HoleId = holePoints.HoleId;
                this.X = holePoints.X;
                this.Y = holePoints.Y;
                this.Z = holePoints.Z;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardCutter
{
    class Board
    {
        public double Price { get; set; }
        public int Length { get; set; }

        public double PricePerUnit 
        {
            get { return Price / Length; }
        }
    }
}

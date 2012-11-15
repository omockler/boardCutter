using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardCutter
{
    public static class Extensions
    {
        public static List<double> CutFromScrap(this List<double> scraps, double longestCut)
        {
            var currentScrap = scraps.Where(x => x > longestCut).OrderBy(x => x).First();
            scraps.Remove(currentScrap);
            //TODO: Remove saw width
            scraps.Add(currentScrap - longestCut);

            return scraps;
        }
    }
}

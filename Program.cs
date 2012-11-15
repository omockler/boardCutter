using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoardCutter
{
    class Program
    {
        static void Main(string[] args)
        {
            var cuts = new List<Cut> {
                new Cut { Number = 2,  Length = 82  },
                new Cut { Number = 2,  Length = 36  },
                new Cut { Number = 12, Length = 36  },
                new Cut { Number = 4,  Length = 132 },
                new Cut { Number = 4,  Length = 94  },
                new Cut { Number = 14, Length = 24  },
                new Cut { Number = 12, Length = 16  },
                new Cut { Number = 12, Length = 36  }
            };

            //Current price list for 2x4's at local menards on 11/14/12
            var stockList = new List<Board> {
                new Board { Length = 20*12, Price = 7.43 },
                new Board { Length = 18*12, Price = 6.65 },
                new Board { Length = 16*12, Price = 5.44 },
                new Board { Length = 14*12, Price = 4.89 },
                new Board { Length = 12*12, Price = 4.11 },
                new Board { Length = 10*12, Price = 2.99 },
                new Board { Length = 116.625, Price = 3.49 },
                new Board { Length = 104.625, Price = 2.93 },
                new Board { Length = 92.625, Price = 2.39 },
                new Board { Length = 7*12, Price = 2.05 },
            };

            //Calculate based on always picking the smallest stock stock big enough for the cut when new stock is needed
            SmallestStock(cuts, stockList);

            //Calculate based on always picking the largest stock when a new stock is needed
            LargestStock(cuts, stockList);
            
            Console.Read();
        }

        private static void SmallestStock(List<Cut> cuts, List<Board> stockList)
        {
            throw new NotImplementedException();
        }

        private static void LargestStock(IList<Cut> cuts, IList<Board> stockList)
        {
            var longest = stockList.OrderByDescending(x => x.Length).First();
            var longestBoard = longest.Length;
            var boardCost = longest.Price;
            var allCuts = ExpandCuts(cuts).ToList();

            var scraps = new List<double>();
            int boardsUsed = 0;

            while (allCuts.Any())
            {
                var longestCut = allCuts.OrderByDescending(x => x).First();
                allCuts.Remove(longestCut);
                if (scraps.Any(x => x > longestCut))
                {
                    var currentScrap = scraps.Where(x => x > longestCut).OrderBy(x => x).First();
                    scraps.Remove(currentScrap);
                    //TODO: Remove saw width
                    scraps.Add(currentScrap - longestCut);
                }
                else
                {
                    boardsUsed++;
                    //TODO: Remove saw width
                    scraps.Add(longestBoard - longestCut);
                }
            }

            Console.WriteLine("Total number of boards used: {0}\n", boardsUsed);
            Console.WriteLine("Total Cost: {0}", boardsUsed * boardCost);
            if (scraps.Any())
            {
                double waste = 0;
                Console.Write("Remainders: ");
                foreach (var s in scraps)
                {
                    waste += s;
                    Console.Write("{0}, ", s);
                }
                Console.Write("\n");
                Console.WriteLine("Utilization Percent: {0}", (1 - (waste / (boardsUsed * longestBoard))) * 100);
            }
        }

        private static IEnumerable<double> ExpandCuts(IList<Cut> cuts)
        {
            foreach (var cut in cuts)
            {
                for (int i = 0; i < cut.Number; i++)
                {
                    yield return cut.Length;
                }
            }
        }
    }
}

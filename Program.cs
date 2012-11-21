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
        public const double kerf = 0.125;
        static void Main(string[] args)
        {
            var cuts = new List<Cut> {
                new Cut { Number = 13, Length = 27.5 },
                new Cut { Number = 6,  Length = 129  },
                new Cut { Number = 3,  Length = 105  },
                new Cut { Number = 8,  Length = 15   },
                new Cut { Number = 2,  Length = 6*12 },
                new Cut { Number = 2,  Length = 42   },
                new Cut { Number = 7,  Length = 42   },
                new Cut { Number = 3,  Length = 38.5 },
                new Cut { Number = 3,  Length =32.3  }
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
            SmallestStock(ExpandCuts(cuts).OrderByDescending(x => x).ToList(), stockList);

            Console.WriteLine("");

            //Calculate based on always picking the largest stock when a new stock is needed
            LargestStock(ExpandCuts(cuts).OrderByDescending(x => x).ToList(), stockList);
            
            Console.Read();
        }

        private static void SmallestStock(IList<double> cuts, List<Board> stockList)
        {
            var stockUsed = new List<Board>();
            var scraps = new List<double>();
            while (cuts.Any())
            {
                //switch to from list to queue
                var longestCut = cuts.OrderByDescending(x => x).First();
                cuts.Remove(longestCut);
                if (scraps.Any(x => x > longestCut))
                {
                    scraps = scraps.CutFromScrap(longestCut);
                }
                else
                {
                    var newStock = stockList.Where(s => s.Length > longestCut).OrderBy(s => s.PricePerUnit).First();
                    //Remove Saw Width
                    stockUsed.Add(newStock);
                    scraps.Add(newStock.Length - longestCut);
                }
            }

            Console.WriteLine("Number of boards used: {0}", stockUsed.Count());            
            Console.WriteLine("Total Cost: {0}", stockUsed.Select(s => s.Price).Sum());
            OutputPurchaseList(stockUsed);
            OutputWaste(scraps, stockUsed.Select(s => s.Length).Sum());
        }        

        private static void LargestStock(IList<double> cuts, IList<Board> stockList)
        {
            var longest = stockList.OrderByDescending(x => x.Length).First();
            var longestBoard = longest.Length;
            var boardCost = longest.Price;

            var scraps = new List<double>();
            var stockUsed = new List<Board>();

            while (cuts.Any())
            {
                var longestCut = cuts.OrderByDescending(x => x).First();
                cuts.Remove(longestCut);
                if (scraps.Any(x => x > longestCut))
                {
                    scraps = scraps.CutFromScrap(longestCut);
                }
                else
                {
                    stockUsed.Add(longest);
                    scraps.Add(longestBoard - longestCut - kerf);
                }
            }

            Console.WriteLine("Total number of boards used: {0}", stockUsed.Count());
            Console.WriteLine("Total Cost: {0}", stockUsed.Count() * boardCost);
            OutputWaste(scraps, stockUsed.Count() * longestBoard);            
        }

        private static void OutputPurchaseList(List<Board> stockUsed)
        {
            //Output Purchase List
            var purchaseList = stockUsed.GroupBy(s => s.Length).Select(s => String.Format("{0} | {1}", s.Key / 12, s.Count()));
            Console.WriteLine("Purchase List: ", purchaseList);
            purchaseList.ToList().ForEach(i => Console.WriteLine("\t {0}", i));
        }

        private static void OutputWaste(List<double> scraps, double totalStock)
        {
            double waste = 0;            
            
            Console.Write("Remainders: ");
            foreach (var s in scraps)
            {
                waste += s;
                Console.Write("{0}, ", s);
            }
            Console.Write("\n");
            Console.WriteLine("Wasted Length: {0}", waste/12);
            Console.WriteLine("Utilization Percent: {0}", (1 - (waste / totalStock)) * 100);
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

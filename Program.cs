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
            var longestBoard = 20*12;
            var boardCost = 7.43;
            var allCuts = ExpandCuts(cuts).ToList();

            //var connectionString = "mongodb://localhost/?safe=true";
            //var server = MongoServer.Create(connectionString);

            //var db = server.GetDatabase("BoardCuter");
            //var boardsCollection = db.GetCollection<Board>("boards");

            //Throw if largest board isn't long enough for longest cut

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
            Console.Read();
        }

        private static IEnumerable<double> ExpandCuts(List<Cut> cuts)
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

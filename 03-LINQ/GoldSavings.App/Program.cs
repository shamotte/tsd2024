using GoldSavings.App.Client;
using GoldSavings.App.Model;
using GoldSavings.App.Services;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Serialization;
namespace GoldSavings.App;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, Gold Investor!");

        // Step 1: Get gold prices
        GoldDataService dataService = new GoldDataService();
        DateTime startDate = new DateTime(2025,1,1);
        DateTime endDate = new DateTime(2025,12,31);

        DateTime jan_start = new DateTime(2020, 1, 1);
        DateTime jan_end = new DateTime(2020, 1, 31);
        List<GoldPrice> goldPrices = dataService.GetGoldPrices(startDate, endDate).GetAwaiter().GetResult();

        if (goldPrices.Count == 0)
        {
            Console.WriteLine("No data found. Exiting.");
            return;
        }

        Console.WriteLine($"Retrieved {goldPrices.Count} records. Ready for analysis.");

        // Step 2: Perform analysis
        GoldAnalysisService analysisService = new GoldAnalysisService(goldPrices);
        var avgPrice = analysisService.GetAveragePrice();

        //2
        //a
        GoldResultPrinter.PrintPrices(goldPrices.OrderBy(x => x.Price).Take(3).ToList(), "lowest gold prices");
        GoldResultPrinter.PrintPrices(goldPrices.OrderBy(x => -x.Price).Take(3).ToList(), "highest gold prices");

        //b
        var jan_prices = dataService.GetGoldPrices(jan_start, jan_end).GetAwaiter().GetResult();
        
        var cartesian_product = from x in jan_prices from y in jan_prices select new { x, y };

        var rois = cartesian_product.Where( x => x.x.Date <= x.y.Date).Select(x => new { ret = (x.y.Price -x.x.Price) / x.x.Price * 100, date_buy = x.x.Date, date_sell = x.y.Date }).Where(x=> x.ret >= 5).OrderBy(x => -x.ret);

        if (rois.Count() == 0)
        {
            Console.WriteLine("it is not possible to ern more than 5 % in january");
        }
        else
        {
            Console.WriteLine("on each of those day combinations you could have erned more than 5%");
            foreach(var ro in rois) 
                Console.WriteLine($"return of invest ={ro.ret} when buying on{ro.date_buy} and seling at {ro.date_sell}");
        }


        var zad3_a = new DateTime(2019, 1, 1);
        var zad3_a_end = new DateTime(2019, 12, 31);

        var zad3_b = new DateTime(2020, 1, 1);
        var zad3_b_end = new DateTime(2020, 12, 31);


        var data = Enumerable.Empty<GoldPrice>();
        
        for (int i = 2019;i<=2022;i++)
        {
            var date_start_3 = new DateTime(i, 1, 1);
            var date_end_3 = new DateTime(i, 12, 31);
            data = data.Concat(dataService.GetGoldPrices(date_start_3, date_end_3).GetAwaiter().GetResult());

            
        }

        
        

       //c

        GoldResultPrinter.PrintPrices(data.OrderBy(x=> -x.Price).Skip(10).Take(3).ToList(), "3 in the second then or sht");


        //d
        int[] years = [2020, 2023, 2024];
        foreach (int x in years)
        {
            var st = new DateTime(x, 1, 1);
            var end = new DateTime(x, 12, 31);

            var dat = dataService.GetGoldPrices(st, end).GetAwaiter().GetResult();

            var res = (from aaaa in dat select aaaa.Price).Average();
            GoldResultPrinter.PrintSingleValue(res, $"avarge gold price in {x}");
        }

        var full_data = Enumerable.Empty<GoldPrice>();
        for (int i = 2020; i <= 2024; i++)
        {
            var date_start_3 = new DateTime(i, 1, 1);
            var date_end_3 = new DateTime(i, 12, 31);
            full_data = full_data.Concat(dataService.GetGoldPrices(date_start_3, date_end_3).GetAwaiter().GetResult());


        }

        //e
        var cartesian_product_v2 = from x in full_data from y in full_data select new { x, y };

        var best_roi = cartesian_product_v2.Where(x => x.x.Date <= x.y.Date).Select(x => new { ret = (x.y.Price - x.x.Price) / x.x.Price * 100, date_buy = x.x.Date, date_sell = x.y.Date }).OrderBy(x => -x.ret).Take(1).ToList()[0];

        Console.WriteLine($"return of invest ={best_roi.ret} when buying on{best_roi.date_buy} and seling at {best_roi.date_sell}");

        // Step 3: Print results
        GoldResultPrinter.PrintSingleValue(Math.Round(avgPrice, 2), "Average Gold Price Last Half Year");

        Console.WriteLine("\nGold Analyis Queries with LINQ Completed.");


        //zad 3
        double[] sample_data = [3.1, 2.3, 1.1,5.0];
        XMLUtils.SaveToXml(sample_data.ToList(), "data.xml");
        var new_data = XMLUtils.LoadFromXml("data.xml");

        bool good_result = true;
        for (int i = 0; i < new_data.Count; i++)
        {
            good_result &= sample_data[i]==new_data[i];
        }
        Console.WriteLine($"it is {good_result} that saving to xml works");



        Func<int,bool> is_leep_year = year => year % 4 == 0 && (( year % 100 ==0 && year % 400 ==0) || year % 100 != 0);

        int[] tests = [2024, 2026, 1700, 1600, 2000];

        foreach (int test in tests)
        {
            if (is_leep_year(test))
                Console.WriteLine($"{test} is a leap year");
            else
                Console.WriteLine($"{test} is not a leap year");
        }

    }

   
public static class XMLUtils
{
    public static void SaveToXml(List<double> data, string path)
    {
        var serializer = new XmlSerializer(typeof(List<double>));
        using var writer = new StreamWriter(path);
        serializer.Serialize(writer, data);
    }

        public static List<double> LoadFromXml(string path)
        {
            return (List<double>)new System.Xml.Serialization.XmlSerializer(typeof(List<double>))
                .Deserialize(new System.IO.StreamReader(path));
        }

    }

}

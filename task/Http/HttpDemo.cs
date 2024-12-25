using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;

namespace task.Http
{
    internal class HttpDemo
    {
        private String _url = "http://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?json";
        public void Run()
        {
            Console.WriteLine("Http Demo");
            Task.Run(ShowRate).Wait();
        }

        private async Task ShowRate()
        {
            string[] curr = new string[33];
            int i = 0;
            Console.WriteLine("Enter date: ");
            string date = Console.ReadLine();
            _url = $"http://bank.gov.ua/NBUStatService/v1/statdirectory/exchange?date={date}&json";

            while (true) 
            {
                Console.WriteLine("Enter currency: ");
                string currency = Console.ReadLine();
                if (currency != "")
                {
                    curr[i] = currency;
                    i++;
                }
                else
                {
                    break;
                }
            }

            HttpClient client = new();
            HttpResponseMessage res = await client.GetAsync(_url);

            //Console.WriteLine("HTTP/{0} {1} {2}", res.Version, (int)res.StatusCode, res.ReasonPhrase);

            //if (!hasCurrencies)
            //{
            //    foreach (var header in res.Headers)
            //    {
            //        Console.WriteLine("{0}: {1}",
            //            header.Key,
            //            HttpUtility.UrlDecode(header.Value.First(), Encoding.Latin1)
            //            );
            //    }
            //}
            //else
            //{
            //    i = 0;
            //    foreach (var header in res.Headers)
            //    {
            //        if (curr.Contains(header.Key))
            //        {
            //            Console.WriteLine("{0}: {1}",
            //            header.Key,
            //            HttpUtility.UrlDecode(header.Value.First(), Encoding.Latin1)
            //            );
            //        }
            //    }
            //}
            //Console.WriteLine();

            String body = await res.Content.ReadAsStringAsync();
            Console.WriteLine(body);
            client.Dispose();

            bool hasCurrencies = curr.Any(c => !string.IsNullOrEmpty(c));

            var rates = JsonSerializer.Deserialize <List<NbuRate>>(body);

            if (rates == null || !rates.Any())
            {
                Console.WriteLine("No rates available.");
                return;
            }

            if (!hasCurrencies)
            {
                foreach (var rate in rates)
                {
                    Console.WriteLine($"{rate.cc}: {rate.rate}, {rate.r030}, {rate.txt}, {rate.exchangedate}");
                }
            }
            else
            {
                foreach (var rate in rates)
                {
                    if (curr.Contains(rate.cc))
                    {
                        Console.WriteLine($"{rate.cc}: {rate.rate}, {rate.r030}, {rate.txt}, {rate.exchangedate}");
                    }
                }
            }

            while (true)
            {
                Console.WriteLine("Sort options:");
                Console.WriteLine("1 - By name (ascending)");
                Console.WriteLine("2 - By name (descending)");
                Console.WriteLine("3 - By rate (ascending)");
                Console.WriteLine("4 - By rate (descending)");
                Console.WriteLine("0 - Exit");
                Console.Write("Enter your choice: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        rates = rates.OrderBy(rate => rate.txt).ToList();
                        break;
                    case "2":
                        rates = rates.OrderByDescending(rate => rate.txt).ToList();
                        break;
                    case "3":
                        rates = rates.OrderBy(rate => rate.rate).ToList();
                        break;
                    case "4":
                        rates = rates.OrderByDescending(rate => rate.rate).ToList();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Invalid choice. Please try again.");
                        continue;
                }
                if (!hasCurrencies)
                {
                    foreach (var rate in rates)
                    {
                        Console.WriteLine($"{rate.cc}: {rate.rate}, {rate.r030}, {rate.txt}, {rate.exchangedate}");
                    }
                }
                else
                {
                    foreach (var rate in rates)
                    {
                        if (curr.Contains(rate.cc))
                        {
                            Console.WriteLine($"{rate.cc}: {rate.rate}, {rate.r030}, {rate.txt}, {rate.exchangedate}");
                        }
                    }
                }
            }
        }
    }

    internal class NbuRate
    {
        public int r030 { get; set; }
        public String txt {  get; set; }
        public double rate { get; set; }
        public String cc { get; set; }
        public String exchangedate { get; set; }

        //public override string ToString()
        //{
        //    return $"{cc} ({txt}) {rate}";
        //}
    }
}

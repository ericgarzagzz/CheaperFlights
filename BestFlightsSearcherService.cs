using CheaperFlights.Models;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.DevTools.V111.Storage;

namespace CheaperFlights
{
    internal static class BestFlightsSearcherService
    {
        public static Task<List<Cluster>> Search(StatusContext ctx, LocaleSuggestion departure, LocaleSuggestion arrival)
        {
            var chromeService = ChromeDriverService.CreateDefaultService();
            chromeService.DriverServicePath = Directory.GetCurrentDirectory();
            chromeService.EnableVerboseLogging = false;
            chromeService.SuppressInitialDiagnosticInformation = true;
            chromeService.HideCommandPromptWindow = true;

            var chromeOptions = new ChromeOptions();
            chromeOptions.PageLoadStrategy = PageLoadStrategy.Normal;
            chromeOptions.AddArgument("headless");

            IWebDriver driver = new ChromeDriver(chromeService, chromeOptions);

            var baseUrl = "https://www.us.despegar.com/";
            var searchUrl = baseUrl + $"flights/{departure.Target.Iata ?? departure.Target.Code}/{arrival.Target.Iata ?? arrival.Target.Code}?from=SB&di=1-0";

            driver.Navigate().GoToUrl(searchUrl);

            var clusters = new List<Cluster>();

            var clustersContainerFilter = By.Id("clusters");

            if (!driver.FindElements(clustersContainerFilter).Any())
            {
                AnsiConsole.MarkupLine("[red]There are no flights available.[/]");
                return Task.FromResult(clusters);
            }

            var clustersContainer = driver.FindElement(clustersContainerFilter);

            var clusterContainerList = clustersContainer.FindElements(By.ClassName("cluster-container"));

            foreach (var clusterContainer in clusterContainerList)
            {
                var cluster = new Cluster();

                var amountElement = clusterContainer.FindElement(By.ClassName("price"));
                string amount = amountElement.Text;

                cluster.Amount = amount;

                var currencyElement = clusterContainer.FindElement(By.ClassName("pricebox-currency"));
                string currency = currencyElement.Text;

                cluster.Currency = currency;

                try
                {
                    var nameElement = clusterContainer.FindElement(By.ClassName("name")).FindElement(By.TagName("span"));
                    string name = nameElement.Text;

                    cluster.Name = name;
                }
                catch (Exception)
                {
                    AnsiConsole.MarkupLine("[yellow]WARNING: A result has no airline name.[/]");
                }

                var itineraryClusterCollection = clusterContainer.FindElements(By.TagName("itinerary-reduced-cluster"));

                var departureItineraryContainer = itineraryClusterCollection.ElementAtOrDefault(0);

                if (departureItineraryContainer != null)
                {
                    var dateElement = departureItineraryContainer.FindElement(By.ClassName("date"));
                    string date = dateElement.Text;

                    var reduceInfoItemElement = departureItineraryContainer.FindElement(By.TagName("reduce-info-item"));
                    var reduceInfoTextElements = reduceInfoItemElement.FindElements(By.ClassName("stops-text"));
                    string reduceInfo = string.Join("", reduceInfoTextElements.Select(e => e.Text));

                    cluster.DepartureInfo = $"{date} {reduceInfo}";
                }

                var returnItineraryContainer = itineraryClusterCollection.ElementAtOrDefault(1);

                if (returnItineraryContainer != null)
                {
                    var dateElement = returnItineraryContainer.FindElement(By.ClassName("date"));
                    string date = dateElement.Text;

                    var reduceInfoItemElement = returnItineraryContainer.FindElement(By.TagName("reduce-info-item"));
                    var reduceInfoTextElements = reduceInfoItemElement.FindElements(By.ClassName("stops-text"));
                    string reduceInfo = string.Join("", reduceInfoTextElements.Select(e => e.Text));

                    cluster.ReturnInfo = $"{date} {reduceInfo}";
                }

                var quantityDaysElement = clusterContainer.FindElement(By.ClassName("quantity-days")).FindElement(By.ClassName("elipsis-days"));
                var quantityDays = quantityDaysElement.Text;

                cluster.Length = quantityDays;

                clusters.Add(cluster);
            }

            driver.Quit();

            return Task.FromResult(clusters);
        }
    }
}

using CheaperFlights;
using CheaperFlights.Models;
using Spectre.Console;

var departureHint = AnsiConsole.Ask<string>("Where from?");

var suggestionsResponse = await LocaleSearchRepository.Search(hint: departureHint);

if (suggestionsResponse == null || !suggestionsResponse.Items.Any())
{
    AnsiConsole.MarkupLine("[maroon] Could not get any results.[/]");
    return;
}

var departure = AnsiConsole.Prompt(new SelectionPrompt<LocaleSuggestion>()
    .Title("Select one of the following results")
    .PageSize(5)
    .MoreChoicesText("[grey](Move up and down to reveal more results)[/]")
    .AddChoices(suggestionsResponse.Items.SelectMany(i => i.Items))
);

var arrivalHint = AnsiConsole.Ask<string>("Where to?");

suggestionsResponse = await LocaleSearchRepository.Search(hint: arrivalHint);

if (suggestionsResponse == null || !suggestionsResponse.Items.Any())
{
    AnsiConsole.MarkupLine("[maroon] Could not get any results.[/]");
    return;
}

var arrival = AnsiConsole.Prompt(new SelectionPrompt<LocaleSuggestion>()
    .Title("Select one of the following results")
    .PageSize(5)
    .MoreChoicesText("[grey](Move up and down to reveal more results)[/]")
    .AddChoices(suggestionsResponse.Items.SelectMany(i => i.Items))
);

var clusters = await AnsiConsole.Status()
    .StartAsync("Searching for the best prices...", async ctx =>
    {
        return await BestFlightsSearcherService.Search(ctx, departure, arrival);
    });

var table = new Table();
table.Expand();
table.Title = new TableTitle($"Cheapest flights from [blue1]{departure.Display}[/] to [green1]{arrival.Display}[/]");

table.AddColumns(new[] { "Name", "Departure", "Return", "Días", "Price" });

string emptyDataStr = "No information";

foreach (var cluster in clusters)
{
    table.AddRow(cluster.Name ?? emptyDataStr, cluster.DepartureInfo ?? emptyDataStr, cluster.ReturnInfo ?? emptyDataStr, cluster.Length, $"[bold green1]{cluster.Amount} {cluster.Currency}[/]");
}

if (!clusters.Any())
    table.AddEmptyRow();

AnsiConsole.Clear();
AnsiConsole.Write(table);

Console.ReadLine();
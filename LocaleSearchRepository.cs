using CheaperFlights.Models;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheaperFlights
{
    internal static class LocaleSearchRepository
    {
        public static async Task<LocaleSuggestionsResponse?> Search(string locale = "en-US", string hint = "")
        {
            if (string.IsNullOrEmpty(locale))
                throw new ArgumentNullException(nameof(locale));
            else if (string.IsNullOrEmpty(hint))
                throw new ArgumentNullException(nameof(hint));

            var clientOptions = new RestClientOptions("https://www.us.despegar.com");
            var client = new RestClient(clientOptions);

            var request = new RestRequest("suggestions", Method.Get);
            request.AddParameter("locale", locale);
            request.AddParameter("profile", "sbox-flights");
            request.AddParameter("hint", hint);

            var response = await client.GetAsync<LocaleSuggestionsResponse>(request);

            return response;
        }
    }
}

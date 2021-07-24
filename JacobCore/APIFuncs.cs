using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JacobCore
{
    class APIFuncs
    {
        public static HttpClient httpClient = new HttpClient() { Timeout = TimeSpan.FromMinutes(30) };

        /// <summary>
        /// Invokes a REST API with the given parameters.
        /// </summary>
        /// <param name="requestInfo">Tuple in the format ("VERB", "requestUrl")</param>
        /// <param name="accessToken">Access token to authorize the request. Passed in headers as "Bearer accessToken" unless overrideAuthorizationHeader is true.</param>
        /// <param name="overrideAuthorizationHeader">If true, uses the entire accessToken string as the authorization header. For example, if it requires "Authorization accessToken", you would pass that instead of just the access token.</param>
        /// <param name="stringContent">String content for a POST or PUT request.</param>
        /// <param name="byteContent">Byte array content fr a POST or PUT request.</param>
        /// <returns>Tuple indicating success or failure with item 1 and response content in item 2.</returns>
        public static async Task<(bool, JObject)> InvokeAPI((string, string) requestInfo, string accessToken, bool overrideAuthorizationHeader = false, string stringContent = null, byte[] byteContent = null)
        {
            using (var request = new HttpRequestMessage(new HttpMethod(requestInfo.Item1), requestInfo.Item2))
            {
                request.Headers.TryAddWithoutValidation("Accept", "application/json");
                request.Headers.TryAddWithoutValidation("Authorization", overrideAuthorizationHeader ? accessToken : "Bearer " + accessToken);
                if(stringContent != null)
                {
                    request.Content = new StringContent(stringContent);
                    request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
                }
                else if(byteContent != null)
                {
                    request.Content = new ByteArrayContent(byteContent);
                }
                HttpResponseMessage response;
                try
                {
                    response = await httpClient.SendAsync(request);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception thrown while executing API request " + requestInfo.Item1 + ", " + requestInfo.Item2 + "\n" + ex.Message);
                    return (false, new JObject() { "StatusCode", "418" });
                }

                bool success = response.IsSuccessStatusCode;
                string contentString = await response.Content.ReadAsStringAsync();
                JObject contentJson = JObject.Parse(string.IsNullOrWhiteSpace(contentString) ? "{}" : contentString);
                contentJson.Add(new JProperty("StatusCode", response.StatusCode));

                return (success, contentJson);
            }
        }

        /// <summary>
        /// This function interfaces with certain Azure REST APIs that have a limit on the number of items they return, but provide a nextLink to the next set of items until there are no more.
        /// </summary>
        /// <param name="initialUri">First request Uri; this is updated with the nextLink if it exists until there is no nextLink.</param>
        /// <param name="action">Action to perform on the "value" of the response. Should accept a JObject and probably add it to a list or something.</param>
        /// <param name="accessToken">Authorization token to the REST API.</param>
        /// <param name="failureMessage">Message to print to console if one of the calls fails.</param>
        /// <returns>Bool indicating whether or not all requests were successful.</returns>
        public static async Task<bool> GetIterativeContent(string initialUri, Action<JObject> action, string accessToken, string failureMessage)
        {
            bool allSucceeded = true;
            string currentUrl = initialUri;
            int itemsReturned;
            do
            {
                itemsReturned = 0;
                (string, string) currentRequest = ( "GET", currentUrl );
                var response = await APIFuncs.InvokeAPI(currentRequest, accessToken);
                if (response.Item1)
                {
                    JObject itemList = response.Item2;
                    foreach (JObject item in itemList["value"])
                    {
                        action(item);
                        itemsReturned++;
                    }
                    if (itemList.ContainsKey("nextLink") && !string.IsNullOrWhiteSpace(itemList["nextLink"].ToString()))
                    {
                        currentUrl = itemList["nextLink"].ToString();
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    Console.WriteLine(failureMessage);
                    allSucceeded = false;
                    break;
                }
            } while (itemsReturned != 0);
            return allSucceeded;
        }
    }
}

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace APITesting
{
    public abstract class SupportingMethods
    {
        public RestRequest restRequest;
        public RestResponse restResponse;
        private RestClient restClient;

        public void SetUpRestClient()
        {
            restClient = new RestClient("https://reqres.in/api");
        }

        public void AddHeaders(RestRequest restRequest)
        {
            restRequest.AddHeader("Content-Type", "application/json");
            restRequest.AddHeader("X-RequestId", "AUTOMAT" + DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        public RestResponse SendRequestAndValidateResponseCode(RestRequest restRequest, object requestBody, int expectedHttpCode)
        {
            // Add headers to the request
            AddHeaders(restRequest);

            // Print headers and URL parth
            PrintHeadersFromRequestResponse("request");
            PrintUrl();

            //Set request format to JSON and add request body
            restRequest.RequestFormat = DataFormat.Json;
            restRequest.AddJsonBody(requestBody);

            // Print request body
            PrintRequest(requestBody);

            //Execute request and save response to variable
            RestResponse restResponse = restClient.Execute(restRequest);
            TestData.sResponse = restResponse.Content;

            //Extract response code
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;

            // Print headers and response body
            PrintHeadersFromRequestResponse("response");
            PrintResponse(TestData.sResponse);

            //Assertion - expected vs actual http code//
            Assert.IsTrue(numericStatusCode == expectedHttpCode, $"Invalid response code!. Expected code: {expectedHttpCode}, " +
                $"but service returned code: {numericStatusCode}");

            return restResponse;
        }

        public RestResponse SendRequestWithoutBodyAndValidateResponseCode(RestRequest restRequest, int expectedHttpCode)
        {
            // Print headers and URL parth
            PrintHeadersFromRequestResponse("request");
            PrintUrl();

            //Execute request and save response to variable
            restResponse = restClient.Execute(restRequest);
            TestData.sResponse = restResponse.Content;

            //Extract response code
            HttpStatusCode statusCode = restResponse.StatusCode;
            int numericStatusCode = (int)statusCode;

            // Print headers and response body
            PrintHeadersFromRequestResponse("response");
            PrintResponse(TestData.sResponse);

            //Assertion - expected vs actual http code//
            Assert.IsTrue(numericStatusCode == expectedHttpCode, $"Invalid response code!. Expected code: {expectedHttpCode}, " +
                $"but service returned code: {numericStatusCode}");

            return restResponse;
        }

        public static void PrintRequest(object Request)
        {
            // Serialize object to JSON string
            string formatedRequest = JsonConvert.SerializeObject(Request, Formatting.Indented);
            Console.WriteLine("============= JSON Request =============");
            Console.WriteLine(formatedRequest);
            Console.WriteLine("\n");
        }

        public static void PrintResponse(string Response)
        {
            // Deserialize JSON
            dynamic jsonObject = JsonConvert.DeserializeObject(Response); ;
            Console.WriteLine("============= Deserialized JSON Response =============");
            Console.WriteLine(JsonConvert.SerializeObject(jsonObject, Formatting.Indented));
            Console.WriteLine("\n");
        }

        public void PrintHeadersFromRequestResponse(string headersFrom)
        {
            Console.WriteLine("============= Headers from " + headersFrom + " =============");
            switch (headersFrom)
            {
                case "request":
                    if (restRequest != null)
                    {
                        foreach (var param in restRequest.Parameters)
                        {
                            Console.WriteLine(String.Format("{0}: {1}",
                                param.Name,
                                param.Value)); }

                    }
                    else
                    {
                        Console.WriteLine("Request header is null!");
                    }
                    break;

                case "response":
                    if (restResponse != null)
                    {
                        foreach (var param in restResponse.Headers)
                        {
                            Console.WriteLine(String.Format("{0}: {1}",
                                param.Name,
                                param.Value));
                        }
                    }
                    else
                    {
                        Console.WriteLine("Response header is null!");
                    }
                    break;
            }
            Console.WriteLine("\n");

        }
        public void PrintUrl()
        {
            Console.WriteLine("URL: " + restClient.BuildUri(restRequest).AbsoluteUri);
            Console.WriteLine("\n");
        }

        public static void GetResponseDataAndValidateError(string expectedErrorMessage)
        {
            var responseObject = JObject.Parse(TestData.sResponse);
            string errorMessage = responseObject["error"].ToString();
            Assert.IsTrue(errorMessage.Contains(expectedErrorMessage), "Incorrect error message. " +
                "Expected message: {0} | Actual message: {1}", expectedErrorMessage, errorMessage);
        }

        public static void GetResponseDataAndValidateError(string expectedRegex, bool isRegex = true)
        {
            var responseObject = JObject.Parse(TestData.sResponse);
            string errorMessage = responseObject["error"].ToString();
            if (isRegex == true)
            {
                Assert.IsTrue(Regex.IsMatch(errorMessage, expectedRegex),
                 "Incorrect error message. " +
                 "Expected regex message: {0} | Actual message: {1}", expectedRegex, errorMessage);
            }
            else
	        {
                 Assert.AreEqual(expectedRegex, errorMessage,
                 "Expected message: {0} | Actual message: {1}", expectedRegex, errorMessage);

	        }
        }

        public static string GetValueByKeyPath (JObject response, string keyPath, bool failIfKeyNotExist = true)
        {
            try
            {
                string responseValue = response.SelectToken(keyPath).ToString();
                return responseValue;
            }
            catch (System.NullReferenceException)
            {
                if (failIfKeyNotExist)
                {
                    Assert.Fail("Key {0} not exist in path", keyPath);
                }
                else { 
                Console.WriteLine("Key {0} not exist in path", keyPath);
                }
                return null;
            }
        }

        public static string GetValueByKeyPath(string response, string keyPath, bool failIfKeyNotExist = true)
        {
            try
            {
                var json = JObject.Parse(response);
                string responseValue = json.SelectToken(keyPath).ToString();
                return responseValue;
            }
            catch (System.NullReferenceException)
            {
                if (failIfKeyNotExist)
                {
                    Assert.Fail("Key {0} not exist in path", keyPath);
                }
                else
                {
                    Console.WriteLine("Key {0} not exist in path", keyPath);
                }
                return null;
            }
        }


        public static void AssertIfKeyExist(JObject response, string keyParentPath, string key)
        {
            bool keyExists = IsKeyPresent(response, keyParentPath, key);
            Assert.IsTrue(keyExists, $"Key: '{key}' does't exist in JSON response");
        }

        public static bool IsKeyPresent(JObject jsonObject, string parentObjectName, string key)
        {
            // Check if the parent object contains the specified key
            if (jsonObject[parentObjectName] is JObject parentObject)
            {
                return parentObject.ContainsKey(key);
            }

            return false;
        }

        public static void AssertIfKeyExist(JObject response, string key)
        {
            GetValueByKeyPath(response, key);
        }

    }

}

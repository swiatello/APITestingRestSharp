
using NUnit.Framework;
using RestSharp;
using System;
using System.Xml.Linq;

namespace APITesting
{    /// <summary>
     /// Test examples for unsuccessful register  -> POST /api/register
    /// </summary>
    public class PostLoginUnsuccessful : SupportingMethods
    {
        [SetUp]
        public new void SetUp()
        {
            SetUpRestClient();
        }


        [Test]
        [Order(1)]
        public void VerifyErrorAndCodeUnsuccessfulRegisterTest()
        {

            restRequest = new RestRequest("/register", Method.Post);
            object RequestJsonBody = new
            {
                email = "test@gmail.com",
            };
            SendRequestAndValidateResponseCode(restRequest, RequestJsonBody, 400);
            GetResponseDataAndValidateError("Missing password");

            GetResponseDataAndValidateError(@"M.*pas.word", true);
        }

        [Test]
        [Order(2)]
        public void VerifyErrorRegexAndCodeUnsuccessfulRegisterTest()
        {

            restRequest = new RestRequest("/register", Method.Post);
            object RequestJsonBody = new
            {
                email = "testRegex@gmail.com",
            };
            SendRequestAndValidateResponseCode(restRequest, RequestJsonBody, 400);
            GetResponseDataAndValidateError(@"M.*pas.word", true);
        }

    }
}

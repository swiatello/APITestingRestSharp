
using NUnit.Framework;
using RestSharp;
using System;
using System.Xml.Linq;

namespace APITesting
{
    public class PostUsers : SupportingMethods
    {
        [SetUp]
        public new void SetUp()
        {
            SetUpRestClient();
        }


        [Test]
        [Order(1)]
        public void PostUsersVerifyResponseCodeTest()
        {

            restRequest = new RestRequest("/users", Method.Post);
            object RequestJsonBody = new
            {
                name = "Swiatello",
                job = "leader"
            };
            SendRequestAndValidateResponseCode(restRequest, RequestJsonBody, 201);
        }
    }
}

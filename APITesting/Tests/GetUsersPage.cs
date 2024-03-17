
using NUnit.Framework;
using RestSharp;
using System;

namespace APITesting
{
    /// <summary>
    /// Test examples for list users -> GET /api/users?page=2
    /// </summary>
    public class GetUsersPage : SupportingMethods
    {
        [SetUp]
        public new void SetUp()
        {
            SetUpRestClient();
        }


        [Test]
        [Order(1)]
        public void GetUsersPageVerifyResponseCodeTest()
        {

            restRequest = new RestRequest("/users?page=2", Method.Get);
            SendRequestWithoutBodyAndValidateResponseCode(restRequest, 200);
        }
    }
}

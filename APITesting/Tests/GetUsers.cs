using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RestSharp;
using System;
using FluentAssertions;
using Dynamitey.DynamicObjects;
using System.Collections.Generic;

namespace APITesting
{
    /// <summary>
    /// Test examples for single user -> GET /api/users=2
    /// </summary>
    public class GetUsers : SupportingMethods
    {
        [SetUp]
        public new void SetUp()
        {
            SetUpRestClient();
        }

        [Test]
        [Order(1)]
        public void VerifyResponseCodeTest()
        {
            restRequest = new RestRequest("/users/2", Method.Get);
            SendRequestWithoutBodyAndValidateResponseCode(restRequest, 200);
        }

        [Test]
        [Order(2)]
        public void VerifyEmailValueFromResponseTest()
        {
            TestData.jResponse = JObject.Parse(TestData.sResponse);
            TestData.sEmail = GetValueByKeyPath(TestData.jResponse, "data.email");
            string expectedEmail = "janet.weaver@reqres.in";

            TestData.sEmail.Should().Be(expectedEmail, $"Incorrect email!. " +
                $"Expected email: {expectedEmail}, but actual email is: {TestData.sEmail}");
        }

        [Test]
        [Order(3)]
        public void VerifySupportTextMandatoryTest()
        {
            AssertIfKeyExist(TestData.jResponse, "support", "text");
        }

        [Test]
        [Order(4)]
        public void VerifyAllFieldsMandatoryTest()
        {

            List<string> lstResponseKeys = new List<string> { "data.id", "data.email", "data.first_name", "data.last_name", "data.avatar", "support.url", "support.text" };
            AssertIfKeyExist(TestData.jResponse, "support", "text");

            foreach (var key in lstResponseKeys)
            {
                AssertIfKeyExist(TestData.jResponse, key);
            }
        }

    }
}

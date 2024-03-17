
using FluentAssertions;
using NUnit.Framework;
using RestSharp;
using System;
using System.Xml.Linq;

namespace APITesting
{
    public class Crud : SupportingMethods
    {
        [SetUp]
        public new void SetUp()
        {
            SetUpRestClient();
            TestData.sName = "Anne";
            TestData.sUpdatedName = "Malgorzata";
            TestData.sJob = "developer";
            TestData.sUpdatedJob = "tester";
        }


        [Test]
        [Order(1)]
        public void PostCreateUserTest()
        {
            
            restRequest = new RestRequest("/users", Method.Post);
            object RequestJsonBody = new
            {
                name = TestData.sName,
                job = TestData.sJob
            };

            SendRequestAndValidateResponseCode(restRequest, RequestJsonBody, 201);
            TestData.sId = GetValueByKeyPath(TestData.sResponse, "id");
        }

        [Test]
        [Order(2)]
        public void PatchUpdateUserTest()
        {

            restRequest = new RestRequest("/users/" + TestData.sId, Method.Patch);
            object RequestJsonBody = new
            {
                name = TestData.sUpdatedName,
                job = TestData.sUpdatedJob
            };

            SendRequestAndValidateResponseCode(restRequest, RequestJsonBody, 200);

            string actualName = GetValueByKeyPath(TestData.sResponse, "name");
            string actualJob = GetValueByKeyPath(TestData.sResponse, "job");

            TestData.sUpdatedName.Should().Be(actualName, $"Incorrect name value!. " +
                $"Expected name value: '{TestData.sUpdatedName}', but actual name value is: '{actualName}'");

            TestData.sUpdatedJob.Should().Be(actualJob, $"Incorrect job value!. " +
            $"Expected name value: '{TestData.sUpdatedJob}', but actual name value is: '{actualJob}'");

        }

        [Test]
        [Order(3)]
        // It doesn't work correctly, user is not save
        public void GetUserTest()
        {

            restRequest = new RestRequest("/users/" + TestData.sId, Method.Get);

            SendRequestWithoutBodyAndValidateResponseCode(restRequest, 200);
            
        }

        [Test]
        [Order(4)]
        public void DeleteUserTest()
        {

            restRequest = new RestRequest("/users/" + TestData.sId, Method.Delete);

            SendRequestWithoutBodyAndValidateResponseCode(restRequest, 204);
            TestData.sResponse.Should().BeEmpty();

        }

    }
}

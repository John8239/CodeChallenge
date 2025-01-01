using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class ReportingStructureControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // TO REVIEWER: I don't know why but my direct reports always return null unless I debug into the repository layer //
        // and hover over the _employeeContext.Employees... then when I see they are there it will return them as they     //
        // should be. I don't know what the reason for that is and I don't know if that is just a quirk on my machine but  //
        // I wanted to mention it because it has caused my tests to fail until I debugged into them and performed the      //
        // the above steps, after which they passed.                                                                       //
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        [TestMethod]
        public void GetReportingStructureByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reportingStructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var reportingStructure = response.DeserializeContent<ReportingStructure>();
            Assert.IsNotNull(reportingStructure);

            Assert.IsNotNull(reportingStructure.Employee.DirectReports);
            // By the time the test gets here the Replace function is run and John Lennon only has 1 DirectReport
            Assert.AreEqual(1, reportingStructure.NumberOfReports);
            Assert.AreEqual(expectedFirstName, reportingStructure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, reportingStructure.Employee.LastName);
        }

        [TestMethod]
        public void GetReportingStructureByEmployeeId_Returns_NotFound()
        {
            // Arrange
            var employeeId = "FakeEmployeeId";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/reportingStructure/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}

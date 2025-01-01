using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace CodeChallenge.Tests.Integration
{
    [TestClass]
    public class CompensationControllerTests
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

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedSalary = 1000000.00;

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var compensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(compensation);
            Assert.IsNotNull(compensation.Employee);
            Assert.IsNotNull(compensation.CompensationId);
            Assert.IsNotNull(compensation.EffectiveDate);
            Assert.AreEqual(expectedFirstName, compensation.Employee.FirstName);
            Assert.AreEqual(expectedLastName, compensation.Employee.LastName);
            Assert.AreEqual(expectedSalary, compensation.Salary);
        }

        [TestMethod]
        public void GetCompensationByEmployeeId_Returns_NotFound()
        {
            // Arrange
            var employeeId = "FakeEmployeeId";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/compensation/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "c0c2293d-16bd-4603-8e08-638a9d18b22c",
                Department = "Engineering",
                FirstName = "George",
                LastName = "Harrison",
                Position = "Developer III",
            };

            var compensation = new Compensation()
            {
                Salary = 100000.00,
                EmployeeId = employee.EmployeeId,
                Employee = employee
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newCompensation = response.DeserializeContent<Compensation>();
            Assert.IsNotNull(newCompensation.CompensationId);
            Assert.IsNotNull(newCompensation.EffectiveDate);
            Assert.AreEqual(employee.EmployeeId, newCompensation.EmployeeId);
            Assert.AreEqual(employee.LastName, newCompensation.Employee.LastName);
            Assert.AreEqual(compensation.Salary, newCompensation.Salary);
        }

        [TestMethod]
        public void CreateCompensation_Returns_BadRequest()
        {
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // TO REVIEWER: It wasn't specified in the ReadMe but I'm assuming that if the employee doesn't already exist in the //
            // database then we should throw an error, henceforth why I made this test.                                          //
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "16a596ae-edd3-4847-99fe-86fMetallica",
                Department = "Music",
                FirstName = "James",
                LastName = "Hetfield",
                Position = "Singer",
            };

            var compensation = new Compensation()
            {
                Salary = 100000.00,
                EmployeeId = employee.EmployeeId,
                Employee = employee
            };

            var requestContent = new JsonSerialization().ToJson(compensation);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/compensation",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}

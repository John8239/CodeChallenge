using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/reportingStructure")]
    public class ReportingStructureController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public ReportingStructureController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpGet("{id}", Name = "getReportingStructureByEmployeeId")]
        public IActionResult GetReportingStructureByEmployeeId(String id)
        {
            _logger.LogDebug($"Received ReportingStructure get request for EmployeeId '{id}'");

            Employee employee = _employeeService.GetById(id);

            int total = 0;

            // Using recursion in the helper function call below to get the NumberOfReports
            ReportingStructure reportingStructure = new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = employee.DirectReports != null && employee.DirectReports.Count() > 0 ?
                    CountDirectReports(employee.DirectReports, ref total) : 0
            };

            if (reportingStructure == null)
                return NotFound();

            return Ok(reportingStructure);
        }

        #region Helper Functions

        private int CountDirectReports(List<Employee> directReports, ref int total)
        {
            foreach (Employee emp in directReports)
            {
                // Also checking .Count() > 0 might be redundant but better safe than sorry
                if (emp.DirectReports != null && emp.DirectReports.Count() > 0)
                {
                    CountDirectReports(emp.DirectReports, ref total);
                }
                total++;
            }

            return total;
        }

        #endregion
    }
}

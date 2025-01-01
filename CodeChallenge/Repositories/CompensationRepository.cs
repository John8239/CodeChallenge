using CodeChallenge.Data;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public class CompensationRepository : ICompensationRepository
    {
        private readonly CompensationContext _compensationContext;
        private readonly EmployeeContext _employeeContext;
        private readonly ILogger<ICompensationRepository> _logger;

        public CompensationRepository(ILogger<ICompensationRepository> logger, CompensationContext compensationContext, EmployeeContext employeeContext)
        {
            _logger = logger;
            _compensationContext = compensationContext;
            _employeeContext = employeeContext;
        }

        public Compensation GetById(string id)
        {
            Compensation compensation = _compensationContext.Compensations.Where(c => c.EmployeeId == id).OrderByDescending(c => c.EffectiveDate).FirstOrDefault();
            Employee employee = _employeeContext.Employees.FirstOrDefault(e => e.EmployeeId == id);

            compensation.Employee = employee;

            return compensation;
        }
    }
}

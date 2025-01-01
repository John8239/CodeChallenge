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

            if (employee != null) 
            {
                compensation.Employee = employee;
            }

            return compensation;
        }

        public Compensation Add(Compensation compensation)
        {
            compensation.CompensationId = Guid.NewGuid().ToString();
            compensation.EmployeeId = compensation.Employee.EmployeeId;
            compensation.EffectiveDate = DateTime.Now;

            _compensationContext.Compensations.Add(compensation);

            return compensation;
        }

        public Task SaveAsync()
        {
            return _compensationContext.SaveChangesAsync();
        }
    }
}

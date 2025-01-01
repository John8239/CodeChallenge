using CodeChallenge.Models;
using System;

namespace CodeChallenge.Services
{
    public interface ICompensationService
    {
        Compensation GetById(String id);
        Compensation Create(Compensation compensation);
    }
}

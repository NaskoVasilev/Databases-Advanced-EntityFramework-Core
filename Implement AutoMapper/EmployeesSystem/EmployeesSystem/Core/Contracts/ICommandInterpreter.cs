﻿namespace EmployeesSystem.Core.Contracts
{
    public interface ICommandInterpreter
    {
        string Read(string[] commandParams);
    }
}

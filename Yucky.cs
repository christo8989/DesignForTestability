﻿using System;
using System.Collections.Generic;

namespace designIssueExample
{
    class Yucky
    {
        private const int EmployeeIdColumnIndex = 0;
        private const int EmployeeNameColumnIndex = 1;
        private const int EmployeeAgeColumnIndex = 2;
        private const int EmployeeIsSalariedColumnIndex = 3;

        public IEnumerable<Employee> GetEmployees(EmployeeFilter employeeFilter, FakeSqlConnection connection)
        {
            EmployeeFilter.ValidateEmployeeFilter(employeeFilter);

            string query = "select * from employee, employee_role inner join employee.Id == employee_role.EmployeeId";

            List<Employee> result = new List<Employee>();
            using (FakeSqlCommand sqlCommand = new FakeSqlCommand(query, connection))
            {
                FakeSqlDataReader reader;
                int retryCount = 5;

                while (true)
                {
                    try
                    {
                        reader = sqlCommand.ExecuteReader();
                        break;
                    }
                    catch (Exception)
                    {
                        if (retryCount-- == 0) throw;
                    }
                }

                while (reader.Read())
                {
                    int id = reader.GetInt32(EmployeeIdColumnIndex);
                    string name = reader.GetString(EmployeeNameColumnIndex);
                    int age = reader.GetInt32(EmployeeAgeColumnIndex);
                    bool isSalaried = reader.GetBoolean(EmployeeIsSalariedColumnIndex);

                    if (EmployeeFilter.DoFilter(employeeFilter, name, age, isSalaried)) continue;

                    result.Add(new Employee {Name = name, Id = id, Age = age, IsSalaried = isSalaried});
                }
            }

            return result;
        }
    }
}
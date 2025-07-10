using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveObjectBrowser.Model
{
    public class Worker
    {

        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public double Salary { get; set; }
        public int Experience { get; set; }

        public ObservableCollection<Worker> Employees { get; set; } = new ObservableCollection<Worker>();
        public Worker? Parent { get; set; }  // To facilitate recursive deletion

        public Worker(string name, string surname, string position, double salary, int experience)
        {
            Name = name;
            Surname = surname;
            Position = position;
            Salary = salary;
            Experience = experience;
        }

        public void AddEmployee(Worker employee)
        {
            employee.Parent = this;
            Employees.Add(employee);
        }

        public int CountAllSubordinates()
        {
            int count = Employees.Count;
            foreach (var emp in Employees)
            {
                count += emp.CountAllSubordinates();
            }
            return count;
        }

        public override string ToString()
        {
            return $"Worker{{position='{Position}', surname='{Surname}', name='{Name}', experience={Experience}, salary={Salary}}}";
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Worker other) return false;
            return Name == other.Name &&
                   Surname == other.Surname &&
                   Position == other.Position &&
                   Experience == other.Experience &&
                   Salary == other.Salary;
        }
        public void Remove()
        {
            if (Parent != null)
            {
                Parent.Employees.Remove(this);
            }
            // If no parent, it must be removed from the root collection
        }


    }
}

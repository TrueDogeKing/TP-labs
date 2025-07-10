using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RecursiveObjectBrowser.Model
{
    public class Worker
    {
        [XmlIgnore]
        public Guid ID { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Position { get; set; }
        public double Salary { get; set; }
        public int Experience { get; set; }

        public ObservableCollection<Worker> Employees { get; set; } = new ObservableCollection<Worker>();
        public WorkerDetails? Details { get; set; }

        [XmlIgnore]
        public Worker? Parent { get; set; }  // To facilitate recursive deletion

        public Worker() { }
        public Worker(string name, string surname, string position, double salary, int experience)
        {
            ID = Guid.NewGuid();
            Name = name;
            Surname = surname;
            Position = position;
            Salary = salary;
            Experience = experience;
            var rnd = new Random();
            Details = new WorkerDetails(
                rnd.Next(1, 11), // Rating 1–10
                Math.Round(rnd.NextDouble() * 2000 + 500, 2), // Bonus 500–2500
                (WorkMode)rnd.Next(0, 3) // enum WorkMode: Remote, OnSite, Hybrid
            );

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

        public void RemoveWithSubordinates(ObservableCollection<Worker> rootCollection)
        {
            // Recursively remove all employees first
            foreach (var employee in Employees.ToList()) // avoid collection modification during iteration
            {
                employee.RemoveWithSubordinates(rootCollection);
            }

            Employees.Clear(); // Clear references to all subordinates

            // Now, remove this worker from its parent (if it has a parent)
            Remove(rootCollection);
        }

        public void Remove(ObservableCollection<Worker> rootCollection)
        {
            if (Parent != null)
            {
                // If there's a parent, remove the worker from the parent's list of employees
                Parent.Employees.Remove(this);
            }
            else
            {
                // If there is no parent, it's a root worker. In this case, remove it from the root collection
                RemoveFromRootCollection(rootCollection);
            }
        }

        private void RemoveFromRootCollection(ObservableCollection<Worker> rootCollection)
        {
            // This method should be called from the UI or root management logic to remove the root worker
            if (rootCollection.Contains(this))
            {
                rootCollection.Remove(this); // Remove it from the root collection (e.g., m_mainView.Children)
            }
        }



    }
}

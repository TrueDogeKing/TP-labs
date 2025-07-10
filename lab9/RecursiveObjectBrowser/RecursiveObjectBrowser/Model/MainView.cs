using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using RecursiveObjectBrowser.Model;

namespace RecursiveObjectBrowser
{
    public class MainView : IComparable<MainView>, INotifyPropertyChanged
    {
        // public ObservableCollection<Worker> Children { get; set; } = new ObservableCollection<Worker>();
        public ModelCollection<Worker> Children { get; set; } = new ModelCollection<Worker>();

        // All workers (flattened tree)
        private List<Worker> AllWorkers = new List<Worker>();

        // Filterable view of workers
        private ICollectionView _employeesCollectionView;
        public ICollectionView EmployeesCollectionView
        {
            get => _employeesCollectionView;
            private set
            {
                _employeesCollectionView = value;
                OnPropertyChanged(nameof(EmployeesCollectionView));
            }
        }

        private string _employeesFilter = string.Empty;
        public string EmployeesFilter
        {
            get => _employeesFilter;
            set
            {
                if (_employeesFilter != value)
                {
                    _employeesFilter = value;
                    EmployeesCollectionView?.Refresh();
                    OnPropertyChanged();
                }
            }
        }

        private string _salaryThreshold = string.Empty;
        public string SalaryThreshold
        {
            get => _salaryThreshold;
            set
            {
                if (_salaryThreshold != value)
                {
                    _salaryThreshold = value;
                    EmployeesCollectionView?.Refresh();
                    OnPropertyChanged();
                }
            }
        }

        public MainView()
        {
            // Watch for changes in the top-level collection
            Children.CollectionChanged += (s, e) => RefreshAllWorkers();
        }

        public void RefreshAllWorkers()
        {
            AllWorkers = FlattenWorkers(Children).ToList();
            EmployeesCollectionView = CollectionViewSource.GetDefaultView(AllWorkers);
            EmployeesCollectionView.Filter = FilterEmployees;
            EmployeesCollectionView.Refresh();
        }

        private bool FilterEmployees(object obj)
        {
            if (obj is Worker employee)
            {
                bool nameMatch = string.IsNullOrWhiteSpace(EmployeesFilter)
                                 || employee.Name.Contains(EmployeesFilter, StringComparison.OrdinalIgnoreCase);

                bool salaryMatch = true;
                if (double.TryParse(SalaryThreshold, out double threshold))
                {
                    salaryMatch = employee.Salary > threshold;
                }

                return nameMatch && salaryMatch;
            }
            return false;
        }

        private IEnumerable<Worker> FlattenWorkers(IEnumerable<Worker> rootWorkers)
        {
            foreach (var worker in rootWorkers)
            {
                yield return worker;

                if (worker.Employees != null && worker.Employees.Count > 0)
                {
                    foreach (var child in FlattenWorkers(worker.Employees))
                    {
                        yield return child;
                    }
                }
            }
        }

        public int CompareTo(MainView other)
        {
            double thisTotal = Children.Sum(w => w.Salary);
            double otherTotal = other.Children.Sum(w => w.Salary);
            return thisTotal.CompareTo(otherTotal);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

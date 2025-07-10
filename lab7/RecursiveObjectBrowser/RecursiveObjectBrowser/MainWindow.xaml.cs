using System.Windows;
using System.Collections.ObjectModel;
using RecursiveObjectBrowser.Model;
using System.Windows.Controls;
using System.Text;

namespace RecursiveObjectBrowser
{
    public partial class MainWindow : Window
    {
        private MainView m_mainView;

        public MainWindow()
        {
            InitializeComponent();

            // Initialize the main view model
            m_mainView = new MainView();
            DataContext = m_mainView;

            // Create sample data
            m_mainView = new MainView();
        }


        // preprepared data set
        private void GenerateData_Click(object sender, RoutedEventArgs e)
        {
            DetailsTextBlock.Text = string.Empty;
            m_mainView.Children.Clear();

            var ceo = new Worker("John", "Smith", "CEO", 250000, 20);
            var cto = new Worker("Sarah", "Johnson", "CTO", 220000, 18);
            var cfo = new Worker("Michael", "Williams", "CFO", 220000, 17);

            // Technology Department
            var devManager = new Worker("Robert", "Brown", "Development Manager", 180000, 15);
            var devLead1 = new Worker("Emily", "Davis", "Senior Dev Lead", 150000, 12);
            var devLead2 = new Worker("David", "Miller", "Dev Lead", 130000, 10);

            var seniorDev1 = new Worker("Jennifer", "Wilson", "Senior Developer", 120000, 8);
            var seniorDev2 = new Worker("Richard", "Taylor", "Senior Developer", 118000, 7);
            var dev1 = new Worker("Jessica", "Anderson", "Developer", 95000, 5);
            var dev2 = new Worker("Thomas", "Martinez", "Developer", 90000, 4);

            // Finance Department
            var financeManager = new Worker("Elizabeth", "Garcia", "Finance Manager", 160000, 14);
            var accountant1 = new Worker("Daniel", "Robinson", "Senior Accountant", 110000, 9);
            var accountant2 = new Worker("Lisa", "Clark", "Accountant", 85000, 6);

            // HR Department
            var hrManager = new Worker("Patricia", "Rodriguez", "HR Manager", 150000, 12);
            var recruiter = new Worker("Charles", "Lee", "Senior Recruiter", 100000, 8);

            // Build reporting structure
            ceo.AddEmployee(cto);
            ceo.AddEmployee(cfo);
            ceo.AddEmployee(hrManager);

            // Technology team
            cto.AddEmployee(devManager);
            devManager.AddEmployee(devLead1);
            devManager.AddEmployee(devLead2);
            devLead1.AddEmployee(seniorDev1);
            devLead1.AddEmployee(seniorDev2);
            devLead2.AddEmployee(dev1);
            devLead2.AddEmployee(dev2);

            // Finance team
            cfo.AddEmployee(financeManager);
            financeManager.AddEmployee(accountant1);
            financeManager.AddEmployee(accountant2);

            // HR team
            hrManager.AddEmployee(recruiter);

            // Add interns
            var intern1 = new Worker("Kevin", "Harris", "Tech Intern", 40000, 1);
            var intern2 = new Worker("Amanda", "Young", "Finance Intern", 38000, 1);
            dev1.AddEmployee(intern1);
            accountant1.AddEmployee(intern2);

            m_mainView.Children.Add(ceo);

            DataContext = m_mainView;
        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is Worker worker)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {worker.Name}?",
                    "Confirm deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    if (worker.Parent != null)
                    {
                        worker.Parent.Employees.Remove(worker);
                    }
                    else
                    {
                        m_mainView.Children.Remove(worker);
                    }
                }
                trv.Items.Refresh();
                if (trv.SelectedItem == null)
                {
                    DetailsTextBlock.Text = string.Empty;
                }
            }
        }

        private void CreateMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is Worker parentWorker)
            {
                var dialog = new AddWorkerWindow
                {
                    Owner = this
                };

                if (dialog.ShowDialog() == true && dialog.CreatedWorker != null)
                {
                    parentWorker.AddEmployee(dialog.CreatedWorker);
                    trv.Items.Refresh(); // odświeżenie drzewa, jeśli potrzeba
                }
            }

        }

        private void Version_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Wersja: 0b00000001\nData: 10.02.2025", "Informacje o wersji");
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private string GetSubordinatesRecursive(Worker worker, int indentLevel = 1)
        {
            var result = new StringBuilder();
            string indent = new string(' ', indentLevel * 5);

            foreach (var child in worker.Employees)
            {
                result.AppendLine($"{indent}Name: {child.Name}");
                result.AppendLine($"{indent}Surname: {child.Surname}");
                result.AppendLine($"{indent}Position: {child.Position}");
                result.AppendLine($"{indent}Experience: {child.Experience}");
                result.AppendLine($"{indent}Salary: {child.Salary}");
                result.AppendLine($"{indent}Total Subordinates: {child.CountAllSubordinates()}");
                result.AppendLine();

                if (child.Employees.Any())
                {
                    result.Append(GetSubordinatesRecursive(child, indentLevel + 1));
                }
            }

            return result.ToString();
        }
        private void trv_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is Worker worker)
            {
                var sb = new StringBuilder();

                sb.AppendLine($"Name: {worker.Name}");
                sb.AppendLine($"Surname: {worker.Surname}");
                sb.AppendLine($"Position: {worker.Position}");
                sb.AppendLine($"Experience: {worker.Experience}");
                sb.AppendLine($"Salary: {worker.Salary}");
                sb.AppendLine($"Total Subordinates: {worker.CountAllSubordinates()}");
                sb.AppendLine();

                if (worker.Employees.Any())
                {
                    sb.AppendLine("Subordinates:");
                    sb.AppendLine(GetSubordinatesRecursive(worker));
                }

                DetailsTextBlock.Text = sb.ToString();
            }
        }

    }
}
using System.Windows;
using System.Collections.ObjectModel;
using RecursiveObjectBrowser.Model;
using System.Windows.Controls;
using System.Text;
using System;
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Windows.Media;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Windows.Input;

namespace RecursiveObjectBrowser
{
    public partial class MainWindow : Window
    {
        private MainView m_mainView;
        private List<dynamic>? projectionResult;
        private List<Worker> workers;

        public MainWindow()
        {
            InitializeComponent();

            DataContext = this;

            // Initialize the main view model
            m_mainView = new MainView();
            DataContext = m_mainView;

            // Create sample data
            m_mainView = new MainView();
        }

        private List<Worker> GenerateWorkers(int count)
        {
            var rnd = new Random();

            string[] names = { "Anna", "Jan", "Tomasz", "Ewa", "Krzysztof", "Maria", "Wojciech", "Katarzyna"};
            string[] surnames = { "Nowak", "Kowalski", "Wiśniewski", "Wójcik", "Kamińska","Smith","Bolt","Lorney"};
            string[] positions = { "Developer", "Tester", "HR", "Manager", "Accountant","CEO","CTO","CFO"};

            var list = new List<Worker>();

            for (int i = 0; i < count; i++)
            {
                var name = names[rnd.Next(names.Length)];
                var surname = surnames[rnd.Next(surnames.Length)];
                var position = positions[rnd.Next(positions.Length)];
                var salary = rnd.Next(40000, 150000);
                var experience = rnd.Next(1, 21);

                var worker = new Worker(name, surname, position, salary, experience);
                list.Add(worker);
            }

            return list;
        }


        private void WorkersDataGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            var newWorker = e.NewItem as Worker;
            if (newWorker != null)
            {
                newWorker.ID = Guid.NewGuid();
                newWorker.Name = "New";
                newWorker.Surname = "Employee";
                newWorker.Position = "Position";
                newWorker.Salary = 0;
                newWorker.Experience = 0;

                var rnd = new Random();
                newWorker.Details = new WorkerDetails(
                    rnd.Next(1, 11),
                    Math.Round(rnd.NextDouble() * 2000 + 500, 2),
                    (WorkMode)rnd.Next(0, 3)
                );
            }
        }

        private void WorkersDataGrid_RowEditEnding(object sender, DataGridRowEditEndingEventArgs e)
        {
            if (e.EditAction == DataGridEditAction.Commit && e.Row.IsNewItem)
            {
                var worker = e.Row.Item as Worker;
                if (worker != null)
                {
                    m_mainView.Children.Add(worker);
                }
            }
        }
        private void WorkersDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete && !WorkersDataGrid.IsReadOnly)
            {
                var grid = (DataGrid)sender;
                if (grid.SelectedItems.Count > 0)
                {
                    foreach (var item in grid.SelectedItems.Cast<Worker>().ToList())
                    {
                        item.RemoveWithSubordinates(m_mainView.Children);
                    }
                    e.Handled = true;
                }
            }
        }


        // preprepared data set
        private void GenerateData_Click(object sender, RoutedEventArgs e)
        {
            DetailsTextBlock.Text = string.Empty;
            m_mainView.Children.Clear();

            workers = GenerateWorkers(50); 

            
            for (int i = 0; i < workers.Count; i++)
            {
                if (i % 5 == 0)
                {
                    m_mainView.Children.Add(workers[i]); 
                }
                else
                {
                    workers[i - 1].AddEmployee(workers[i]); 
                }
            }

            DataContext = m_mainView;


            // sort by salary
            m_mainView.Children.SortBy<double>("Salary");

            // find worker with a surname "Nowak"
            var found = m_mainView.Children.FindBy("Surname", "Nowak");
            if (found != null)
                MessageBox.Show($"Znaleziono: {found.Name} {found.Surname}");

        }

        private void DeleteMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is MenuItem menuItem && menuItem.DataContext is Worker worker)
            {
                var result = MessageBox.Show(
                    $"Are you sure you want to delete {worker.Name} and all subordinates?",
                    "Confirm deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Pass m_mainView.Children to the RemoveWithSubordinates method
                    worker.RemoveWithSubordinates(m_mainView.Children);

                    trv.Items.Refresh(); // Refresh the TreeView to reflect the changes
                    if (trv.SelectedItem == null)
                    {
                        DetailsTextBlock.Text = string.Empty;
                    }
                }
            }
        }

        private void DeleteWorker_Click(object sender, RoutedEventArgs e)
        {
            // Get the DataGridRow that was right-clicked
            var menuItem = sender as MenuItem;
            var contextMenu = menuItem?.Parent as ContextMenu;
            var dataGridRow = contextMenu?.PlacementTarget as DataGridRow;

            // Get the worker from either the DataGridRow or the selected item
            var workerToDelete = dataGridRow?.Item as Worker ?? WorkersDataGrid.SelectedItem as Worker;

            if (workerToDelete != null)
            {
                string message = $"Are you sure you want to delete this worker?\n\n" +
                               $"Name: {workerToDelete.Name} {workerToDelete.Surname}\n" +
                               $"Position: {workerToDelete.Position}\n" +
                               $"Experience: {workerToDelete.Experience} years\n" +
                               $"Salary: {workerToDelete.Salary:C}\n" +
                               $"Subordinates: {workerToDelete.Employees.Count}";

                MessageBoxResult result = MessageBox.Show(
                    message,
                    "Confirm Deletion",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    // Remove from parent if exists, otherwise remove from root collection
                    if (workerToDelete.Parent != null)
                    {
                        workerToDelete.Parent.Employees.Remove(workerToDelete);
                    }
                    else if (WorkersDataGrid.ItemsSource is ObservableCollection<Worker> rootCollection)
                    {
                        rootCollection.Remove(workerToDelete);
                    }

                    // Refresh the view
                    WorkersDataGrid.Items.Refresh();
                }
            }
            else
            {
                MessageBox.Show("Please select a worker to delete.",
                               "No Selection",
                               MessageBoxButton.OK,
                               MessageBoxImage.Exclamation);
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
        private void PropertyComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (PropertyComboBox.Items.Count == 0)
            {
                var properties = typeof(Worker)
                    .GetProperties()
                    .Where(p => p.PropertyType == typeof(string) || 
                    p.PropertyType == typeof(int) ||
                    p.PropertyType == typeof(Int32) ||
                    p.PropertyType == typeof(double))
                    .Select(p => p.Name);

                foreach (var prop in properties)
                {
                    PropertyComboBox.Items.Add(prop);
                }
            }
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (PropertyComboBox.SelectedItem is string selectedProperty && !string.IsNullOrWhiteSpace(SearchTextBox.Text))
            {
                string filterValue = SearchTextBox.Text.Trim();

                var found = m_mainView.Children.FindBy(selectedProperty, filterValue);


                m_mainView.EmployeesCollectionView.Filter = obj =>
                {
                    if (obj is Worker worker)
                    {
                        var prop = typeof(Worker).GetProperty(selectedProperty);

                        if (prop != null)
                        {
                            object? val = prop.GetValue(worker);
                            if (val != null)
                            {
                                return val.ToString()!.Equals(filterValue, StringComparison.OrdinalIgnoreCase);
                            }
                        }
                    }
                    return false;
                };

                m_mainView.EmployeesCollectionView.Refresh();
            }
        }


        private void AnalyzeProjection_Click(object sender, RoutedEventArgs e)
        {
            projectionResult = m_mainView.Children
                .Where(w => BitConverter.ToInt64(w.ID.ToByteArray(), 0) % 2 != 0)
                .Select(w => new
                    {
                        SUM_OF = w.Details.Rating + w.Details.Bonus,
                        UPPERCASE = w.Details.Mode.ToString().ToUpper()
                    })
                .Cast<dynamic>()
                .ToList();

            var sb = new StringBuilder();

            foreach (var item in projectionResult)
            {
                sb.AppendLine($"UPPERCASE: {item.UPPERCASE}");
                sb.AppendLine($"SUM_OF: {item.SUM_OF:F2}");
                sb.AppendLine();
            }

            DetailsTextBlock.Text = sb.ToString();
        }

        private void AnalyzeGrouping_Click(object sender, RoutedEventArgs e)
        {
            var ss = new StringBuilder();
            if (projectionResult == null || !projectionResult.Any())
            {
                ss.AppendLine($"Firstly run 1 query");
                ss.AppendLine();
            }
            else
            {
                var grouped = projectionResult
                    .GroupBy(p => p.UPPERCASE)
                    .Select(g => new
                    {
                        Group = g.Key,
                        Average = g.Average(x => (double)x.SUM_OF)
                    });

                foreach (var group in grouped)
                {
                    ss.AppendLine($"GROUP: {group.Group}");
                    ss.AppendLine($"AVERAGE SUM_OF: {group.Average:F2}");
                    ss.AppendLine();
                }
            }
            DetailsTextBlock.Text = ss.ToString();
        }
        private void SaveToXML_Click(object sender, RoutedEventArgs e)
        {
            var ss = new StringBuilder();
            try
            {
                // Check if there is any data in the root collection
                if (m_mainView.Children == null || !m_mainView.Children.Any())
                {
                    ss.AppendLine("No data to save. Please generate data first.");
                    DetailsTextBlock.Text = ss.ToString();
                    return;
                }

                // Convert ObservableCollection<Worker> to List<Worker> for serialization
                var workersList = m_mainView.Children.ToList();

                // Serialize the list of workers
                var serializer = new XmlSerializer(typeof(List<Worker>));
                using (var writer = new StreamWriter("workers.xml"))
                {
                    serializer.Serialize(writer, workersList);
                }

                ss.AppendLine($"Save to XML successfully");
            }
            catch (InvalidOperationException ex)
            {
                ss.AppendLine($"Save to XML failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                ss.AppendLine($"Unexpected error: {ex.Message}");
            }

            DetailsTextBlock.Text = ss.ToString();
        }

        private void LoadFromXML_Click(object sender, RoutedEventArgs e)
        {
            var ss = new StringBuilder();
            try
            {
                if (!File.Exists("workers.xml"))
                {
                    ss.AppendLine("XML file not found.");
                    DetailsTextBlock.Text = ss.ToString();
                    return;
                }

                var serializer = new XmlSerializer(typeof(List<Worker>));
                using (var reader = new StreamReader("workers.xml"))
                {
                    // Deserialize and populate the root workers
                    var workers = (List<Worker>)serializer.Deserialize(reader);

                    m_mainView.Children.Clear();
                    foreach (var worker in workers)
                    {
                        m_mainView.Children.Add(worker); 
                    }
                }

                ss.AppendLine("Load from XML successfully.");
            }
            catch (Exception ex)
            {
                ss.AppendLine($"Load from XML failed: {ex.Message}");
            }

            DetailsTextBlock.Text = ss.ToString();
        }

        private void Xpath_Click(object sender, RoutedEventArgs e)
        {
            var result = new StringBuilder();
            if (!File.Exists("workers.xml"))
            {
                DetailsTextBlock.Text = "File workers.xml not found";
                return;
            }
            try
            {

                XDocument doc = XDocument.Load("workers.xml");
                var workers = doc.XPathSelectElements("//Worker");

                var seenRatings = new HashSet<string>();
                foreach (var worker in workers)
                {
                    var rating = worker.Element("Details")?.Element("Rating")?.Value;

                    if (string.IsNullOrEmpty(rating) || !seenRatings.Add(rating))
                        continue;
                    result.AppendLine($"Name: {worker.Element("Name")?.Value}");
                    result.AppendLine($"Surname: {worker.Element("Surname")?.Value}");
                    result.AppendLine($"Position: {worker.Element("Position")?.Value}");
                    result.AppendLine($"Experience: {worker.Element("Experience")?.Value}");
                    result.AppendLine($"Salary: {worker.Element("Salary")?.Value}");

                    var details = worker.Element("Details");
                    result.AppendLine($"Rating: {details?.Element("Rating")?.Value}");
                    result.AppendLine($"Bonus: {details?.Element("Bonus")?.Value}");
                    result.AppendLine($"Mode: {details?.Element("Mode")?.Value}");
                    result.AppendLine();
                }


                DetailsTextBlock.Text = result.ToString();
            }
            catch (Exception ex)
            {
                DetailsTextBlock.Text = $"Error processing XML: {ex.Message}";
            }
        }

        private void GenerateXHTML_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a properly formatted XHTML document
                var xhtml = XNamespace.Get("http://www.w3.org/1999/xhtml");

                var xhtmlDoc = new XDocument(
                    new XDeclaration("1.0", "utf-8", null),
                    new XDocumentType("html",
                        "-//W3C//DTD XHTML 1.0 Strict//EN",
                        "http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd",
                        null),
                    new XElement(xhtml + "html",
                        new XAttribute("xmlns", xhtml.NamespaceName),
                        new XAttribute(XNamespace.Xml + "lang", "en"),
                        new XElement(xhtml + "head",
                            new XElement(xhtml + "title", "Workers Table"),
                            new XElement(xhtml + "meta",
                                new XAttribute("http-equiv", "Content-Type"),
                                new XAttribute("content", "text/html; charset=utf-8")),
                            new XElement(xhtml + "style",
                                new XAttribute("type", "text/css"),
                                @"body { font-family: Arial, sans-serif; margin: 20px; }
                  h1 { color: #333; }
                  table { border-collapse: collapse; width: 100%; margin-top: 20px; }
                  th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
                  th { background-color: #f2f2f2; }
                  tr:nth-child(even) { background-color: #f9f9f9; }")
                        ),
                        new XElement(xhtml + "body",
                            new XElement(xhtml + "h1", "Workers List"),
                            new XElement(xhtml + "table",
                                new XElement(xhtml + "tr",
                                    new XElement(xhtml + "th", "Name"),
                                    new XElement(xhtml + "th", "Surname"),
                                    new XElement(xhtml + "th", "Position"),
                                    new XElement(xhtml + "th", "Salary"),
                                    new XElement(xhtml + "th", "Experience")
                                ),
                                from worker in workers
                                select new XElement(xhtml + "tr",
                                    new XElement(xhtml + "td", worker.Name),
                                    new XElement(xhtml + "td", worker.Surname),
                                    new XElement(xhtml + "td", worker.Position),
                                    new XElement(xhtml + "td", worker.Salary),
                                    new XElement(xhtml + "td", worker.Experience)
                                )
                            )
                        )
                    )
                );


                // Create writer settings for proper formatting without BOM
                var writerSettings = new XmlWriterSettings
                {
                    Indent = true,
                    IndentChars = "  ",
                    NewLineChars = "\n",
                    NewLineHandling = NewLineHandling.Replace,
                    OmitXmlDeclaration = false,
                    Encoding = new UTF8Encoding(false) // This prevents BOM
                };

                // Save with proper formatting
                using (var writer = XmlWriter.Create("workers.xhtml", writerSettings))
                {
                    xhtmlDoc.Save(writer);
                }

                // Open in default browser
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "workers.xhtml",
                    UseShellExecute = true
                });

                DetailsTextBlock.Text = "Valid XHTML document generated and opened in browser!";


            }
            catch (Exception ex)
            {
                DetailsTextBlock.Text = $"Error generating XHTML: {ex.Message}";
            }
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
                result.AppendLine($"{indent}Rating: {child.Details.Rating}");
                result.AppendLine($"{indent}Bonus: {child.Details.Bonus}");
                result.AppendLine($"{indent}Work Mode: {child.Details.Mode}");
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
                sb.AppendLine($"Rating: {worker.Details.Rating}");
                sb.AppendLine($"Bonus: {worker.Details.Bonus}");
                sb.AppendLine($"Work Mode: {worker.Details.Mode}");
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
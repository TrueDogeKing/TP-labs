using System;
using System.Windows;
using System.Windows.Controls;
using RecursiveObjectBrowser.Model;

namespace RecursiveObjectBrowser
{
    public partial class AddWorkerWindow : Window
    {
        public Worker? CreatedWorker { get; private set; }

        public AddWorkerWindow()
        {
            InitializeComponent();
        }

        private void AddWorker_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(NameBox.Text) || string.IsNullOrWhiteSpace(SurnameBox.Text))
            {
                MessageBox.Show("Imię i nazwisko są wymagane.");
                return;
            }

            if (!double.TryParse(SalaryBox.Text, out double salary) || salary <= 0)
            {
                MessageBox.Show("Niepoprawna pensja.");
                return;
            }

            if (!int.TryParse(ExperienceBox.Text, out int experience) || experience < 0)
            {
                MessageBox.Show("Niepoprawne doświadczenie.");
                return;
            }

            CreatedWorker = new Worker(NameBox.Text, SurnameBox.Text, PositionBox.Text, salary, experience);
            DialogResult = true;
        }

        private void GenerateRandom_Click(object sender, RoutedEventArgs e)
        {
            var rnd = new Random();
            string[] names = { "Anna", "Tomasz", "Monika", "Jan", "Ewa" };
            string[] surnames = { "Nowak", "Wiśniewski", "Kowalski", "Zając", "Kamińska" };
            string[] positions = { "Developer", "HR", "Finanse", "Tester" };

            NameBox.Text = names[rnd.Next(names.Length)];
            SurnameBox.Text = surnames[rnd.Next(surnames.Length)];
            PositionBox.Text = positions[rnd.Next(positions.Length)];
            SalaryBox.Text = rnd.Next(40000, 120000).ToString();
            ExperienceBox.Text = rnd.Next(1, 20).ToString();
        }
    }
}

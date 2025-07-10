using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Delegations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private CancellationTokenSource _cts;

        public MainWindow()
        {
            InitializeComponent();
        }

        private int _globalProgress = 0;
        private object _progressLock = new object();

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
         
            RunButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            SimulationProgressBar.Value = 0;
            OutputTextBox.Clear();

            _cts = new CancellationTokenSource();
            CancellationToken token = _cts.Token;

            int threadsNumber = 16;
            if (int.TryParse(ThreadsTextBox.Text, out int parsed))
            {
                threadsNumber = parsed;
            }

            AppendOutput($"[Main] Number of threads: {threadsNumber}");



            string filename = "population.txt";
            int populationSize;

            try
            {
                using var reader = new StreamReader(filename);
                var line = await reader.ReadLineAsync();
                populationSize = int.Parse(line.Trim());
            }
            catch (Exception ex)
            {
                AppendOutput($"[Error] {ex.Message}");
                RunButton.IsEnabled = true;
                return;
            }

            Population population = new Population(populationSize);
            PopulationLoader.LoadPopulationFromFile(filename, population);

           
            int index = populationSize / threadsNumber;
            int remainder = populationSize % threadsNumber;
            int startIdx = 0;
            int simulationTime = 10;

            var tasks = new List<Task>();


             DateTime simStart = DateTime.Now;
            AppendOutput("[Main] Simulation started...");


            var progress = new Progress<int>(_ =>
            {
                lock (_progressLock)
                {
                    _globalProgress++;
                    double totalSteps = threadsNumber * simulationTime;
                    double percent = (double)_globalProgress / totalSteps * 100;

                    Dispatcher.Invoke(() =>
                    {
                        SimulationProgressBar.Value = percent;
                    });
                }
            });


            for (int i = 0; i < threadsNumber; i++)
            {
                int endIdx = startIdx + index + (i < remainder ? 1 : 0);
                var sim = new Simulation(population, startIdx, endIdx, i, simulationTime, token, progress, Log);
                tasks.Add(sim.Run());
                startIdx = endIdx;
            }



            await Task.WhenAll(tasks);
           

            DateTime simEnd = DateTime.Now;

            if (_cts.IsCancellationRequested)
            {
                AppendOutput("[Main] Simulation was cancelled successfully.");
                SimulationProgressBar.Value = 0;
            }
            else
            {

                AppendOutput($"[Main] Simulation completed in {(simEnd - simStart).TotalSeconds:F2} seconds");
                SimulationProgressBar.Value = 100;
            }


            if (!_cts.IsCancellationRequested)
            {
                int healthy = 0, infected = 0, recovered = 0;
                for (int i = 0; i < populationSize; i++)
                {
                    var person = population.GetPerson(i);
                    if (person.HasRecovered) recovered++;
                    else if (person.IsInfected) infected++;
                    else healthy++;
                }

                AppendOutput($"\n[Main] Simulation results after {simulationTime} days:");
                AppendOutput($"- Healthy: {healthy} ({(double)healthy / populationSize:P2})");
                AppendOutput($"- Infected: {infected} ({(double)infected / populationSize:P2})");
                AppendOutput($"- Recovered: {recovered} ({(double)recovered / populationSize:P2})");
            }


            SimulationProgressBar.Value = 100;
            RunButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void AppendOutput(string message)
        {
            OutputTextBox.AppendText(message + Environment.NewLine);
            OutputTextBox.ScrollToEnd();
        }

        private void Log(string message)
        {
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText(message + Environment.NewLine);
                OutputTextBox.ScrollToEnd();
            });
        }


        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (_cts != null)
            {
                _cts.Cancel();
                AppendOutput("[Main] Simulation cancellation requested.");
            }
        }




    }
}


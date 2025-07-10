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

            var phase1Barrier = new Barrier(threadsNumber);
            var phase2Barrier = new Barrier(threadsNumber);

            int index = populationSize / threadsNumber;
            int remainder = populationSize % threadsNumber;
            int startIdx = 0;
            int simulationTime = 10;


            var tasks = new List<Task<SimulationResult>>();


            DateTime simStart = DateTime.Now;
            AppendOutput("[Main] Simulation started...");

            var progress = new Progress<int>(day =>
            {
                double percent = (double)day / simulationTime * 100;
                SimulationProgressBar.Value = percent;
            });


            for (int i = 0; i < threadsNumber; i++)
            {
                int endIdx = startIdx + index + (i < remainder ? 1 : 0);
                var sim = new Simulation(population, startIdx, endIdx, i, phase1Barrier, phase2Barrier, simulationTime, token, progress, Log);
                tasks.Add(Task.Run(() => sim.Run()));
                startIdx = endIdx;
            }



            var results = await Task.WhenAll(tasks);

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
                int totalHealthy = 0, totalInfected = 0, totalRecovered = 0;
                foreach (var result in results)
                {
                    totalHealthy += result.Healthy;
                    totalInfected += result.Infected;
                    totalRecovered += result.Recovered;
                    AppendOutput($"[Thread {result.ThreadNumber}]  | Healthy: {result.Healthy} | Infected: {result.Infected} | Recovered: {result.Recovered}");
                }

                AppendOutput($"\n[Main] Aggregated Results:");
                AppendOutput($"- Healthy: {totalHealthy}");
                AppendOutput($"- Infected: {totalInfected}");
                AppendOutput($"- Recovered: {totalRecovered}");
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


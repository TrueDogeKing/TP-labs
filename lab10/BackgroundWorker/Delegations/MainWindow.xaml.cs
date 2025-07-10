using System.ComponentModel;
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
    public partial class MainWindow : Window
    {
        private BackgroundWorker worker;
        private CancellationTokenSource cts;

        public MainWindow()
        {
            InitializeComponent();
            InitializeBackgroundWorker();
        }

        private void InitializeBackgroundWorker()
        {
            worker = new BackgroundWorker
            {
                WorkerReportsProgress = true,
                WorkerSupportsCancellation = true
            };

            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
        }

        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
            RunButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            

            SimulationProgressBar.Value = 0;
            OutputTextBox.Clear();

            if (!worker.IsBusy)
            {
                worker.RunWorkerAsync();
                cts = new CancellationTokenSource();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            int threadsNumber = 16;
            Dispatcher.Invoke(() =>
            {
                if (int.TryParse(ThreadsTextBox.Text, out int parsed))
                    threadsNumber = parsed;
                AppendOutput($"[Main] Number of threads: {threadsNumber}");
            });

            string filename = "population.txt";
            int populationSize;

            try
            {
                using var reader = new StreamReader(filename);
                var line = reader.ReadLine();
                populationSize = int.Parse(line.Trim());
            }
            catch (Exception ex)
            {
                Dispatcher.Invoke(() => AppendOutput($"[Error] {ex.Message}"));
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

            var threadList = new List<Thread>();
            DateTime simStart = DateTime.Now;
            Dispatcher.Invoke(() => AppendOutput("[Main] Simulation started..."));

            int progressDay = 0;
            object lockObj = new object();

            for (int i = 0; i < threadsNumber; i++)
            {
                
                int endIdx = startIdx + index + (i < remainder ? 1 : 0);
                int threadId = i;
                int threadStart = startIdx;
                int threadEnd = endIdx;

                var sim = new Simulation(population, threadStart, threadEnd, threadId, phase1Barrier, phase2Barrier, simulationTime,
                    cts.Token, new Progress<int>(day =>
                    {
                        lock (lockObj)
                        {
                            if (day > progressDay)
                            {
                                progressDay = day;
                                worker.ReportProgress((int)((double)day / simulationTime * 100));
                            }
                        }
                    }),
                    message => Dispatcher.Invoke(() => AppendOutput(message)));
                
                var thread = new Thread(sim.Run);
                
                threadList.Add(thread);
                startIdx = endIdx;
            }

            foreach (var t in threadList)
                t.Start();
            

            foreach (var t in threadList)
                t.Join();
            if (cts.Token.IsCancellationRequested)
            {
                e.Cancel = true;
                return;
            }


            e.Result = new
            {
                population,
                simulationTime,
                duration = (DateTime.Now - simStart).TotalSeconds
            };
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            SimulationProgressBar.Value = e.ProgressPercentage;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                AppendOutput("[Main] Simulation was cancelled.");
                SimulationProgressBar.Value = 0;
            }
            else if (e.Error != null)
            {
                AppendOutput($"[Error] {e.Error.Message}");
            }
            else
            {
                var result = (dynamic)e.Result;
                var population = result.population;
                int simulationTime = result.simulationTime;
                double duration = result.duration;

                AppendOutput($"[Main] Simulation completed in {duration:F2} seconds");
                SimulationProgressBar.Value = 100;

                int healthy = 0, infected = 0, recovered = 0;
                for (int i = 0; i < population.GetSize(); i++)
                {
                    var person = population.GetPerson(i);
                    if (person.HasRecovered) recovered++;
                    else if (person.IsInfected) infected++;
                    else healthy++;
                }

                AppendOutput($"\n[Main] Simulation results after {simulationTime} days:");
                AppendOutput($"- Healthy: {healthy} ({(double)healthy / population.GetSize():P2})");
                AppendOutput($"- Infected: {infected} ({(double)infected / population.GetSize():P2})");
                AppendOutput($"- Recovered: {recovered} ({(double)recovered / population.GetSize():P2})");
            }

            RunButton.IsEnabled = true;
            StopButton.IsEnabled = false;
        }

        private void StopButton_Click(object sender, RoutedEventArgs e)
        {
            if (worker.IsBusy && worker.WorkerSupportsCancellation)
            {
                worker.CancelAsync();
                cts?.Cancel();
                AppendOutput("[Main] Simulation cancellation requested.");
            }
        }

        private void AppendOutput(string message)
        {
            OutputTextBox.AppendText(message + Environment.NewLine);
            OutputTextBox.ScrollToEnd();
        }
    }
}


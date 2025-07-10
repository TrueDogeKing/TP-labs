using System;
using System.Collections.Generic;
using System.Threading;

public class Simulation
{
    private readonly Population population;
    private int startIdx;
    private int endIdx;
    private readonly Barrier phase1Barrier;
    private readonly Barrier phase2Barrier;
    private bool running = true;
    private int threadNumber;
    private int simulationDays;

    private long startTime;
    private long endTime;
    private double totalPhase1Time = 0;
    private double totalPhase2Time = 0;
    private int iterationCount = 0;

    private List<int> infectedList = new List<int>();


    private readonly CancellationToken _token;

    private readonly IProgress<int> _progressReporter;
    private readonly Action<string> _log;


    // Constructor to accept the population array and indices of the segment this thread will handle
    public Simulation(Population population, int startIdx, int endIdx, int threadNumber,
     int simulationDays, CancellationToken token, IProgress<int> progressReporter, Action<string> log = null)
    {
        this.population = population;
        this.startIdx = startIdx;
        this.endIdx = endIdx;
        this.threadNumber = threadNumber;
        this.simulationDays = simulationDays;
        this._token = token;
        this._progressReporter = progressReporter;
        this._log = log;

        _log?.Invoke($"Created thread number {threadNumber} start index:{startIdx} end index:{endIdx}");
    }

    // Method that will be executed by each thread
    public async Task Run()
    {
        _log?.Invoke($"Thread {threadNumber} started for {simulationDays} days");

        startTime = DateTime.Now.Ticks;

        for (int day = 0; day < simulationDays && running; day++)
        {
            if (_token.IsCancellationRequested)
            {
                _log?.Invoke($"[Thread {threadNumber}] Simulation cancelled.");
                return;
            }

            // Faza 1 – symulacja infekcji i aktualizacja statusu
            for (int i = startIdx; i < endIdx; i++)
            {
                Person person = population.GetPerson(i);
                SimulateInfection(person);
                person.UpdateInfectionStatus();
            }

            // Faza 2 – zaktualizuj osoby z listy zainfekowanych
            foreach (var infectedId in infectedList)
            {
                population.UpdatePerson(infectedId, true, 1, false);
            }
            infectedList.Clear();

            _progressReporter?.Report(day + 1);

            // Przerwa asynchroniczna – żeby oddać kontrolę GUI
            await Task.Delay(1000);
        }

        endTime = DateTime.Now.Ticks;
        _log?.Invoke($"Thread {threadNumber} completed after {simulationDays} days.");
    }




    // Helper method to get the elapsed time in seconds
    private double GetTime()
    {
        return (endTime - startTime) / 1000.0;
    }

    // This method contains the logic for spreading the infection or advancing infection days
    private void SimulateInfection(Person person)
    {
        Random random = new Random();
        for (int j = 0; j < person.GetContactsPerDay(); j++)
        {
            int randomIndex = random.Next(1000); // Randomly pick another person

            Person otherPerson = population.GetPerson(randomIndex);

            // Infect another person if conditions are met
            if (otherPerson != null && otherPerson.CanBeInfected() && person.GetIsInfected())
            {
                double infectionChance = 0.1;
                if (random.NextDouble() < infectionChance)
                {
                    infectedList.Add(randomIndex);
                }
            }

            // The current person can also be infected
            if (otherPerson != null && otherPerson.GetIsInfected() && !person.GetIsInfected())
            {
                double infectionChance = 0.1;
                if (random.NextDouble() < infectionChance)
                {
                    infectedList.Add(person.GetId());
                }
            }
        }
    }

    // Helper method to stop the simulation
    public void Stop()
    {
        running = false;
        phase1Barrier.SignalAndWait();
        phase2Barrier.SignalAndWait();
    }

    // Helper method to get the average time for Phase 1
    public double GetAveragePhase1Time()
    {
        return (iterationCount == 0) ? 0 : totalPhase1Time / iterationCount / 1000.0;
    }

    // Helper method to get the average time for Phase 2
    public double GetAveragePhase2Time()
    {
        return (iterationCount == 0) ? 0 : totalPhase2Time / iterationCount / 1000.0;
    }

    
}

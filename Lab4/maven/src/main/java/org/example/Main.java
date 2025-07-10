package org.example;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

import java.util.Scanner;
import java.util.concurrent.ExecutorService;
import java.util.concurrent.Executors;

public class Main {
    // Main method for testing

    public static void main(String[] args) {

        int threadsNumber=4;
        if (args.length > 0) {
            try {
                threadsNumber = Integer.parseInt(args[0]);
            } catch (NumberFormatException e) {
                System.out.println("Number of threads: 4.");
            }
        }

        System.out.println("Number of threads: " + threadsNumber);


        // File path for population data
        String filename = "population.txt";

        // Determine population size from the file
        int populationSize = 0;
        try (BufferedReader reader = new BufferedReader(new FileReader(filename))) {
            // Read the first line to get the population size
            String line = reader.readLine();
            populationSize = Integer.parseInt(line.trim());
        } catch (IOException e) {
            e.printStackTrace();
            return;
        }

        // Create the population array with the size specified in the file
        Population population = new Population(populationSize);
        ExecutorService executor = Executors.newFixedThreadPool(threadsNumber);

        PopulationLoader populationLoader = new PopulationLoader();
        // Measure the time taken to read the population data
        long startTime = System.currentTimeMillis();
        populationLoader.loadPopulationFromFile(filename, population);
        long endTime = System.currentTimeMillis();

        // Output time taken to read the file
        System.out.println("File reading took " + (endTime - startTime) / 1000.0 + " seconds.");

        // Optionally, display the first few people in the population (for debugging)

        for (int i = 0; i < populationSize; i++) {
            System.out.println("Person ID: " + population.getPerson(i).id + " Infected: " + population.getPerson(i).isInfected());
        }



        System.out.println(" start multithreading" );




        int index=populationSize/threadsNumber;
        int remainder = populationSize % threadsNumber;
        Simulation[] simulationsArray=new Simulation[threadsNumber];
        Thread[] threadsArray=new Thread[threadsNumber];
        int startIdx = 0;
        for(int i=0;i<threadsNumber;i++){
            int endIdx = startIdx + index + (i < remainder ? 1 : 0);
            if (endIdx > populationSize) endIdx = populationSize;
            simulationsArray[i]=new Simulation(population,startIdx,endIdx,i);
            threadsArray[i]=new Thread(simulationsArray[i]);
            startIdx = endIdx;
        }


        for(int j=0;j<threadsNumber;j++){
            threadsArray[j] = new Thread(simulationsArray[j]);  // Create a new thread if the previous one finished
            threadsArray[j].start();
        }

        //Commander commander=new Commander();
        //Thread commanderThread=new Thread(commander);
        //commanderThread.start();

        startTime = System.currentTimeMillis();
        int timeOfSimulation = 20;
        displayInfectedPeople(population);
        // Critical section
        for (int i = 0; i < timeOfSimulation; i++) {
            if(i==0){
                ActivatingNextStage(simulationsArray,true);
            }
            // if so the next day won't start after the end of simulation
            if(i!=timeOfSimulation) {
                System.out.println("Waking up threads for next cycle...");
                ActivatingNextStage(simulationsArray,false);
            }



            System.out.println("Waiting for Phase 1 (infection simulation) to finish...");
            WaitingPhase(true,simulationsArray,threadsNumber);



            System.out.println("Waking up threads for Phase 2 (update population)...");
            ActivatingNextStage(simulationsArray,true);



            System.out.println("Waiting for Phase 2 (update population) to finish...");

            WaitingPhase(false,simulationsArray,threadsNumber);


            displayInfectedPeople(population);
            System.out.println("day "+i+" finnished");

            /*
            if(!commanderThread.isAlive()){
                break;
            }

             */





        }

        System.out.println("Stopping threads...");
        for (Simulation sim : simulationsArray) {
            sim.stop(); // Signal threads to stop
        }

        System.out.println("All threads stopped. Simulation complete.");


        endTime = System.currentTimeMillis();
        System.out.println("Simulation took " + (endTime - startTime) / 1000.0 + " seconds.");

        double totalPhase1 = 0;
        double totalPhase2 = 0;

        for (int i = 0; i < threadsNumber; i++) {
            totalPhase1 += simulationsArray[i].getAveragePhase1Time();
            totalPhase2 += simulationsArray[i].getAveragePhase2Time();
        }

        double avgPhase1 = totalPhase1 / threadsNumber;
        double avgPhase2 = totalPhase2 / threadsNumber;

        System.out.printf("Average pahse 1 time: %.4f s%n", avgPhase1);
        System.out.printf("Average pahse 2 time: %.4f s%n", avgPhase2);


    }

    public static void displayInfectedPeople(Population population) {
        System.out.println("infected people:");
        int count = 0;
        for (int i = 0; i < population.getSize(); i++) {
            Person person = population.getPerson(i);
            if (person != null && person.isInfected()) {
                //System.out.println("ID: " + person.id + " Days of infection: " + person.getInfectionDays());
                count++;
            }
        }
        System.out.println("Total number of people are sick: " + count);
    }

    public static void WaitingPhase(boolean firstPhase,Simulation[] simulationsArray,int threadsNumber) {
        while (true) {
            int waitingThreads = 0;
            for (Simulation sim : simulationsArray) {
                if (sim.isWaitingPhase1() && firstPhase) {
                    waitingThreads++;
                }
                if (sim.isWaitingPhase2() && !firstPhase) {
                    waitingThreads++;
                }
            }
            if (waitingThreads == threadsNumber) {
                break;
            }
        }
    }

    public static void ActivatingNextStage(Simulation[] simulationsArray,boolean firstPhase) {
        for (Simulation sim : simulationsArray) {
            sim.resumeWork(firstPhase);
        }
    }


}



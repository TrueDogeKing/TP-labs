package org.example;

import java.util.*;

public class Main {
    // Main method for testing

    public static void main(String[] args) {
        if (args.length == 0) {
            System.out.println("Run program with added parameters in configuration");
            return;
        }
        boolean isSorted;
        String mode = args[0];
        WorkerComparator comparator;

        // Options of sorting elements depending on argument
        if (mode.equals("Natural")) {
            isSorted=true;
            comparator=null;
        } else if (mode.equals("Alternative")) {
            isSorted=true;
            comparator = new WorkerComparator();
        } else {
            isSorted=false;
            comparator=null;
        }

        // Parametric number of animals
        final int NUM_ANIMALS = 6;

        // Creating variable for all animals
        Set<Worker> allAnimals = isSorted
                ? (comparator != null ? new TreeSet<>(comparator) : new TreeSet<>())
                : new HashSet<>();

        // Dynamic creating of animals
        Worker[] animals = new Worker[NUM_ANIMALS];
        String[] names = {"Lion", "Tiger", "Wolf", "Jaguar", "Turtle", "Snake"};
        String[] surnames = {"Kowalski","Dedyk","dfjmdjgrh","sdjwhdwhfr","dkiwdhiehfe","fgjhht"};
        String[] positions = {"junior","senior","senior","ceo","project manager","hr"};
        double[] salary = {120.5, 115.0, 80.0, 130.2, 400.2, 50.5};
        int[] experiences = {50, 55, 30, 60, 5, 10};

        for (int i = 0; i < NUM_ANIMALS; i++) {
            animals[i] = new Worker(names[i], surnames[i],positions[i],salary[i],experiences[i], isSorted, comparator);
            allAnimals.add(animals[i]);
        }

        // Adding relations
        if (NUM_ANIMALS >= 3) {
            animals[0].addRelatedWorker(animals[1]); // Lion → Tiger
            animals[0].addRelatedWorker(animals[2]); // Lion → Wolf
        }
        if (NUM_ANIMALS >= 5) {
            animals[3].addRelatedWorker(animals[4]); // Jaguar → Turtle
        }
        if (NUM_ANIMALS >= 6) {
            animals[4].addRelatedWorker(animals[5]); // Turtle → Snake
        }

        // Identifying root elements and visualization of data
        for (Worker animal : allAnimals) {
            boolean isChild = false;
            for (Worker parent : allAnimals) {
                if (parent.getEmplyees().contains(animal)) {
                    isChild = true;
                    break;
                }
            }
            if (!isChild) {
                animal.printStructure(animal, "");
            }

        }
        generateStatistics(allAnimals, isSorted);

    }
    // Method to generate statistics on the number of subordinate elements
    public static void generateStatistics(Set<Worker> allAnimals, boolean isSorted) {
        Map<Worker, Integer> statistics = isSorted ? new TreeMap<>() : new HashMap<>();

        // Populate statistics with the number of subordinate animals
        for (Worker animal:  allAnimals) {
            statistics.put(animal, animal.countAllSubordinates());
        }

        // Print the results to the console
        System.out.println("\nStatistics of subordinate elements:");
        for (Map.Entry<Worker, Integer> entry : statistics.entrySet()) {
            System.out.println(entry.getKey().getName() + " → " + entry.getValue());
        }
    }


}



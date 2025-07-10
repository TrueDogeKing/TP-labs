package org.example;

import java.io.BufferedReader;
import java.io.FileReader;
import java.io.IOException;

public class PopulationLoader implements  Runnable{


    @Override
    public void run() {
        // potem dodam wczytywanie wielowątkowe

    }

    public static void loadPopulationFromFile(String filename, Population population) {
        try (BufferedReader reader = new BufferedReader(new FileReader(filename))) {
            String line;

            // Skip the first line that contains the population count
            reader.readLine();

            while ((line = reader.readLine()) != null ) {
                String[] data = line.split(",");
                int id = Integer.parseInt(data[0].trim());
                boolean isInfected = Boolean.parseBoolean(data[1].trim());
                int infectionDays = Integer.parseInt(data[2].trim());
                int contactsPerDay = Integer.parseInt(data[3].trim());
                boolean hasRecovered = Boolean.parseBoolean(data[4].trim());

                // Create Person object and add to the array
                population.setPerson(id-1,new Person(id,isInfected, infectionDays, contactsPerDay, hasRecovered));
            }
        } catch (IOException e) {
            e.printStackTrace();
        }
    }


}


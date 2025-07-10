package org.example;


import java.io.Serializable;

public class SimulationResult implements Serializable {

    private static final long serialVersionUID = 1L;

    private int totalInfected;
    private int populationSize;

    public SimulationResult(int totalInfected, int populationSize) {
        this.totalInfected = totalInfected;
        this.populationSize = populationSize;
    }

    public int getTotalInfected() {
        return totalInfected;
    }

    public int getPopulationSize() {
        return populationSize;
    }

    @Override
    public String toString() {
        return "Simulation result: " + totalInfected + " infected out of " + populationSize + ".";
    }
}


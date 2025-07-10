package org.example;


import static java.lang.Thread.sleep;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;
import java.util.List;
import java.util.ArrayList;
import java.util.Random;

public class Simulation implements Runnable {

    private final Population population;
    private int startIdx;
    private int endIdx;
    private final Lock lock = new ReentrantLock();
    private final Condition condition = lock.newCondition();
    private boolean waitingPhase1 = true;
    private boolean waitingPhase2 = false;
    private volatile boolean running = true; // Stop flag
    private int threadNumber;

    private long startTime;
    private long endTime;
    private double totalPhase1Time = 0;
    private double totalPhase2Time = 0;
    private int iterationCount = 0;

    List<Integer> infectedList = new ArrayList<>();


    // Constructor to accept the population array and indices of the segment this thread will handle
    public Simulation(Population population, int startIdx, int endIdx,int threadNumber) {
        this.population = population;
        this.startIdx = startIdx;
        this.endIdx = endIdx;
        this.threadNumber=threadNumber;
        System.out.println("Created thread number "+threadNumber+" start index:"+startIdx+" end index:"+endIdx);
    }


    @Override
    public void run() {
        System.out.println("thread started: " );
        while (running) {
            if (!running) break;



            lock.lock();
            try {
                while (waitingPhase1 && running) {
                    condition.await(); // Wait until signaled
                }
                if (!running) {
                    break;
                }
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                return;
            } finally {
                lock.unlock();
            }

            long counter=0;
            startTime = System.currentTimeMillis();
            for (int i = startIdx; i < endIdx; i++) {
                for (int j = startIdx; j < endIdx; j++){counter++;}

            }


            // First phase: Infection simulation
            for (int i = startIdx; i < endIdx; i++) {
                Person person = population.getPerson(i);
                simulateInfection(person);
                person.updateInfectionStatus();
                for (int j = startIdx; j < endIdx; j++){}

            }

            endTime = System.currentTimeMillis();
            totalPhase1Time += (endTime - startTime);
            System.out.println("Phase 1 completed simulation took " +getTime()+ "s thread number:"+threadNumber);



            // Second waiting phase before updating the population array
            lock.lock();
            try {
                waitingPhase1 = true;
                condition.await();
                if (!running) {
                    break;
                }
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                return;
            } finally {
                lock.unlock();
            }

            startTime = System.currentTimeMillis();

            // Second phase: Update infected population
            for (Integer infectedId : infectedList) {
                population.updatePerson(infectedId, true, 1, false);
            }

            infectedList.clear();


            endTime = System.currentTimeMillis();
            totalPhase2Time += (endTime - startTime);
            System.out.println("Phase 2 completed simulation took " +getTime()+ "s thread number:"+threadNumber);


            lock.lock();
            try {
                waitingPhase2 = true;
                iterationCount++;
                condition.await();
                if (!running) {
                    break;
                }
            } catch (InterruptedException e) {
                Thread.currentThread().interrupt();
                return;
            } finally {
                lock.unlock();
            }
        }
        System.out.println("Thread stopping: " + Thread.currentThread().getName());
    }

    public void resumeWork(boolean firstPhase) {
        lock.lock();
        try {
            if (firstPhase) {
                waitingPhase1 = false;
            } else {
                waitingPhase2 = false;
            }
            condition.signalAll(); // Wake up threads
        } finally {
            lock.unlock();
        }
    }

    public boolean isWaitingPhase1() {
        lock.lock();
        try {
            return waitingPhase1;
        } finally {
            lock.unlock();
        }
    }

    public boolean isWaitingPhase2() {
        lock.lock();
        try {
            return waitingPhase2;
        } finally {
            lock.unlock();
        }
    }

    public double getTime(){
        return (endTime - startTime) / 1000.0;
    }

    public void stop() {
        lock.lock();
        try {
            running = false;
            waitingPhase1 = false;
            waitingPhase2 = false;
            condition.signalAll(); // Wake up thread so it can exit
        } finally {
            lock.unlock();
        }
    }
    private synchronized void addInfected(int index) {
        infectedList.add(index);
    }


    // This method can contain logic for spreading the infection or advancing infection days
    private void simulateInfection(Person person) {
        Random random = new Random();
        for (int j = 0; j < person.getContactsPerDay(); j++) {
            int randomIndex = random.nextInt(1000) + 1;

            Person otherPerson = population.getPerson(randomIndex);

            if (otherPerson != null && otherPerson.canBeInfect() && person.isInfected()) {
                double infectionChance = 0.1;
                if (Math.random() < infectionChance) {
                    infectedList.add(randomIndex);
                }
            }

            if (otherPerson != null && otherPerson.isInfected() && !person.isInfected()) {
                double infectionChance = 0.1;
                if (Math.random() < infectionChance) {
                    infectedList.add(person.id);
                }
            }

        }


    }

    public double getAveragePhase1Time() {
        return (iterationCount == 0) ? 0 : totalPhase1Time / iterationCount / 1000.0;
    }

    public double getAveragePhase2Time() {
        return (iterationCount== 0) ? 0 : totalPhase2Time / iterationCount / 1000.0;
    }

}

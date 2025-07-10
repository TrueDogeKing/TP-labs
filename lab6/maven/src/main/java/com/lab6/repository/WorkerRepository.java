package com.lab6.repository;

import com.lab6.model.Worker;

import java.util.ArrayList;
import java.util.Collection;
import java.util.Optional;

public class WorkerRepository {
    private final Collection<Worker> collection;

    public WorkerRepository() {
        this.collection = new ArrayList<>();
    }
    public Optional<Worker> find(String name) {
        for (Worker m : collection) {
            if (m.getName().equals(name)) {
                return Optional.of(m);
            }
        }
        return Optional.empty();
    }

    public void delete(String name) throws IllegalArgumentException {
        for (Worker m : collection) {
            if (m.getName().equals(name)) {
                collection.remove(m);
                return;
            }
        }
        throw new IllegalArgumentException("Mage with name " + name + " not found");
    }

    public void save(Worker worker) throws IllegalArgumentException {
        for (Worker m : collection) {
            if (m.getName().equals(worker.getName())) {
                throw new IllegalArgumentException("Mage with name " + worker.getName() + " already exists");
            }
        }
        collection.add(worker);
    }
}

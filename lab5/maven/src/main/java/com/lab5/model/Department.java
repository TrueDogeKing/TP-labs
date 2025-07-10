package com.lab5.model;

import com.lab5.Database;

import java.util.ArrayList;
import java.util.List;

import java.util.logging.Logger;

import javax.persistence.CascadeType;
import javax.persistence.Entity;
import javax.persistence.FetchType;
import javax.persistence.Id;
import javax.persistence.OneToMany;

@Entity
public class Department {
    private static final Logger LOGGER = Logger.getLogger(Database.class.getName());
    @Id
    private Integer id;
    private String name;
    private final int buildingNumber;

    @OneToMany(mappedBy = "department", orphanRemoval = true)  // "department" should match the field name in Worker class
    private List<Worker> workers;

    // Default constructor for Hibernate
    public Department() {
        buildingNumber=0;
    }

    // Constructor with parameters
    public Department(Integer id, String name, int buildingNumber) {
        this.id = id;
        this.name = name;
        this.buildingNumber = buildingNumber;
        this.workers = new ArrayList<>();
    }

    public void addWorker(Worker worker) {
        workers.add(worker);
    }

    public void removeWorker(Worker worker) {
        workers.remove(worker);
    }

    public List<Worker> getWorkers() {
        return workers;
    }

    public String getName() {
        return name;
    }

    public Integer getId() { return id; }

    public void printWorkers() {
        LOGGER.info("Department: " + name);
        for (Worker worker : workers) {
            LOGGER.info("  - " + worker);
        }
    }

    @Override
    public String toString() {
        return "Department{name='" + name + "', totalWorkers=" + workers.size() + "}";
    }
}

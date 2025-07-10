package com.lab5.model;

import javax.persistence.CascadeType;
import javax.persistence.Entity;
import javax.persistence.Id;
import javax.persistence.ManyToOne;
import javax.persistence.criteria.CriteriaBuilder;

@Entity
public class Worker {
    @Id
    private final Integer id;
    private final String name;
    private final String surname;
    private final int experience;
    private final double salary;
    private final String position;

    @ManyToOne(cascade = CascadeType.REFRESH)
    private Department department;
    public Worker() {
        this.id = null;
        this.name = null;
        this.surname = null;
        this.position = null;
        this.salary = 0;
        this.experience = 0;
        this.department = null;
    }

    // Updated constructor
    public Worker(Integer id, String name, String surname, String position, double salary, int experience, Department department) {
        this.id = id;
        this.name = name;
        this.surname = surname;
        this.position = position;
        this.salary = salary;
        this.experience = experience;
        this.department = department;
    }

    @Override
    public boolean equals(Object object) {
        if (this == object) return true;
        if (object == null || getClass() != object.getClass()) return false;
        Worker worker = (Worker) object;
        return experience == worker.experience &&
                Double.compare(worker.salary, salary) == 0 &&
                position.equals(worker.position) &&
                surname.equals(worker.surname) &&
                name.equals(worker.name);
    }

    @Override
    public int hashCode() {
        return (31 * name.hashCode() + 67 * experience + 97 * (int) salary) * 2137;
    }

    @Override
    public String toString() {
        return "Worker{" +
                "position='" + position + '\'' +
                ", surname='" + surname + '\'' +
                ", name='" + name + '\'' +
                ", experience=" + experience +
                ", salary=" + salary +
                ", department=" + department.getName() +
                '}';
    }

    // Getters for name and department
    public String getName() {
        return name;
    }
    public Integer getId() { return id; }

    public Department getDepartment() {
        return department;
    }
}

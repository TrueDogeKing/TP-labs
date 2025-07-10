package com.lab6.model;

import lombok.Getter;
import lombok.Setter;


@Getter
@Setter
public class Worker {
    private String name;
    private String surname;
    private int experience;
    private double salary;
    private String position;

    public Worker(String name, String surname, String position, double salary, int experience) {
        this.name = name;
        this.surname = surname;
        this.position = position;
        this.salary = salary;
        this.experience = experience;
    }

    @Override
    public String toString() {
        return "Worker{" +
                "name='" + name + '\'' +
                ", surname='" + surname + '\'' +
                ", position='" + position + '\'' +
                ", salary=" + salary +
                ", experience=" + experience +
                '}';
    }

    @Override
    public boolean equals(Object obj) {
        if (obj == this) {
            return true;
        }
        if (!(obj instanceof Worker worker)) {
            return false;
        }
        return name.equals(worker.name)
                && surname.equals(worker.surname)
                && position.equals(worker.position)
                && Double.compare(worker.salary, salary) == 0
                && experience == worker.experience;
    }

    public Object getName() {
        return name;
    }
}



package org.example;

import java.util.HashSet;
import java.util.Set;
import java.util.TreeSet;

public class Worker implements Comparable<Worker> {
    final String name;
    final String surname;
    final int experience;
    final double salary;

    final String position;
    final Set<Worker> Employee;  // Recursive structure

    public Worker(String name, String surname,String position,double salary, int experience, boolean isSorted, WorkerComparator comparator) {
        this.name = name;
        this.surname=surname;
        this.position=position;
        this.salary = salary;
        this.experience = experience;

        // Initialize the set
        if (isSorted) {
            if (comparator != null) {
                this.Employee = new TreeSet<>(comparator);
            } else {
                this.Employee = new TreeSet<>();
            }
        } else {
            this.Employee = new HashSet<>();
        }
    }

    // Method to add related animals
    public void addRelatedWorker(Worker animal) {
        Employee.add(animal);
    }


    // equals (based on attack, health, and name)

    @Override
    public boolean equals(Object object) {
        if (this == object) {
            return true;
        }
        if (object == null || getClass() != object.getClass()) {
            return false;
        }
        Worker worker = (Worker) object;

        if (experience != worker.experience) {
            return false;
        }
        if (Double.compare(worker.salary, salary) != 0) {
            return false;
        }
        if (this.position.compareTo(worker.position) != 0)
        {
            return false;
        }
        if (this.surname.compareTo(worker.surname) != 0)
        {
            return false;
        }


        return name.equals(worker.name);
    }

    // hashCode (based on attack, health, and name)
    @Override
    public int hashCode() {
        return (31 * name.hashCode() + 67 * experience + 97 * (int) salary) * 2137;
    }

    // Compare by health first, then attack, then name
    @Override
    public int compareTo(Worker other) {

        if (this.surname.compareTo(other.surname) != 0)
        {
            return this.surname.compareTo(other.surname);
        }
        if (this.name.compareTo(other.name) != 0)
        {
            return this.name.compareTo(other.name);
        }

        if (this.position.compareTo(other.position) != 0)
        {
            return this.position.compareTo(other.position);
        }


        if (this.experience != other.experience) {
            return Integer.compare(this.experience,other.experience);
        }

        return Double.compare(this.salary, other.salary);


    }

    // toString for text representation
    @Override
    public String toString() {
        return "Worker{position='" + position + "', surname'"+surname + "', name'"+name +"', experience="+ experience + ", salary=" + salary + "}";
    }

    // Visualization of data (related animals)
    public void printStructure(Worker animal, String prefix) {
        System.out.println(prefix + animal);
        for (Worker related : animal.getEmplyees()) {
            printStructure(related, prefix + "  ");
        }
    }
    // Method to count all subordinate elements recursively
    public int countAllSubordinates() {
        int count = Employee.size();
        for (Worker related : Employee) {
            count += related.countAllSubordinates();
        }
        return count;
    }

    public String getName()
    {
        return this.name;
    }

    public Set<Worker> getEmplyees() {
        return Employee;
    }

}
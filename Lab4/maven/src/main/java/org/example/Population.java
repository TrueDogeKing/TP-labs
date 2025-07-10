package org.example;


public class Population {
    private final Person[] people;

    public Population(int size) {
        this.people = new Person[size];
    }

    public void setPerson(int index, Person person) {
        synchronized (this) {
            people[index] = person;
        }
    }

    // Thread-safe method to read a person
    public Person getPerson(int id) {
        if (id >= 0 && id < people.length) {
            return people[id];
        }
        return null;
    }

    // Thread-safe method to update a person
    public void updatePerson(int id, boolean isInfected, int infectionDays, boolean hasRecovered) {
        synchronized (this) {
            if (id >= 0 && id < people.length) {
                people[id].isInfected = isInfected;
                people[id].infectionDays = infectionDays;
                people[id].hasRecovered = hasRecovered;
            }
        }
    }

    public int getSize() {
        return people.length;
    }
}


package org.example;


public class Person  {
    int id;
    boolean isInfected;
    int infectionDays;
    int contactsPerDay;
    boolean hasRecovered;


    public Person(int id,boolean isInfected,int infectionDays,int contactsPerDay,boolean hasRecovered) {
        this.id=id;
        this.isInfected = isInfected;
        this.infectionDays = infectionDays;
        this.contactsPerDay=contactsPerDay;
        this.hasRecovered = hasRecovered;
    }

    // Default constructor
    public Person() {
        this.isInfected = false;
        this.infectionDays = 0;
        this.contactsPerDay = 0;
        this.hasRecovered = false;
    }

    // Getter and Setter methods (optional)
    public boolean isInfected() {
        return isInfected;
    }

    public void setInfected(boolean infected) {
        isInfected = infected;
    }

    public int getInfectionDays() {
        return infectionDays;
    }

    public void setInfectionDays(int infectionDays) {
        this.infectionDays = infectionDays;
    }

    public int getContactsPerDay() {
        return contactsPerDay;
    }

    public void setContactsPerDay(int contactsPerDay) {
        this.contactsPerDay = contactsPerDay;
    }

    public boolean isHasRecovered() {
        return hasRecovered;
    }

    public void setHasRecovered(boolean hasRecovered) {
        this.hasRecovered = hasRecovered;
    }

    // This method can simulate the daily infection status of a person.
    // You can update this logic depending on how you want to simulate the spread of the disease.
    public void updateInfectionStatus() {
        if (isInfected) {
            infectionDays++;
            if (infectionDays > 5) {  // Assuming recovery after 14 days of infection
                hasRecovered = true;
                isInfected = false;
            }
        }
    }

    // You can also add methods to simulate infection spread based on contacts if needed
    public boolean canBeInfect() {
        return !hasRecovered && !isInfected;  // Can only be infected if not already recovered or infected
    }


}
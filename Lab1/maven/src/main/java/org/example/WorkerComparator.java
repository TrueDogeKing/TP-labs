package org.example;


import java.util.Comparator;

public class WorkerComparator implements Comparator<Worker> {
    @Override
    public int compare(Worker worker1, Worker worker2) {

        // Compare by attack first (higher attack comes first)

        if (worker1.position.compareTo(worker2.position) != 0)
        {
            return worker1.position.compareTo(worker2.position);
        }


        if (worker1.experience != worker2.experience) {
            return Integer.compare(worker1.experience,worker2.experience);
        }

        if (worker1.surname.compareTo(worker2.surname) != 0)
        {
            return worker1.surname.compareTo(worker2.surname);
        }
        if (worker1.name.compareTo(worker2.name) != 0)
        {
            return worker1.name.compareTo(worker2.name);
        }


        // If both health and name are equal, compare by health (higher health comes first)
        return Double.compare(worker1.salary, worker2.salary);
    }
}
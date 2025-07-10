package com.lab6;

import com.lab6.controller.WorkerController;
import com.lab6.repository.WorkerRepository;

import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        WorkerRepository repository = new WorkerRepository();
        WorkerController controller = new WorkerController(repository);

        label:
        while (true) {
            String[] command = scanner.nextLine().split(" ");
            switch (command[0]) {
                case "find":
                    System.out.println(controller.find(command[1]));
                    break;
                case "delete":
                    System.out.println(controller.delete(command[1]));
                    break;
                case "save":
                    System.out.println(controller.save(command[1], command[2],command[3],Double.parseDouble(command[4]),Integer.parseInt(command[5])));
                    // String name, String surname, String position, double salary, int experience
                    break;
                case "exit":
                    break label;
            }
        }

        scanner.close();
    }
}

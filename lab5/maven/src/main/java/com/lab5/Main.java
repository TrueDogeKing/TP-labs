package com.lab5;

import java.util.Scanner;
import java.util.logging.Logger;

import com.lab5.model.Department;
import com.lab5.model.Worker;

public class Main
{
    public static void main( String[] args )
    {
        Database db = new Database();
        Logger LOGGER = Logger.getLogger(Database.class.getName());
        Scanner scanner = new Scanner(System.in);

        // Create departments
        Department department_1 = new Department(1, "Engineering",1);
        Worker john=new Worker(1, "John", "Doe", "Engineer", 60000, 5, department_1);
        Worker jane = new Worker(2, "Jane", "Smith", "Engineer", 70000, 6,  department_1);
        Worker mark = new Worker(3, "Mark", "Taylor", "Senior Engineer", 90000, 10, department_1);

        Department department_2 = new Department(2, "Human Resources",2);
        Worker alice = new Worker(4, "Alice", "Brown", "HR Manager", 80000, 8,  department_2);
        Worker bob = new Worker(5, "Bob", "White", "HR Assistant", 50000, 3, department_2);

        db.insertDepartment(department_1);
        db.insertDepartment(department_2);
        db.showData("Department", null);
        db.insertWorker(john);
        db.insertWorker(jane);
        db.insertWorker(alice);
        db.insertWorker(bob);

        db.insertWorker(mark);
        Integer idWorker = 5;
        Integer idDepartment=2;
        boolean running=true;
        while(running) {
            String choice = scanner.nextLine();
            switch (choice) {
                case "exit":
                    running=false;
                    break;
                case "insert":
                    LOGGER.info("enter worker/department:");
                    choice = scanner.nextLine();
                    switch (choice) {
                        case "worker":
                            idWorker++;
                            LOGGER.info("enter name:");
                            String name = scanner.nextLine();
                            LOGGER.info("enter surname:");
                            String surname = scanner.nextLine();
                            LOGGER.info("enter position:");
                            String position = scanner.nextLine();
                            LOGGER.info("enter salary:");
                            Integer salary = Integer.parseInt(scanner.nextLine().trim());
                            LOGGER.info("enter experience:");
                            Integer experience = Integer.parseInt(scanner.nextLine().trim());
                            LOGGER.info("enter department number:");
                            Integer workerNumber = Integer.parseInt(scanner.nextLine().trim());
                            Worker worker=new Worker(idWorker, name, surname, position, salary, experience, db.getDepartment(workerNumber));
                            db.insertWorker(worker);
                            break;

                        case "department":
                            idDepartment++;
                            LOGGER.info("enter name:");
                            String nameD = scanner.nextLine();
                            LOGGER.info("enter building number:");
                            Integer buildingNumber = Integer.parseInt(scanner.nextLine().trim());
                            Department departmentName=new Department(idDepartment,nameD,buildingNumber);
                            db.insertDepartment(departmentName);
                            break;
                    }
                    break;
                case "remove":
                    LOGGER.info("who to remove worker/department:");
                    choice = scanner.nextLine();
                    switch (choice) {
                        case "worker":
                            LOGGER.info("enter worker id:");
                            Integer workerNumber = Integer.parseInt(scanner.nextLine().trim());
                            db.removeWorker(db.getWorker(workerNumber));
                            break;
                        case "department":
                            LOGGER.info("enter department number:");
                            Integer departmentNumber = Integer.parseInt(scanner.nextLine().trim());
                            db.removeDepartmnent(db.getDepartment(departmentNumber));
                            break;
                    }
                    break;
                case "show":
                    LOGGER.info("enter data to show:");
                    String name = scanner.nextLine();
                    LOGGER.info("enter number:");
                    Integer number = Integer.parseInt(scanner.nextLine().trim());
                    db.showData(name, number);
                    break;
                case "5qr":
                    db.show_Queries();
                    break;
            }
        }



        db.dumpDatabase();
        db.shutdown();

    }
}

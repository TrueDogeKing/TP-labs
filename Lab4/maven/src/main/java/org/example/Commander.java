package org.example;



import static java.lang.Thread.sleep;

import java.util.Scanner;
import java.util.concurrent.locks.Condition;
import java.util.concurrent.locks.Lock;
import java.util.concurrent.locks.ReentrantLock;
import java.util.List;
import java.util.ArrayList;
import java.util.Random;

public class Commander implements Runnable {



    @Override
    public void run() {

        Scanner scanner = new Scanner(System.in);

        while(true){
            if (scanner.hasNextLine()) {
                String command = scanner.nextLine();
                if (command.equalsIgnoreCase("e")) {
                    break;
                }
            }
        }

        System.out.println("Dead");

    }



}

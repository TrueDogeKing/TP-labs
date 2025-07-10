package org.example.client;

import org.example.Main;
import org.example.SimulationResult;

import java.io.*;
import java.net.Socket;
import java.net.SocketException;
import java.util.Scanner;
import java.util.logging.Logger;

public class Client {
    private static final Logger LOGGER = Logger.getLogger(Client.class.getName());
    private static final String SERVER_ADDRESS = "localhost";
    private static final int SERVER_PORT = 8080;
    private static volatile boolean running = true;

    public static void main(String[] args) {
        try (Socket socket = new Socket(SERVER_ADDRESS, SERVER_PORT);
             ObjectOutputStream outputStream = new ObjectOutputStream(socket.getOutputStream());
             ObjectInputStream inputStream = new ObjectInputStream(socket.getInputStream());
             Scanner scanner = new Scanner(System.in)) {

            Runtime.getRuntime().addShutdownHook(new Thread(() -> {
                running = false;
                LOGGER.info("Client shutting down...");
            }));

            Object welcome = inputStream.readObject();
            if (welcome instanceof String msg) {
                LOGGER.info("Server: " + msg);
                LOGGER.info("Type one of the following commands: send_result | exit");
            }

            // Start a thread to listen for server messages
            Thread serverListener = new Thread(() -> {
                try {
                    while (running) {
                        Object serverMsg = inputStream.readObject();
                        if (serverMsg instanceof String msg) {
                            if ("SERVER_SHUTDOWN".equals(msg)) {
                                LOGGER.info("Server is shutting down. Disconnecting...");
                                running = false;
                                System.exit(0);
                            }
                            LOGGER.info("Server: " + msg);
                        }
                    }
                } catch (EOFException e) {
                    if (running) {
                        LOGGER.info("Server closed the connection");
                        System.exit(0);
                    }
                } catch (SocketException e) {
                    if (running) {
                        LOGGER.info("Connection to server lost");
                        System.exit(0);
                    }
                } catch (Exception e) {
                    if (running) {
                        LOGGER.warning("Error reading from server: " + e.getMessage());
                    }
                }
            });
            serverListener.start();

            while (running) {
                try {
                    if (System.in.available() > 0 || scanner.hasNextLine()) {
                        String input = scanner.nextLine();
                        handleUserInput(input, outputStream);
                    }
                } catch (IOException e) {
                    if (running) {
                        LOGGER.warning("Input error: " + e.getMessage());
                    }
                }
            }

        } catch (Exception e) {
            LOGGER.severe("Failed to connect to server: " + e.getMessage());
        } finally {
            LOGGER.info("Client terminated");
            System.exit(0);
        }
    }

    private static void handleUserInput(String input, ObjectOutputStream outputStream) throws IOException {
        switch (input.toLowerCase()) {
            case "exit":
                outputStream.writeObject("exit");
                outputStream.flush();
                running = false;
                break;

            case "send_result":
                sendSimulationResult(outputStream);
                break;
            case "calculate":
                Main main=new Main();
                String[] threadsNumber={"2"};
                main.main(threadsNumber);
                break;

            default:
                outputStream.writeObject(input);
                outputStream.flush();
                break;
        }
    }

    private static void sendSimulationResult(ObjectOutputStream outputStream) {
        try (FileInputStream fis = new FileInputStream("result.ser");
             ObjectInputStream fileInput = new ObjectInputStream(fis)) {

            SimulationResult result = (SimulationResult) fileInput.readObject();
            LOGGER.info("Read from file: " + result);
            outputStream.writeObject(result);
            outputStream.flush();
            LOGGER.info("Result sent to server");

        } catch (FileNotFoundException e) {
            LOGGER.warning("Result file not found");
        } catch (Exception e) {
            LOGGER.warning("Error sending result: " + e.getMessage());
        }
    }
}
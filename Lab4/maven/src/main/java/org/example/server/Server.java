package org.example.server;

import java.io.IOException;
import java.net.ServerSocket;
import java.net.Socket;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Scanner;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.logging.Logger;

public class Server {
    private static final Logger LOGGER = Logger.getLogger(Server.class.getName());
    private static final LinkedList<Thread> threads = new LinkedList<>();
    private static final AtomicBoolean shouldStop = new AtomicBoolean(false);
    private static final LinkedList<ServerThread> serverThreads = new LinkedList<>();

    public static synchronized void addServerThread(ServerThread thread) {
        serverThreads.add(thread);
    }

    public static synchronized void removeServerThread(ServerThread thread) {
        serverThreads.remove(thread);
    }

    public static synchronized List<ServerThread> getServerThreads() {
        return new ArrayList<>(serverThreads); // Return a copy for thread safety
    }

    public static void main(String[] args) {
        ServerSocket serverSocket = null;
        Scanner scanner1 = new Scanner(System.in);

        // Thread to listen for exit command
        new Thread(() -> {
            Scanner scanner = new Scanner(System.in);
            while (!shouldStop.get()) {
                if (scanner.hasNextLine()) {
                    String line = scanner.nextLine();
                    if (line.equalsIgnoreCase("exit")) {
                        shouldStop.set(true);
                        LOGGER.info("Exit command received. Stopping server...");
                        synchronized (serverThreads) {
                            for (ServerThread thread : serverThreads) {
                                thread.shutdown();
                            }
                        }
                        break;
                    }
                }
            }
        }).start();

        try {
            serverSocket = new ServerSocket(8080);
            serverSocket.setSoTimeout(1000);
            LOGGER.info("Server started. Input 'exit' to stop the server.");
            LOGGER.info("Server is listening on port 8080.");
            LOGGER.info("Server is waiting for a client to connect...");

            while (!shouldStop.get()) {
                try {
                    Socket socket = serverSocket.accept();
                    LOGGER.info("Client connected: " + socket.getInetAddress());
                    ServerThread serverThread = new ServerThread(socket);
                    Thread thread = new Thread(serverThread);
                    thread.start();
                    threads.add(thread);
                    synchronized (serverThreads) {
                        serverThreads.add(serverThread);
                    }
                } catch (Exception e) {
                    // Expected when timeout occurs while waiting for accept
                }
            }
        } catch (IOException e) {
            e.printStackTrace();
        } finally {
            LOGGER.info("Server is shutting down...");
            LOGGER.info("Closing all client connections...");


            for (Thread thread : threads) {
                thread.interrupt();
            }

            for (Thread thread : threads) {
                try {
                    thread.join(1000);
                } catch (InterruptedException e) {
                    e.printStackTrace();
                }
            }

            LOGGER.info("All client connections closed.");
            LOGGER.info("Closing server socket...");
            if (serverSocket != null) {
                try {
                    serverSocket.close();
                } catch (IOException e) {
                    e.printStackTrace();
                }
            }
            LOGGER.info("Server socket closed.");
            LOGGER.info("Server stopped.");
        }
    }
}
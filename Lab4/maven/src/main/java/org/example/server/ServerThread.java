package org.example.server;

import org.example.SimulationResult;

import java.io.EOFException;
import java.io.IOException;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.net.Socket;
import java.net.SocketException;
import java.util.logging.Logger;

class ServerThread implements Runnable {
    private static final Logger LOGGER = Logger.getLogger(ServerThread.class.getName());
    private final Socket socket;
    private volatile boolean running = true;
    private ObjectOutputStream outputStream;
    private ObjectInputStream inputStream;

    public ServerThread(Socket socket) {
        this.socket = socket;
    }

    @Override
    public void run() {
        Server.addServerThread(this);

        try {
            outputStream = new ObjectOutputStream(socket.getOutputStream());
            inputStream = new ObjectInputStream(socket.getInputStream());

            sendMessage("READY");

            while (running && !Thread.currentThread().isInterrupted()) {
                try {
                    Object received = inputStream.readObject();

                    if (received instanceof String message) {
                        handleStringMessage(message);
                    } else if (received instanceof SimulationResult result) {
                        handleSimulationResult(result);
                    }
                } catch (EOFException e) {
                    LOGGER.info("Client disconnected normally");
                    break;
                } catch (SocketException e) {
                    if (running) {
                        LOGGER.warning("Socket error: " + e.getMessage());
                    }
                    break;
                }
            }
        } catch (IOException | ClassNotFoundException e) {
            if (running) {
                LOGGER.warning("Communication error: " + e.getMessage());
            }
        } finally {
            cleanUp();
        }
    }

    private void handleStringMessage(String message) throws IOException {
        if(message=="Request DATA"){

        }
        LOGGER.info("Received message from client: " + message);

        if ("exit".equalsIgnoreCase(message)) {
            sendMessage("GOODBYE");
            running = false;
        } else {
            sendMessage("Message received: " + message);
        }
    }

    private void handleSimulationResult(SimulationResult result) throws IOException {

        LOGGER.info("Received simulation result from client: " + result);
        sendMessage("Result received. Thank you!\n" + result);
    }

    private void sendMessage(String message) throws IOException {
        if (!socket.isClosed()) {
            outputStream.writeObject(message);
            outputStream.flush();
        }
    }

    public void shutdown() {
        running = false;
        try {
            sendMessage("SERVER_SHUTDOWN");
            socket.close();
        } catch (IOException e) {
            LOGGER.warning("Error during shutdown: " + e.getMessage());
        }
    }

    private void cleanUp() {
        Server.removeServerThread(this);
        try {
            if (inputStream != null) inputStream.close();
        } catch (IOException e) {
            LOGGER.warning("Error closing input stream: " + e.getMessage());
        }

        try {
            if (outputStream != null) outputStream.close();
        } catch (IOException e) {
            LOGGER.warning("Error closing output stream: " + e.getMessage());
        }

        try {
            if (!socket.isClosed()) socket.close();
        } catch (IOException e) {
            LOGGER.warning("Error closing socket: " + e.getMessage());
        }

        LOGGER.info("Client connection handler shut down");
    }
}
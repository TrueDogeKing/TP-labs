package com.lab6.controller;

import com.lab6.model.Worker;
import com.lab6.repository.WorkerRepository;

import java.util.Optional;

public class WorkerController {
    private final WorkerRepository repository;
    public WorkerController(WorkerRepository repository) {
        this.repository = repository;
    }
    public String find(String name) {
        Optional<Worker> worker = repository.find(name);
        if (worker.isPresent()) {
            return worker.get().toString();
        } else {
            return WorkerControllerStatus.NOT_FOUND.toString();
        }
    }
    public String delete(String name) {
        try {
            repository.delete(name);
            return WorkerControllerStatus.DONE.toString();
        } catch (IllegalArgumentException e) {
            return WorkerControllerStatus.NOT_FOUND.toString();
        }
    }
    public String save(String name, String surname, String position, double salary, int experience) {
        try {
            repository.save(new Worker( name,  surname,  position,  salary,  experience));
            return WorkerControllerStatus.DONE.toString();
        } catch (IllegalArgumentException e) {
            return WorkerControllerStatus.BAD_REQUEST.toString();
        }
    }
}

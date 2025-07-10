package com.lab6.controller;

import com.lab6.model.Worker;
import com.lab6.repository.WorkerRepository;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import java.util.Optional;

import static org.assertj.core.api.Assertions.assertThat;
import static org.mockito.Mockito.doThrow;
import static org.mockito.Mockito.when;

public class WorkerControllerTest {

    private WorkerController workerController;

    @Mock
    private WorkerRepository mockedRepository;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
        workerController = new WorkerController(mockedRepository);
    }

    //TODO tests for all edge cases
    @Test
    void testDelete_MageDeleted() { // delete existing worker
        String name = "john";
        String result = workerController.delete(name);

        assertThat(result).isEqualTo("done");
    }
    @Test
    void testDelete_nonexistent() { // delete not existing worker
        String name = "jan";
        doThrow(new IllegalArgumentException()).when(mockedRepository).delete(name);

        String result = workerController.delete(name);

        assertThat(result).isEqualTo("not found");
    }
    @Test
    void testFind_existent() { // find existing worker
        String name = "john";
        Worker worker = new Worker(name, "kowalski", "Engineer", 10, 20);
        when(mockedRepository.find(name)).thenReturn(Optional.of(worker));
        String result = workerController.find(name);
        assertThat(result).isEqualTo(worker.toString());

    }
    @Test
    void testFind_nonexistent() { // find not existing worker
        String name = "jan";
        when(mockedRepository.find(name)).thenReturn(Optional.empty());
        String result = workerController.find(name);
        assertThat(result).isEqualTo("not found");
    }
    @Test
    void testSave_returnDone() {  // save worker successfully
        String result = workerController.save("Alice", "Smith", "Manager", 70, 80);
        assertThat(result).isEqualTo("done");
    }
    @Test
    void testSave_badRequest() { // save worker failed
        Worker duplicate = new Worker("Bob", "Smith", "Analyst", 6, 4);
        doThrow(new IllegalArgumentException()).when(mockedRepository).save(duplicate);

        String result = workerController.save("Bob", "Smith", "Analyst", 6, 4);

        assertThat(result).isEqualTo("bad request");
    }

}

package com.lab6.repository;

import com.lab6.model.Worker;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.BeforeEach;

import java.util.Optional;

import static org.assertj.core.api.Assertions.assertThatThrownBy;
import static org.assertj.core.api.Assertions.assertThat;

public class WorkerRepositoryTest {

    private WorkerRepository workerRepository;

    @BeforeEach
    void setUp() {
        workerRepository = new WorkerRepository();
    }

    //TODO test all edge cases for Repository

    @Test
    void testDelete_NotFound() {
        assertThatThrownBy(() -> workerRepository.delete("empty"))
                .isInstanceOf(IllegalArgumentException.class);
    }

    @Test
    void testFind_NotFound() {
        Optional<Worker> result = workerRepository.find("empty");
        assertThat(result).isEmpty();
    }

    @Test
    void testFind_Found() {
        Worker worker = new Worker("Anna", "Nowak", "Manager", 7000.0, 8);
        workerRepository.save(worker);
        Optional<Worker> result = workerRepository.find("Anna");
        assertThat(result).isPresent();
        assertThat(result.get()).isEqualTo(worker);
    }


    @Test
    void testSave_keyExist(){
        Worker worker1 = new Worker("Jan","Kowalski","Senior",7450,5);
        Worker worker2 = new Worker("Jan","Nowacki","Middle",5900,3);
        workerRepository.save(worker1);
        assertThatThrownBy(() -> workerRepository.save(worker2))
                .isInstanceOf(IllegalArgumentException.class);
    }
}

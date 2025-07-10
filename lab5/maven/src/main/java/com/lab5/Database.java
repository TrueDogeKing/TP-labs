package com.lab5;

import java.util.List;
import java.util.logging.Logger;
import javax.persistence.EntityManager;
import javax.persistence.EntityManagerFactory;
import javax.persistence.EntityTransaction;
import javax.persistence.Persistence;
import javax.persistence.Query;

import com.lab5.model.Department;
import com.lab5.model.Worker;

public class Database {
    private static final Logger LOGGER = Logger.getLogger(Database.class.getName());
    private static final String PERSISTENCE_UNIT_NAME = "PersistenceUnit";
    private static EntityManagerFactory entityManagerFactory;

    public Database() {
        entityManagerFactory = Persistence.createEntityManagerFactory(PERSISTENCE_UNIT_NAME);
    }

    // Insert a Worker
    public void insertWorker(Worker worker) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        EntityTransaction transaction = entityManager.getTransaction();
        transaction.begin();
        entityManager.persist(worker);
        transaction.commit();
        LOGGER.info("Worker inserted successfully: " + worker.getName());
        entityManager.close();
    }

    // Insert a Department
    public void insertDepartment(Department department) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        EntityTransaction transaction = entityManager.getTransaction();
        transaction.begin();
        entityManager.persist(department);
        transaction.commit();
        LOGGER.info("Department inserted successfully: " + department.getName());
        entityManager.close();
    }

    // Update a Worker
    public void updateWorker(Worker worker) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        EntityTransaction transaction = entityManager.getTransaction();
        transaction.begin();
        entityManager.merge(worker);
        transaction.commit();
        LOGGER.info("Worker updated successfully: " + worker.getName());
        entityManager.close();
    }
    public void removeWorker(Worker worker) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        EntityTransaction transaction = entityManager.getTransaction();
        transaction.begin();
        Worker managedWorker = entityManager.find(Worker.class, worker.getId());
        entityManager.remove(managedWorker);
        transaction.commit();
        LOGGER.info("Worker removed successfully: " + worker.getName());
        entityManager.close();
    }

    // Update a Department
    public void updateDepartment(Department department) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        EntityTransaction transaction = entityManager.getTransaction();
        transaction.begin();
        entityManager.merge(department);
        transaction.commit();
        LOGGER.info("Department updated successfully: " + department.getName());
        entityManager.close();
    }

    public void removeDepartmnent(Department department) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        EntityTransaction transaction = entityManager.getTransaction();
        transaction.begin();
        Department managedDepartment = entityManager.find(Department.class, department.getId());
        LOGGER.info("Workers removed from department");
        managedDepartment.printWorkers();
        managedDepartment.getWorkers().clear();
        LOGGER.info("Removed all workers from department: " + department.getName());

        entityManager.remove(managedDepartment);
        transaction.commit();
        LOGGER.info(("Department removed successfully: " + department.getName()));
        entityManager.close();

    }

    public void show_Queries(){

            EntityManager em = entityManagerFactory.createEntityManager();

            LOGGER.info("\n 1. Workers with experience > 5 years:");
            List<Worker> experienced = em.createQuery(
                            "SELECT w FROM Worker w WHERE w.experience > 5", Worker.class)
                    .getResultList();
            for (Object obj : experienced)
            {
                LOGGER.info(obj.toString());
            }

            LOGGER.info("\n 2. First 3 workers with the highest salary:");
            List<Worker> topSalaries = em.createQuery(
                            "SELECT w FROM Worker w ORDER BY w.salary DESC", Worker.class)
                    .setMaxResults(3)
                    .getResultList();
            for (Object obj : topSalaries)
            {
                LOGGER.info(obj.toString());
            }

            LOGGER.info("\n 3. Workers from the Human Resources department:");
            List<Worker> hrWorkers = em.createQuery(
                            "SELECT w FROM Worker w WHERE w.department.name = 'Human Resources'", Worker.class)
                    .getResultList();
            for (Object obj : hrWorkers)
            {
                LOGGER.info(obj.toString());
            }

            LOGGER.info("\n 4. Workers with surname starting with 'S':");
            List<Worker> surnameS = em.createQuery(
                            "SELECT w FROM Worker w WHERE w.surname LIKE 'S%'", Worker.class)
                    .getResultList();
            for (Object obj : surnameS)
            {
                LOGGER.info(obj.toString());
            }


            LOGGER.info("\n 5. Average salary in Engineering department:");
            Double avgSalary = em.createQuery(
                            "SELECT AVG(w.salary) FROM Worker w WHERE w.department.name = 'Engineering'", Double.class)
                    .getSingleResult();
            LOGGER.info("Average salary: " + avgSalary);

            em.close();
    }

    // Get a Worker by name
    public Worker getWorker(Integer id) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        Worker worker = entityManager.find(Worker.class, id);
        entityManager.close();
        return worker;
    }

    // Get a Department by name
    public Department getDepartment(Integer id) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        Department department = entityManager.find(Department.class, id);
        entityManager.close();
        return department;
    }





    // Clear the database (remove all Workers and Departments)
    public void clearDatabase() {
        EntityManager entityManager = entityManagerFactory.createEntityManager();
        entityManager.getTransaction().begin();

        // Clear Workers
        Query workerClearQuery = entityManager.createQuery("DELETE FROM Worker");
        workerClearQuery.executeUpdate();

        // Clear Departments
        Query departmentClearQuery = entityManager.createQuery("DELETE FROM Department");
        departmentClearQuery.executeUpdate();

        entityManager.getTransaction().commit();
        entityManager.close();

        LOGGER.info("Database cleared.");
    }
    public void showData(String entity, Integer number) {
        EntityManager entityManager = entityManagerFactory.createEntityManager();

        String jpql = "SELECT e FROM " + entity + " e";
        Query query = entityManager.createQuery(jpql);
        if (number != null && number > 0) {
            query.setMaxResults(number);
        }
        List<?> results = query.getResultList();

        LOGGER.info("Results from: " + entity);
        for (Object obj : results) {
            LOGGER.info(obj.toString());
        }

        entityManager.close();
    }


    // Dump all Workers and Departments from the database
    public void dumpDatabase() {
        EntityManager entityManager = entityManagerFactory.createEntityManager();

        // Dump Workers
        Query workerQuery = entityManager.createQuery("SELECT w FROM Worker w");
        List<Worker> workers = workerQuery.getResultList();
        LOGGER.info("Dumping Workers from the database:");
        for (Worker worker : workers) {
            LOGGER.info(worker.toString());
        }

        // Dump Departments
        Query departmentQuery = entityManager.createQuery("SELECT d FROM Department d");
        List<Department> departments = departmentQuery.getResultList();
        LOGGER.info("Dumping Departments from the database:");
        for (Department department : departments) {
            LOGGER.info(department.toString());
        }

        entityManager.close();
    }


    // Shutdown the EntityManagerFactory
    public void shutdown() {
        if (entityManagerFactory != null && entityManagerFactory.isOpen()) {
            entityManagerFactory.close();
            LOGGER.info("EntityManagerFactory closed successfully.");
        }
    }
}

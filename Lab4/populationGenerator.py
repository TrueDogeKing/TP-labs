# Python script to generate a population.txt file with exactly 10 initially infected individuals

import random

def generate_population_with_initial_infected(filename, num_people, num_initially_infected):
    with open(filename, 'w') as file:
        # Write the population size on the first line
        file.write(f"{num_people}\n")
        
        # Create a list of people where 10 are initially infected
        infected_ids = random.sample(range(1, num_people+1), num_initially_infected)
        
        # Generate each person's data and write to the file
        for i in range(num_people):
            is_infected = (i+1) in infected_ids
            infection_days = random.randint(1, 5) if is_infected else 0  # Infected people will have between 1 to 5 infection days initially
            contacts_per_day = random.randint(5, 15)
            has_recovered = is_infected and random.random() < 0.0001  # 30% chance of recovery
            
            # Write the data in the specified format: id,isInfected,infectionDays,contactsPerDay,hasRecovered
            file.write(f"{i+1},{is_infected},{infection_days},{contacts_per_day},{has_recovered}\n")

# Example: generate a population.txt file with 1000 people, with 10 initially infected
generate_population_with_initial_infected('population.txt', 100000, 10)

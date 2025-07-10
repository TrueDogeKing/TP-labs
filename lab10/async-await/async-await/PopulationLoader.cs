using System.IO;

public static class PopulationLoader
{
    public static void LoadPopulationFromFile(string filename, Population population)
    {
        var lines = File.ReadAllLines(filename);
        for (int i = 1; i < lines.Length; i++)
        {
            var data = lines[i].Split(',');
            int id = int.Parse(data[0].Trim());
            bool isInfected = bool.Parse(data[1].Trim());
            int infectionDays = int.Parse(data[2].Trim());
            int contactsPerDay = int.Parse(data[3].Trim());
            bool hasRecovered = bool.Parse(data[4].Trim());

            var person = new Person(id, isInfected, infectionDays, contactsPerDay, hasRecovered);
            population.SetPerson(id - 1, person);
        }
    }
}

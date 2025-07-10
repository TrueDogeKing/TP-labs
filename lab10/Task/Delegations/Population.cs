public class Population
{
    private readonly Person[] _people;

    public Population(int size)
    {
        _people = new Person[size];
    }

    public void SetPerson(int index, Person person)
    {
        lock (this)
        {
            _people[index] = person;
        }
    }

    public Person GetPerson(int id)
    {
        return (id >= 0 && id < _people.Length) ? _people[id] : null;
    }

    public void UpdatePerson(int id, bool isInfected, int infectionDays, bool hasRecovered)
    {
        lock (this)
        {
            if (id >= 0 && id < _people.Length)
            {
                _people[id].IsInfected = isInfected;
                _people[id].InfectionDays = infectionDays;
                _people[id].HasRecovered = hasRecovered;
            }
        }
    }

    public int GetSize()
    {
        return _people.Length;
    }
}

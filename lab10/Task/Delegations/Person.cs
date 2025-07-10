public class Person
{
    public int Id { get; set; }
    public bool IsInfected { get; set; }
    public int InfectionDays { get; set; }
    public int ContactsPerDay { get; set; }
    public bool HasRecovered { get; set; }

    public Person(int id, bool isInfected, int infectionDays, int contactsPerDay, bool hasRecovered)
    {
        Id = id;
        IsInfected = isInfected;
        InfectionDays = infectionDays;
        ContactsPerDay = contactsPerDay;
        HasRecovered = hasRecovered;
    }

    public Person() : this(0, false, 0, 0, false) { }

    public void UpdateInfectionStatus()
    {
        if (IsInfected)
        {
            InfectionDays++;
            if (InfectionDays > 5)
            {
                HasRecovered = true;
                IsInfected = false;
            }
        }
    }

    public bool CanBeInfected()
    {
        return !HasRecovered && !IsInfected;
    }

    public int GetContactsPerDay() {  return ContactsPerDay; }

    public int GetId() {  return Id; }
    public bool GetIsInfected() { return IsInfected; }
}

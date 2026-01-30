using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

public class Person
{
    public string name;
    public Country country;

    public Person(string name, Country country)
    {
        this.name = name;
        this.country = country;
    }
}

[System.Serializable]
public class Country
{
    [ToggleGroup("isActive", "$countryName")] public bool isActive = false;
    [ToggleGroup("isActive", "$countryName")] public string countryName;
    [ToggleGroup("isActive", "$countryName"), PreviewField(60f)] public Sprite flag;
    [ToggleGroup("isActive", "$countryName")] public string[] names;
}

[System.Serializable]
public class Continent
{
    [ToggleGroup("isActive", "$continentName")] public bool isActive = true;
    [ToggleGroup("isActive", "$continentName")] public string continentName;
    [ToggleGroup("isActive", "$continentName")] public List<Country> countries = new List<Country>();
}

public class Globalizer : ScriptableObject
{
    [SerializeField] private List<Continent> continents = new List<Continent>();

    public List<Person> GetRandomPersons(int count)
    {
        List<Person> result = new List<Person>();
        List<Continent> continentsTemp = new List<Continent>(continents);

        List<Country> countries = new List<Country>();
        for(int i = 0; i < continentsTemp.Count; i++)
        {
            if (continentsTemp[i].isActive)
            {
                countries.AddRange(continentsTemp[i].countries.FindAll((x) => x.isActive == true));
            }
        }

        for (int i = 0; i < count; i++)
        {
            int countryID = Random.Range(0, countries.Count);
            int nameID = Random.Range(0, countries[countryID].names.Length);
            Person newPerson = new Person(countries[countryID].names[nameID], countries[countryID]);

            result.Add(newPerson);
            countries.RemoveAt(countryID);
        }

        return result;
    }

    public Person GetRandomPerson()
    {
        List<Continent> continentsTemp = new List<Continent>(continents);
        List<Country> countries = new List<Country>();
        for(int i = 0; i < continentsTemp.Count; i++)
        {
            if (continentsTemp[i].isActive)
            {
                countries.AddRange(continentsTemp[i].countries.FindAll((x) => x.isActive == true));
            }
        }

        int countryID = Random.Range(0, countries.Count);
        int nameID = Random.Range(0, countries[countryID].names.Length);
        Person newPerson = new Person(countries[countryID].names[nameID], countries[countryID]);

        return newPerson;
    }
}

using UnityEngine;

public class TavernManager : MonoBehaviour
{
    public int tavernLevel = 1;
    
    public Character GetRandomVolunteer() 
    {
        ClassType randomClass = GetRandomClass();
        Weapon randomWeapon = GetRandomWeapon();
        string randomName = GetRandomName(randomClass);

        Character volunteer = new Character(randomName,randomClass,randomWeapon);
        return volunteer;
    }
    
    public ClassType GetRandomClass()
    {
        int count = System.Enum.GetValues(typeof(ClassType)).Length;
        return (ClassType) Random.Range(0,count);
    }

    private WeaponType GetRandomWeaponType()
    {
        int count= System.Enum.GetValues(typeof(WeaponType)).Length;
        return (WeaponType) Random.Range(0,count);
    }

    private Weapon GetRandomWeapon()
    { 
        WeaponType randomWeaponType= GetRandomWeaponType();
        switch (randomWeaponType)
        {
            case WeaponType.Sword:
                return new Weapon("Basic Sword", WeaponType.Sword, 5, 15, 1);
            case WeaponType.Axe:
                return new Weapon("Basic Axe", WeaponType.Axe, 4, 14, 1);
            case WeaponType.Bow:
                return new Weapon("Basic Bow", WeaponType.Bow, 4, 12, 3);
            case WeaponType.Crossbow:
                return new Weapon("Basic Crossbow", WeaponType.Crossbow, 6, 14, 3);
            case WeaponType.Staff:
                return new Weapon("Basic Staff", WeaponType.Staff, 4, 10, 2);
            case WeaponType.Wand:
                return new Weapon("Basic Wand", WeaponType.Wand, 2, 8, 2);
            default:
                Debug.LogError("Unknown weapon type!");
                return new Weapon("Broken Stick", WeaponType.Wand, 1, 3, 1);
        }
    }
    
    private string GetRandomName(ClassType classType)
    {
        switch (classType)
        {
            case ClassType.Warrior:
                string[] warriors =
                {
                "Borin", "Kael", "Durgan", "Thorin", "Ragnar",
                "Garrik", "Ulric", "Magnus", "Hector", "Brakus",
                "Valen", "Drogan", "Tarkon", "Roderik", "Baldric"
            };
                return warriors[Random.Range(0, warriors.Length)];

            case ClassType.Archer:
                string[] archers =
                {
                "Lira", "Sera", "Nessa", "Elira", "Faelyn",
                "Sylra", "Kara", "Nyra", "Aeris", "Selene",
                "Talia", "Vera", "Rina", "Lyra", "Arwen"
            };
                return archers[Random.Range(0, archers.Length)];

            case ClassType.Mage:
                string[] mages =
                {
                "Eldrin", "Mira", "Vael", "Altheron", "Zerath",
                "Lyrius", "Velora", "Orin", "Seraphis", "Malver",
                "Ilyra", "Azriel", "Thalor", "Varyn", "Celindra"
            };
                return mages[Random.Range(0, mages.Length)];
        }

        return "Volunteer";
    }
}
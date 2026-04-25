namespace Infrastructure.Database.Seed;

public class SeedContext
{
    public Guid AdminId { get; set; }

    public Guid Arena_1 { get; set; }
    public Guid Arena_2 { get; set; }

    public Guid Orc_Warrior { get; set; }

    public Guid Class_Warrior { get; set; }
    public Guid Class_Mage { get; set; }

    public Guid Spec_Warrior_Berserker { get; set; }
    public Guid Spec_Warrior_Guardian { get; set; }
    public Guid Spec_Warrior_Duelist { get; set; }

    public Guid Spec_Mage_Pyromancer { get; set; }
    public Guid Spec_Mage_Luminary { get; set; }
    public Guid Spec_Mage_Arcanist { get; set; }
}

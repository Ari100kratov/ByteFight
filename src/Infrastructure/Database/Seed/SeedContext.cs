namespace Infrastructure.Database.Seed;

public class SeedContext
{
    public Guid AdminId { get; set; }

    // Арены
    public Guid ForestEdge_Arena { get; set; }
    public Guid Graveyard_Arena { get; set; }
    public Guid Swamp_Arena { get; set; }

    // Враги
    public Guid Ghoul { get; set; }
    public Guid Leshy { get; set; }
    public Guid Kikimora { get; set; }
    public Guid SkeletonWarrior { get; set; }
    public Guid Volkolak { get; set; }

    // Классы
    public Guid Class_Vityaz { get; set; }
    public Guid Class_Volkhv { get; set; }
    public Guid Class_Okhotnik { get; set; }
    public Guid Class_Vedunya { get; set; }

    // Стези витязя
    public Guid Spec_Vityaz_SwordBearer { get; set; }
    public Guid Spec_Vityaz_ShieldBearer { get; set; }
    public Guid Spec_Vityaz_Druzhinnik { get; set; }

    // Стези волхва
    public Guid Spec_Volkhv_StormCaller { get; set; }
    public Guid Spec_Volkhv_WardKeeper { get; set; }
    public Guid Spec_Volkhv_Veden { get; set; }

    // Стези охотника
    public Guid Spec_Okhotnik_Archer { get; set; }
    public Guid Spec_Okhotnik_Trappers { get; set; }
    public Guid Spec_Okhotnik_BeastStalker { get; set; }

    // Стези ведуньи
    public Guid Spec_Vedunya_Herbalist { get; set; }
    public Guid Spec_Vedunya_Whisperer { get; set; }
    public Guid Spec_Vedunya_Sorceress { get; set; }
}

using NinjaDomain.Classes;
using System.Data.Entity;

namespace NinjaDomain.DataModel
{
    public class NinjaContext: DbContext
    {
        DbSet<Ninja> Ninjas { get; set; }
        DbSet<NinjaEquipment> Equipment { get; set; }
        DbSet<Clan> Clans{ get; set; }
    }
}

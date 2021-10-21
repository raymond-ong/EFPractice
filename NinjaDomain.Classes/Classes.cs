using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace NinjaDomain.Classes
{
    public class Ninja
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool ServedInOniwaban { get; set; }
        public Clan Clan { get; set; }
        public int ClanId { get; set; } // Notice this is FK for Clan. Recommended to add this "extra" FK ID to help EF identify this is a required field
        public List<NinjaEquipment> EquipmentOwned { get; set; } 
        public DateTime DateOfBirth { get; set; }
    }

    public class Clan
    {
        public int Id { get; set; }
        public string ClanName { get; set; }
        public List<Ninja> Ninjas{ get; set; }
    }

    public class NinjaEquipment
    {
        public int Id{ get; set; }

        public string Name { get; set; }

        public EquipmentType Type { get; set; }


        // Notice there is no NinjaId FK compared to Clan ID so EF will assume it's ok that this Equipment not necessarily owned by any Ninja
        // But our intention is that all equipment should belong to a ninja.
        // One way to solve it is by adding a Required attribute
        [Required]
        public Ninja Ninja { get; set; }
    }
}

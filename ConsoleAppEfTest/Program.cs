using NinjaDomain.Classes;
using NinjaDomain.DataModel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppEfTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ConsoleAppEfTest start...");
            Database.SetInitializer(new NullDatabaseInitializer<NinjaContext>());
            //InsertNinja();
            //QueryNinjas();
            //UpdateNinja();
            //UpdateNinjaDisconnected();
            //InsertNinjaWithEquipment();
            //QueryNinjasEager();
            //QueryNinjasEagerSpecific();
            QueryNinjasProjection();
            Console.WriteLine("ConsoleAppEfTest end...");
            Console.ReadLine();
        }

        public static void InsertNinja()
        {
            Console.WriteLine("Inserting new ninja...");
            Ninja newNinja = new Ninja()
            {
                ClanId = 1,
                DateOfBirth = new DateTime(2000, 1, 1),
                ServedInOniwaban = false,
                Name = "Charlie"
            };

            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Add(newNinja);
                context.SaveChanges();
            }
        }

        public static void InsertNinjaWithEquipment()
        {
            Console.WriteLine("Inserting new ninja...");
            Ninja newNinja = new Ninja()
            {
                ClanId = 1,
                DateOfBirth = new DateTime(2000, 1, 1),
                ServedInOniwaban = false,
                Name = "David"
            };

            NinjaEquipment equip1 = new NinjaEquipment()
            {
                Name = "Sword",
                Type = EquipmentType.Weapon
            };
            NinjaEquipment equip2 = new NinjaEquipment()
            {
                Name = "Gloves",
                Type = EquipmentType.Tool
            };
            
            newNinja.EquipmentOwned.Add(equip1);
            newNinja.EquipmentOwned.Add(equip2);

            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Add(newNinja);
                context.SaveChanges();
            }
        }

        public static void QueryNinjas()
        {
            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas.ToList();
                foreach(var ninja in ninjas)
                {
                    //Console.WriteLine($"{ninja.Name}, {ninja.Clan}"); // was not able to determine the Clan given the Clan ID
                    Console.WriteLine($"{ninja.Name}, {ninja.Clan.ClanName}"); // After we change the declaration of Clan to be virtual, it is now able to load it without explcitly loading it
                }

                // BAD: Will hold the database connection while iterating through the list
                // context.Ninjas is also resonsible for query execution; do not perform lots of work in it.
                // Adding context.Database.Log = Console.WriteLine;
                // You will notice that above, the SQL connection was closed after ToList();
                // Below, the SQL Connection was ony closed after the iteration
                foreach (var ninja in context.Ninjas)
                {
                    Console.WriteLine(ninja.Name);
                }
            }
        }

        public static void QueryNinjasProjection()
        {
            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                // ninjas is an anonoymous type, properties based on Select
                var ninjas = context.Ninjas
                    .Select(n => new { n.Name } )
                    .ToList();
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine($"{ninja.Name}"); // After we change the declaration of Clan to be virtual, it is now able to load it without explcitly loading it
                }
            }
        }

        public static void QueryNinjasEager()
        {
            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjas = context.Ninjas.Include(n => n.EquipmentOwned).ToList();
                //var ninjas = context.Ninjas.Include(n => n.EquipmentOwned).Include(m=> m.Clan).ToList();
                foreach (var ninja in ninjas)
                {
                    Console.WriteLine($"{ninja.Name}, {ninja.EquipmentOwned.Count}"); // was not able to determine the Clan given the Clan ID
                }
            }
        }

        public static void QueryNinjasEagerSpecific()
        {
            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjaDavid = context.Ninjas.FirstOrDefault(n => n.Name == "David");
                // Right now, the ninja object does not have an equipment list yet
                // We can still load it manually
                context.Entry(ninjaDavid).Collection(n => n.EquipmentOwned).Load();
                Console.WriteLine(ninjaDavid.EquipmentOwned.Count);
            }
        }

        public static void UpdateNinja()
        {
            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                var ninjaFirst = context.Ninjas.FirstOrDefault();
                if (ninjaFirst == null)
                {
                    return;
                }

                Console.WriteLine($"Updating {ninjaFirst.Name}");

                ninjaFirst.ServedInOniwaban = !ninjaFirst.ServedInOniwaban; // SQL will only set the ServedInOnibawan

                context.SaveChanges();
            }
        }

        public static void UpdateNinjaDisconnected()
        {
            Ninja firstNinja = null;
            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                firstNinja = context.Ninjas.FirstOrDefault();

                Console.WriteLine($"Updating then disconnecting {firstNinja.Name}");
            }

            firstNinja.DateOfBirth = firstNinja.DateOfBirth.AddDays(10);

            using (NinjaContext context = new NinjaContext())
            {
                context.Database.Log = Console.WriteLine;
                context.Ninjas.Attach(firstNinja); // Without this, it still works
                context.Entry(firstNinja).State = EntityState.Modified; // SQL will set all fields again since it does not know which field got changed (no tracking)

                Console.WriteLine($"Updating in 2nd context: {firstNinja.Name}");

                context.SaveChanges();
            }
        }
    }
}

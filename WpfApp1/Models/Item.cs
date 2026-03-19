using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public abstract class Item
    {
        public string Name { get; protected set; }
        public string Description { get; protected set; }
        public abstract void Use(Player player);
    }

    public class Weapon : Item
    {
        public int AttackBonus { get; private set; }
        public int DefenseBonus { get; private set; }

        public Weapon(string name, int attackBonus, int defenseBonus)
        {
            Name = name;
            AttackBonus = attackBonus;
            DefenseBonus = defenseBonus;
            Description = $"Атака: +{AttackBonus}, Защита: +{DefenseBonus}";
        }

        public override void Use(Player player)
        {
            player.EquipWeapon(this);
        }
    }

    public class Armor : Item
    {
        public int DefenseBonus { get; private set; }
        public int HealthBonus { get; private set; }

        public Armor(string name, int defenseBonus, int healthBonus)
        {
            Name = name;
            DefenseBonus = defenseBonus;
            HealthBonus = healthBonus;
            Description = $"Защита: +{DefenseBonus}, Здоровье: +{HealthBonus}";
        }

        public override void Use(Player player)
        {
            player.EquipArmor(this);
        }
    }

    public class Potion : Item
    {
        public Potion()
        {
            Name = "Зелье восстановления";
            Description = "Полностью восстанавливает здоровье";
        }

        public override void Use(Player player)
        {
            player.HealToFull();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public class Player
    {
        public int Health { get; private set; }
        public int MaxHealth { get; private set; }
        public Weapon CurrentWeapon { get; private set; }
        public Armor CurrentArmor { get; private set; }
        public int CurrentFloor { get; set; }
        public bool IsFrozen { get; set; }

        public Player()
        {
            MaxHealth = 100;
            Health = MaxHealth;
            CurrentFloor = 1;
        }

        public int GetAttackPower()
        {
            return CurrentWeapon?.AttackBonus ?? 5;
        }

        public int GetDefense()
        {
            int baseDefense = 5;
            if (CurrentArmor != null)
                baseDefense += CurrentArmor.DefenseBonus;
            if (CurrentWeapon != null)
                baseDefense += CurrentWeapon.DefenseBonus;
            return baseDefense;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Max(0, Health - damage);
        }

        public void HealToFull()
        {
            Health = MaxHealth;
        }

        public void EquipWeapon(Weapon weapon)
        {
            CurrentWeapon = weapon;
        }

        public void EquipArmor(Armor armor)
        {
            CurrentArmor = armor;
            MaxHealth = 100 + (armor?.HealthBonus ?? 0);
            Health = Math.Min(Health, MaxHealth);
        }

        public bool IsDead => Health <= 0;
    }
}

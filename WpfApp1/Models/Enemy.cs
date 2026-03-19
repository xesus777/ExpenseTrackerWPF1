using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfApp1.Models
{
    public abstract class Enemy
    {
        public string Name { get; protected set; }
        public int Health { get; set; }
        public int MaxHealth { get; protected set; }
        public int Attack { get; protected set; }
        public int Defense { get; protected set; }
        public bool IsBoss { get; protected set; }
        public string ImagePath { get; protected set; }
        protected static Random _random = new Random();

        public abstract int CalculateDamage(int playerDefense, bool playerBlocking);
        public abstract string GetSpecialAbilityDescription();

        public bool IsDead => Health <= 0;
    }

    public class Goblin : Enemy
    {
        public Goblin(bool isBoss = false)
        {
            if (isBoss)
            {
                Name = "ВВГ";
                MaxHealth = (int)(60 * 2.0);
                Health = MaxHealth;
                Attack = (int)(12 * 1.5);
                Defense = (int)(3 * 1.2);
                IsBoss = true;
            }
            else
            {
                Name = "Гоблин";
                MaxHealth = 30;
                Health = MaxHealth;
                Attack = 12;
                Defense = 3;
            }
        }

        public override int CalculateDamage(int playerDefense, bool playerBlocking)
        {
            int baseDamage = Attack;

            int critChance = IsBoss ? 30 : 20;
            bool isCritical = _random.Next(100) < critChance;

            if (isCritical)
            {
                baseDamage *= 2;
            }

            
            if (playerBlocking)
            {
                
                if (_random.Next(100) < 40)
                {
                    return 0; 
                }
                else
                {
                    double blockReduction = 0.7 + (_random.NextDouble() * 0.3);
                    baseDamage = Math.Max(1, baseDamage - (int)(playerDefense * blockReduction));
                }
            }

            return baseDamage;
        }

        public override string GetSpecialAbilityDescription()
        {
            int critChance = IsBoss ? 30 : 20;
            return $"Крит {critChance}% (урон ×2)";
        }
    }

    public class Skeleton : Enemy
    {
        public Skeleton(bool isBoss = false)
        {
            if (isBoss)
            {
                Name = "Ковальский";
                MaxHealth = (int)(40 * 2.5);
                Health = MaxHealth;
                Attack = (int)(10 * 1.3);
                Defense = (int)(5 * 1.4);
                IsBoss = true;
            }
            else
            {
                Name = "Скелет";
                MaxHealth = 40;
                Health = MaxHealth;
                Attack = 10;
                Defense = 5;
            }
        }

        public override int CalculateDamage(int playerDefense, bool playerBlocking)
        {
            int baseDamage = Attack;

            
            if (playerBlocking)
            {
                
                if (_random.Next(100) < 40)
                {
                    return 0; 
                }
            }

            return baseDamage;
        }

        public override string GetSpecialAbilityDescription()
        {
            return "Игнорирует защиту";
        }
    }

    public class Mage : Enemy
    {
        public Mage(bool isBoss = false)
        {
            if (isBoss)
            {
                Name = "Архимаг C++";
                MaxHealth = (int)(25 * 1.8);
                Health = MaxHealth;
                Attack = (int)(15 * 1.6);
                Defense = (int)(2 * 1.1);
                IsBoss = true;
            }
            else
            {
                Name = "Маг";
                MaxHealth = 25;
                Health = MaxHealth;
                Attack = 15;
                Defense = 2;
            }
        }

        public override int CalculateDamage(int playerDefense, bool playerBlocking)
        {
            int baseDamage = Attack;

            int freezeChance = IsBoss ? 25 : 15;

            if (playerBlocking)
            {
                if (_random.Next(100) < 40)
                {
                    return 0; 
                }
                else
                {
                    double blockReduction = 0.7 + (_random.NextDouble() * 0.3);
                    baseDamage = Math.Max(1, baseDamage - (int)(playerDefense * blockReduction));
                }
            }

            return baseDamage;
        }

        public override string GetSpecialAbilityDescription()
        {
            int freezeChance = IsBoss ? 25 : 15;
            return $"Заморозка {freezeChance}%";
        }
    }

    public class Pestov : Enemy
    {
        public Pestov()
        {
            Name = "Пестов С––";
            MaxHealth = (int)(40 * 1.3);
            Health = MaxHealth;
            Attack = (int)(10 * 1.8);
            Defense = (int)(5 * 0.6);
            IsBoss = true;
        }

        public override int CalculateDamage(int playerDefense, bool playerBlocking)
        {
            int baseDamage = Attack;

            
            if (playerBlocking)
            {
                if (_random.Next(100) < 40)
                {
                    return 0; 
                }
            }

            return baseDamage;
        }

        public override string GetSpecialAbilityDescription()
        {
            return "Игнорирует защиту, заморозка 30%";
        }
    }
}

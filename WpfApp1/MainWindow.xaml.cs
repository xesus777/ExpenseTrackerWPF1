using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp1.Models;

namespace WpfApp1
{
    public partial class MainWindow : Window
    {
        private Player _player;
        private List<Enemy> _currentEnemies;
        private Item _currentChestItem;
        private Random _random;
        private bool _isPlayerTurn;
        private bool _isDefending;

        public MainWindow()
        {
            InitializeComponent();
            _random = new Random();
            ShowMainMenu();
        }

        private void ShowMainMenu()
        {
            MainMenuPanel.Visibility = Visibility.Visible;
            GamePanel.Visibility = Visibility.Collapsed;
            GameOverPanel.Visibility = Visibility.Collapsed;
        }

        private void StartNewGame()
        {
            _player = new Player();
            _currentEnemies = new List<Enemy>();
            _currentChestItem = null;
            _isPlayerTurn = true;
            _isDefending = false;

            UpdatePlayerUI();
            EventLogList.Items.Clear();
            AddEvent("Игра началась! Вы на 1 этаже.");

            MainMenuPanel.Visibility = Visibility.Collapsed;
            GamePanel.Visibility = Visibility.Visible;
            GameOverPanel.Visibility = Visibility.Collapsed;

            ShowNewTurnPanel();
        }

        private void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void NextTurnButton_Click(object sender, RoutedEventArgs e)
        {
            _player.CurrentFloor++;
            UpdatePlayerUI();

            if (_player.CurrentFloor % 10 == 0)
            {
                GenerateBoss();
                ShowCombatPanel();
                return;
            }

            if (_random.Next(2) == 0)
            {
                GenerateChest();
                ShowChestPanel();
            }
            else
            {
                GenerateEnemies();
                ShowCombatPanel();
            }
        }

        private void GenerateEnemies()
        {
            _currentEnemies.Clear();
            int enemyCount = _random.Next(1, 4); 

            for (int i = 0; i < enemyCount; i++)
            {
                Enemy enemy = CreateRandomEnemy();
                _currentEnemies.Add(enemy);
            }

            _isPlayerTurn = true;
            _isDefending = false;

            string enemyWord = GetEnemyWord(enemyCount);
            AddEvent($"Появилось {enemyCount} {enemyWord}!");
            UpdateCombatUI();
        }

        private string GetEnemyWord(int count)
        {
            if (count == 1) return "враг";
            if (count == 2) return "врага";
            return "врагов";
        }

        private void GenerateBoss()
        {
            _currentEnemies.Clear();
            Enemy boss = CreateRandomBoss();
            _currentEnemies.Add(boss);

            _isPlayerTurn = true;
            _isDefending = false;

            AddEvent($" БОСС: {boss.Name}");
            UpdateCombatUI();
        }

        private void GenerateChest()
        {
            _currentChestItem = CreateRandomItem();
            AddEvent($"Найден сундук: {_currentChestItem.Name}");
            ChestItemText.Text = $"{_currentChestItem.Name}\n{_currentChestItem.Description}";
        }

        private Enemy CreateRandomEnemy()
        {
            int type = _random.Next(3);
            if (type == 0)
                return new Goblin();
            else if (type == 1)
                return new Skeleton();
            else
                return new Mage();
        }
    private Enemy CreateRandomBoss()
        {
            int type = _random.Next(4);
            if (type == 0)
                return new Goblin(true);
            else if (type == 1)
                return new Skeleton(true);
            else if (type == 2)
                return new Mage(true);
            else
                return new Pestov();
        }

        private Item CreateRandomItem()
        {
            int type = _random.Next(3);
            if (type == 0)
                return new Potion();
            else if (type == 1)
                return new Weapon(GetRandomWeaponName(), _random.Next(3, 8), _random.Next(0, 3));
            else
                return new Armor(GetRandomArmorName(), _random.Next(2, 6), _random.Next(5, 15));
        }

        private string GetRandomWeaponName()
        {
            string[] names = { "Старый меч", "Острый кинжал", "Двуручный топор", "Лук охотника", "Посох мага" };
            return names[_random.Next(names.Length)];
        }

        private string GetRandomArmorName()
        {
            string[] names = { "Кожаная броня", "Кольчуга", "Латы", "Мантия мага", "Щит воина" };
            return names[_random.Next(names.Length)];
        }

        private void AttackEnemy1Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEnemies.Count < 1 || !_isPlayerTurn) return;
            AttackEnemy(0);
        }

        private void AttackEnemy2Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEnemies.Count < 2 || !_isPlayerTurn) return;
            AttackEnemy(1);
        }

        private void AttackEnemy3Button_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEnemies.Count < 3 || !_isPlayerTurn) return;
            AttackEnemy(2);
        }

        private void AttackEnemy(int index)
        {
            _isPlayerTurn = false;
            _isDefending = false;

            if (_player.IsFrozen)
            {
                AddEvent(" Вы заморожены и пропускаете ход!");
                _player.IsFrozen = false;
                ProcessEnemyTurn();
                return;
            }

            Enemy target = _currentEnemies[index];
            int damage = _player.GetAttackPower();
            target.Health -= damage;
            AddEvent($" Вы атаковали {target.Name} и нанесли {damage} урона");

            if (target.IsDead)
            {
                _currentEnemies.RemoveAt(index);
                AddEvent($" {target.Name} повержен!");
            }

            ProcessEnemyTurn();
        }

        private void DefendButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEnemies == null || _currentEnemies.Count == 0 || !_isPlayerTurn)
                return;

            _isPlayerTurn = false;
            _isDefending = true;

            if (_player.IsFrozen)
            {
                AddEvent(" Вы заморожены и пропускаете ход!");
                _player.IsFrozen = false;
                ProcessEnemyTurn();
                return;
            }

            AddEvent(" Вы встали в защитную стойку");
            ProcessEnemyTurn();
        }

        private void ProcessEnemyTurn()
        {
            List<Enemy> enemiesCopy = new List<Enemy>(_currentEnemies);

            foreach (var enemy in enemiesCopy)
            {
                if (_player.IsDead) break;

                int enemyDamage = enemy.CalculateDamage(_player.GetDefense(), _isDefending);

                if (enemyDamage > 0)
                {
                    _player.TakeDamage(enemyDamage);
                    AddEvent($" {enemy.Name} атакует и наносит {enemyDamage} урона");
                }
                else
                {
                    if (_isDefending && _random.Next(100) < 40)
                    {
                        AddEvent($" Вы уклонились от атаки {enemy.Name} и контратакуете!");

                        int counterDamage = _player.GetAttackPower() / 2;
                        if (counterDamage < 1) counterDamage = 1;

                        enemy.Health -= counterDamage;
                        AddEvent($" Контратака наносит {counterDamage} урона {enemy.Name}");

                        if (enemy.IsDead)
                        {
                            _currentEnemies.Remove(enemy);
                            AddEvent($" {enemy.Name} повержен контратакой!");
                        }
                    }
                    else if (_isDefending)
                    {
                        AddEvent($" Блок: атака {enemy.Name} ослаблена");
                    }
                }

                if (enemy is Mage && _random.Next(100) < 15)
                {
                    _player.IsFrozen = true;
                    AddEvent($" {enemy.Name} заморозил вас!");
                }
                if (enemy is Pestov && _random.Next(100) < 30)
                {
                    _player.IsFrozen = true;
                    AddEvent($" {enemy.Name} заморозил вас!");
                }
            }

            UpdatePlayerUI();

            if (_player.IsDead)
            {
                GameOver();
                return;
            }

            if (_currentEnemies.Count == 0)
            {
                AddEvent(" Все враги побеждены!");
                ShowNewTurnPanel();
                return;
            }

            _isPlayerTurn = true;
            _isDefending = false;
            UpdateCombatUI();
        }

        private void TakeItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentChestItem == null) return;

            _currentChestItem.Use(_player);
            AddEvent($" Вы взяли {_currentChestItem.Name}");
            _currentChestItem = null;

            UpdatePlayerUI();
            ShowNewTurnPanel();
        }

        private void DropItemButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentChestItem == null) return;

            AddEvent($" Вы выбросили {_currentChestItem.Name}");
            _currentChestItem = null;

            ShowNewTurnPanel();
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            StartNewGame();
        }

        private void GameOver()
        {
            AddEvent(" ВЫ ПОГИБЛИ!");
            GamePanel.Visibility = Visibility.Collapsed;
            GameOverPanel.Visibility = Visibility.Visible;
        }

        private void UpdatePlayerUI()
        {
            FloorText.Text = $"Этаж: {_player.CurrentFloor}";
            HealthBar.Maximum = _player.MaxHealth;
            HealthBar.Value = _player.Health;
            HealthText.Text = $"{_player.Health}/{_player.MaxHealth}";

            WeaponText.Text = _player.CurrentWeapon?.Name ?? "Нет оружия";
            ArmorText.Text = _player.CurrentArmor?.Name ?? "Нет брони";
        }

        private void UpdateCombatUI()
        {
            Enemy1Border.Visibility = Visibility.Collapsed;
            Enemy2Border.Visibility = Visibility.Collapsed;
            Enemy3Border.Visibility = Visibility.Collapsed;

            for (int i = 0; i < _currentEnemies.Count; i++)
            {
                Enemy enemy = _currentEnemies[i];

                switch (i)
                {
                    case 0:
                        Enemy1Border.Visibility = Visibility.Visible;
                        Enemy1Name.Text = enemy.Name;
                        Enemy1HealthBar.Maximum = enemy.MaxHealth;
                        Enemy1HealthBar.Value = enemy.Health;
                        Enemy1HealthText.Text = $"{enemy.Health}/{enemy.MaxHealth}";
                        Enemy1Ability.Text = enemy.GetSpecialAbilityDescription();

                        try
                        {
                            string imagePath = $"Images/{enemy.Name}.png";
                            Enemy1Image.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                        }
                        catch { Enemy1Image.Source = null; }
                        break;

                    case 1:
                        Enemy2Border.Visibility = Visibility.Visible;
                        Enemy2Name.Text = enemy.Name;
                        Enemy2HealthBar.Maximum = enemy.MaxHealth;
                        Enemy2HealthBar.Value = enemy.Health;
                        Enemy2HealthText.Text = $"{enemy.Health}/{enemy.MaxHealth}";
                        Enemy2Ability.Text = enemy.GetSpecialAbilityDescription();

                        try
                        {
                            string imagePath = $"Images/{enemy.Name}.png";
                            Enemy2Image.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                        }
                        catch { Enemy2Image.Source = null; }
                        break;

                    case 2:
                        Enemy3Border.Visibility = Visibility.Visible;
                        Enemy3Name.Text = enemy.Name;
                        Enemy3HealthBar.Maximum = enemy.MaxHealth;
                        Enemy3HealthBar.Value = enemy.Health;
                        Enemy3HealthText.Text = $"{enemy.Health}/{enemy.MaxHealth}";
                        Enemy3Ability.Text = enemy.GetSpecialAbilityDescription();

                        try
                        {
                            string imagePath = $"Images/{enemy.Name}.png";
                            Enemy3Image.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                        }
                        catch { Enemy3Image.Source = null; }
                        break;
                }
            }
        }

        private void ShowCombatPanel()
        {
            CombatPanel.Visibility = Visibility.Visible;
            ChestPanel.Visibility = Visibility.Collapsed;
            NextTurnButton.Visibility = Visibility.Collapsed;
            UpdateCombatUI();
        }

        private void ShowChestPanel()
        {
            CombatPanel.Visibility = Visibility.Collapsed;
            ChestPanel.Visibility = Visibility.Visible;
            NextTurnButton.Visibility = Visibility.Collapsed;
        }

        private void ShowNewTurnPanel()
        {
            CombatPanel.Visibility = Visibility.Collapsed;
            ChestPanel.Visibility = Visibility.Collapsed;
            NextTurnButton.Visibility = Visibility.Visible;
        }

        private void AddEvent(string message)
        {
            string time = DateTime.Now.ToString("HH:mm:ss");
            EventLogList.Items.Insert(0, $"[{time}] {message}");

            if (EventLogList.Items.Count > 50)
                EventLogList.Items.RemoveAt(EventLogList.Items.Count - 1);
        }
    }
}

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
            AddEvent($"Появилось {enemyCount} врагов!");
            UpdateCombatUI();
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

        private void AttackButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentEnemies == null || _currentEnemies.Count == 0 || !_isPlayerTurn)
                return;

            _isPlayerTurn = false;
            _isDefending = false;

            if (_player.IsFrozen)
            {
                AddEvent(" Вы заморожены и пропускаете ход!");
                _player.IsFrozen = false;
                ProcessEnemyTurn();
                return;
            }

            var enemy = _currentEnemies[0];
            int damage = _player.GetAttackPower();
            enemy.Health -= damage;
            AddEvent($" Вы атаковали {enemy.Name} и нанесли {damage} урона");

            if (enemy.IsDead)
            {
                _currentEnemies.Remove(enemy);
                AddEvent($" {enemy.Name} повержен!");
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

                bool isFrozen = _player.IsFrozen;

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

                if (enemy is Mage || enemy is Pestov)
                {
                    if (_random.Next(100) < (enemy is Pestov ? 30 : 15))
                    {
                        _player.IsFrozen = true;
                        AddEvent($" {enemy.Name} заморозил вас!");
                    }
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
            AddEvent("ВЫ ПОГИБЛИ!");
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
            if (_currentEnemies.Count > 0)
            {
                var enemy = _currentEnemies[0];
                EnemyNameText.Text = enemy.Name;
                EnemyHealthText.Text = $" {enemy.Health}/{enemy.MaxHealth}";
                EnemyAbilityText.Text = enemy.GetSpecialAbilityDescription();

                if (_currentEnemies.Count > 1)
                {
                    EnemyCountText.Text = $"и еще {_currentEnemies.Count - 1} враг(ов)";
                }
                else
                {
                    EnemyCountText.Text = "";
                }

                
                try
                {
                    string imagePath = $"Images/{enemy.Name}.png";
                    EnemyImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
                }
                catch
                {
                    EnemyImage.Source = null;
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

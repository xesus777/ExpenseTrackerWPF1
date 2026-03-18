using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1;
using WpfApp1.Pages;

namespace Test2
{
    [TestClass]
    public class RegistrationTests
    {
        

        [STATestMethod]
        public void RegisterTestSuccess()
        {
            // Arrange
            var registerPage = new RegisterPage();

            // Генерируем уникальный логин для теста
            string uniqueLogin = $"testuser_{DateTime.Now.Ticks}";

            // Act
            var result = registerPage.Register(
                uniqueLogin,
                "password123",
                "password123",
                "test@test.com",
                "Test User"
            );

            // Assert
            Assert.IsTrue(result.Success, "Регистрация должна быть успешной");
            Assert.AreEqual("Регистрация успешна", result.Message);
            Assert.IsNotNull(result.NewUser);
            Assert.AreEqual(uniqueLogin, result.NewUser.Username);
            Assert.AreEqual("test@test.com", result.NewUser.Email);
            Assert.AreEqual("Test User", result.NewUser.FullName);

            // Очистка (удаление тестового пользователя)
            var user = Core.Context.Users.FirstOrDefault(u => u.Username == uniqueLogin);
            if (user != null)
            {
                Core.Context.Users.Remove(user);
                Core.Context.SaveChanges();
            }
        }

        [STATestMethod]
        public void RegisterTestFail()
        {
            // Arrange
            var registerPage = new RegisterPage();
            var failedTests = new System.Collections.Generic.List<string>();

            // ТЕСТ 1: Пустые поля
            var result1 = registerPage.Register("", "", "", "", "");
            if (result1.Success != false || result1.Message != "Заполните логин и пароль")
                failedTests.Add("Пустые поля");

            // ТЕСТ 2: Короткий логин (меньше 3 символов)
            var result2 = registerPage.Register("ab", "password123", "password123", "", "");
            if (result2.Success != false || result2.Message != "Логин должен быть не менее 3 символов")
                failedTests.Add("Короткий логин");

            // ТЕСТ 3: Короткий пароль (меньше 6 символов)
            var result3 = registerPage.Register("testuser", "12345", "12345", "", "");
            if (result3.Success != false || result3.Message != "Пароль должен быть не менее 6 символов")
                failedTests.Add("Короткий пароль");

            // ТЕСТ 4: Дубликат пользователя
            // Сначала создаем пользователя
            string uniqueLogin = $"duplicate_{DateTime.Now.Ticks}";

            var createResult = registerPage.Register(uniqueLogin, "password123", "password123", "", "");
            if (!createResult.Success)
                failedTests.Add("Не удалось создать тестового пользователя");

            // Пытаемся зарегистрировать того же пользователя
            var duplicateResult = registerPage.Register(uniqueLogin, "password123", "password123", "", "");
            if (duplicateResult.Success != false || duplicateResult.Message != "Пользователь с таким логином уже существует")
                failedTests.Add("Дубликат пользователя");

            // Очистка
            var user = Core.Context.Users.FirstOrDefault(u => u.Username == uniqueLogin);
            if (user != null)
            {
                Core.Context.Users.Remove(user);
                Core.Context.SaveChanges();
            }

            // Assert
            Assert.AreEqual(2, failedTests.Count,
                $"Проваленные тесты:\n{string.Join("\n", failedTests)}");
        }
    }
}


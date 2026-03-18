using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfApp1.Pages;
using WpfApp1;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;


namespace Test
{
    [TestClass]
    public class AuthTests
    {
        [STATestMethod]
        public void AuthTest()
        {

            var loginPage = new LoginPage();



            bool result = loginPage.Auth("123", "123");


            Assert.IsTrue(result, $"Авторизация не прошла ");
        }

        [STATestMethod]
        public void AuthTestSuccess()
        {

            var loginPage = new LoginPage();
            var users = Core.Context.Users.ToList();
            var failedUsers = new List<string>();


            foreach (var user in users)
            {
                bool result = loginPage.Auth(user.Username, user.Password);
                if (!result)
                    failedUsers.Add(user.Username);
            }

            Assert.AreEqual(0, failedUsers.Count,
                $"Не прошли: {string.Join(", ", failedUsers)}");
        }

        [STATestMethod]
        public void AuthTestFail()
        {

            var loginPage = new LoginPage();
            var failedTests = new List<string>();

            

            // 1. Пустые поля
            bool result1 = loginPage.Auth("", "");
            if (result1 != false)
                failedTests.Add("Пустые поля");




            bool result4 = loginPage.Auth("123", "");
            if (result4 != false)
                failedTests.Add("Только логин");


            // 5. Только пароль
            bool result5 = loginPage.Auth("", "pass");
            if (result5 != false)
                failedTests.Add("Только пароль");

            // 6. Очень длинный логин
            string longLogin = new string('a', 100);
            bool result6 = loginPage.Auth(longLogin, "pass");
            if (result6 != false)
                failedTests.Add("Очень длинный логин");

            Assert.AreEqual(0, failedTests.Count,
                $"Провалены тесты: {string.Join(", ", failedTests)}");

        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Chrome;

namespace MoneyNovel.Tests
{
    [TestFixture]
    public class Class1
    {
        public IWebDriver driver;
        public string baseUrl;

        [SetUp]
        public void Setup()
        {
            baseUrl = "http://localhost:55481/";
            driver = new ChromeDriver();
        }

        [Test]
        public void TestCase1()
        {
            driver.FindElement(By.XPath("//*[@id='socialLoginList']/p/button")).Click();
        }

        [TearDown]
        public void TearDown()
        {
            driver.Quit();
        }
    }
}
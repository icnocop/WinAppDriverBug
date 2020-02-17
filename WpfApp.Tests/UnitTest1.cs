using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;

namespace WpfApp.Tests
{
    [TestClass]
    public class UnitTest1
    {
        Uri winAppDriverUri = new Uri("http://127.0.0.1:4723/wd/hub");

        WindowsDriverProcess driverProcess = null;

        [TestInitialize]
        public void TestInitialize()
        {
            driverProcess = new WindowsDriverProcess(winAppDriverUri);
            driverProcess.Start();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            driverProcess?.Stop();
        }

        [TestMethod]
        public void TestMethod1()
        {
            AppiumOptions options = new AppiumOptions
            {
                PlatformName = "Windows"
            };
            options.AddAdditionalCapability("platformVersion", "1.0");

            string exeFilePath = Path.Combine(Directory.GetCurrentDirectory(), "WpfApp.exe");
            options.AddAdditionalCapability("app", exeFilePath);

            WindowsDriver<WindowsElement> driver = new WindowsDriver<WindowsElement>(winAppDriverUri, options);

            Console.WriteLine(driver.CurrentWindowHandle);

            driver.CloseApp();
            driver.Quit();
        }
    }
}

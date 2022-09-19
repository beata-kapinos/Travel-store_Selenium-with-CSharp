using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using OpenQA.Selenium.Firefox;

namespace Project
{
    public class CartTests
    {
        RemoteWebDriver driver;
        IJavaScriptExecutor js;

        string baseURL = "https://fakestore.testelka.pl";
        IList<string> ProductsIDs = new List<string>() { "53", "40" };
        IList<string> ProductsURLs = new List<string>()
            {
            "/product/yoga-i-pilates-w-portugalii/",
            "/product/wspinaczka-via-ferraty/"
            };

        IWebElement CloseNotice => driver.FindElement(By.CssSelector(".woocommerce-store-notice__dismiss-link"), 2);
        IWebElement AddToCartButton => driver.FindElement(By.CssSelector(".single_add_to_cart_button "), 2);
        IWebElement GoToCartButton => driver.FindElement(By.CssSelector(".woocommerce-message .wc-forward"), 2);
        IWebElement CartTable => driver.FindElement(By.CssSelector(".shop_table.cart"), 2);
        IList<IWebElement> CartProducts => driver.FindElements(By.CssSelector("tr.cart_item"), 2);
        IList<IWebElement> TdItemsOfProducts => driver.FindElements(By.CssSelector("tr td.product-quantity .quantity input"), 2);
        IWebElement Quantity => driver.FindElement(By.CssSelector(".input-text.qty"), 2);
        IWebElement UpdateCartButton => driver.FindElement(By.CssSelector("[name='update_cart']"), 2);
        By Loaders => By.CssSelector(".blockUI");


        [SetUp]
        public void Setup()
        {
            DriverOptions options = new ChromeOptions();
            //DriverOptions options = new FirefoxOptions();
            driver = new RemoteWebDriver(options);
            js = (IJavaScriptExecutor)driver;
            driver.Manage().Timeouts().PageLoad = TimeSpan.FromSeconds(10);
            driver.Navigate().GoToUrl(baseURL);
            CloseNotice.Click();
        }

        [TearDown]
        public void QuitDriver()
        {
            driver.Quit();
        }

        [Test]
        public void ItemsAddedToCartTest()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, CartProducts.Count, "Number of products in cart is not equal to 1");
                Assert.AreEqual(ProductsIDs[0], CartProducts[0].FindElement(By.CssSelector("a")).GetAttribute("data-product_id"), "ID of the product in the cart is not" + ProductsIDs[0]);
            });

        }

        [Test]
        public void TwoItemsOfProductAddedToCartTest()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[0]);
            Quantity.Clear();
            Quantity.SendKeys("2");
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, CartProducts.Count, "Number of products in cart is not equal to 1");
                Assert.AreEqual(ProductsIDs[0], CartProducts[0].FindElement(By.CssSelector("a")).GetAttribute("data-product_id"), "ID of the product in the cart is not" + ProductsIDs[0]);
                Assert.AreEqual("2", TdItemsOfProducts[0].GetAttribute("value"), "Number of items of the product is not 2");
            });
        }

        [Test]
        public void TwoItemsOfDifferentProductsTest()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[0]);
            AddToCartButton.Click();
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[1]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            Assert.Multiple(() =>
            {
                Assert.AreEqual(2, CartProducts.Count, "Number of products in cart is not equal to 2");
                Assert.AreEqual(ProductsIDs[0], CartProducts[0].FindElement(By.CssSelector("a")).GetAttribute("data-product_id"), "ID of the product in the cart is not" + ProductsIDs[0]);
                Assert.AreEqual("1", TdItemsOfProducts[0].GetAttribute("value"), "Number of items of the product is not 1");
                Assert.AreEqual(ProductsIDs[1], CartProducts[1].FindElement(By.CssSelector("a")).GetAttribute("data-product_id"), "ID of the product in the cart is not" + ProductsIDs[1]);
                Assert.AreEqual("1", TdItemsOfProducts[1].GetAttribute("value"), "Number of items of the product is not 1");
            });
        }

        [Test]
        public void CartIsEmptyAtStartTest()
        {
            driver.Navigate().GoToUrl(baseURL + "/koszyk/");
            Assert.DoesNotThrow(() => driver.FindElement(By.CssSelector(".cart-empty.woocommerce-info")), "There is no \"Empty Cart\" message");
        }

        [Test]
        public void UserCannotAddZeroItemsTest()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[0]);
            Quantity.Clear();
            Quantity.SendKeys("0");
            AddToCartButton.Click();

            bool IsLessThanOne = (bool)js.ExecuteScript("return arguments[0].validity.rangeUnderflow", Quantity);

            Assert.Multiple(() =>
            {
                Assert.IsTrue(IsLessThanOne, "Test probably added 0 items of the product to cart. Range Underflow validation did not return \"true\".");
                Assert.Throws<WebDriverTimeoutException>(() => _ = GoToCartButton, "Option \"Go to cart\" was available. Attempt to add zero items should not cause successful adding anything to the cart");
            });
        }

        [Test]
        public void ChangeNumberOfItemsInCartTest()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            Quantity.Clear();
            Quantity.SendKeys("10");
            UpdateCartButton.Click();
            WaitUntilElementsDissapear(Loaders);

            Assert.Multiple(() =>
            {
                Assert.AreEqual(1, CartProducts.Count, "Number of products in cart is not equal to 1");
                Assert.AreEqual("10", TdItemsOfProducts[0].GetAttribute("value"), "Number of items of the product is not 10");
            });
        }

        [Test]
        public void UserCannotAddMoreItemsThanStock()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[1]);
            string Stock = driver.FindElement(By.CssSelector(".in-stock")).Text.Replace(" w magazynie", "");
            int.TryParse(Stock, out int stockNumber);
            AddToCartButton.Click();
            GoToCartButton.Click();
            _ = CartTable;
            Quantity.Clear();
            Quantity.SendKeys((stockNumber + 1).ToString());
            UpdateCartButton.Click();
            WaitUntilElementsDissapear(Loaders);

            bool IsMoreThanStock = (bool)js.ExecuteScript("return arguments[0].validity.rangeOverflow", Quantity);

            Assert.IsTrue(IsMoreThanStock, "Option \"Go to cart\" was available. Attempt to add more items than available in the stock should not cause successful adding proposed amount to the cart");
        }

        public void WaitUntilElementsDissapear(By by)
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
            try
            {
                wait.Until(d => driver.FindElements(by).Count == 0);
            }
            catch (WebDriverTimeoutException)
            {
                Console.WriteLine("Elements located by " + by + " didn't disappear in 5 seconds.");
                throw;
            }
        }
    }
}

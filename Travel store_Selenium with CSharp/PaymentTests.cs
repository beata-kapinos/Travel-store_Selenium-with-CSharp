using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Travel_store_Selenium_with_CSharp
{
    public class PaymentTests
    {
        RemoteWebDriver driver;
        IJavaScriptExecutor js;

        string baseURL = "https://fakestore.testelka.pl";
        IList<string> ProductsURLs = new List<string>()
            {
            "/product/yoga-i-pilates-w-portugalii/",
            "/product/wspinaczka-via-ferraty/"
            };
        IList<string> ProductsPricesString = new List<string>() {
            "5 399,00 zł",
            "3 299,00 zł"
        };
        IList<int> ProductsPricesInt = new List<int>() {
            5399,
            3299
        };

        IWebElement CloseNotice => driver.FindElement(By.CssSelector(".woocommerce-store-notice__dismiss-link"), 2);
        IWebElement AddToCartButton => driver.FindElement(By.CssSelector(".single_add_to_cart_button "), 2);
        IWebElement GoToCartButton => driver.FindElement(By.CssSelector(".woocommerce-message .wc-forward"), 2);
        IWebElement GoToPayment => driver.FindElement(By.CssSelector(".checkout-button"), 2);
        IWebElement PaymentForm => driver.FindElement(By.CssSelector(".checkout.woocommerce-checkout"), 2);
        IWebElement BillingFirstNameInput => driver.FindElement(By.CssSelector("#billing_first_name"), 2);
        IWebElement BillingLastNameInput => driver.FindElement(By.CssSelector("#billing_last_name_field"), 2);
        IWebElement BillingCompanyNameInput => driver.FindElement(By.CssSelector("#billing_company_field"), 2);
        IWebElement BillingCountryDropdown => driver.FindElement(By.CssSelector("#select2-billing_country-container"), 2);
        IWebElement BillingCountryPoland => driver.FindElement(By.CssSelector("#select2-billing_country-result-1mch-PL"), 2);
        IWebElement BillingAddress1 => driver.FindElement(By.CssSelector("#billing_address_1"), 2);
        IWebElement BillingAddress2 => driver.FindElement(By.CssSelector("#billing_address_2_field"), 2);
        IWebElement BillingPostcode => driver.FindElement(By.CssSelector("#billing_postcode"), 2);
        IWebElement BillingCity => driver.FindElement(By.CssSelector("#billing_city"), 2);
        IWebElement BillingPhoneNumber => driver.FindElement(By.CssSelector("#billing_phone"), 2);
        IWebElement BillingEmail => driver.FindElement(By.CssSelector("#billing_email"), 2);
        IWebElement CreateAccount => driver.FindElement(By.CssSelector("#createaccount"), 2);
        IWebElement OrderComments => driver.FindElement(By.CssSelector("#order_comments"), 2);
        IWebElement CardNumebrFrame => driver.FindElement(By.CssSelector("#stripe-card-element iframe"), 2);
        IWebElement CardNumber => driver.FindElement(By.CssSelector(".CardNumberField-input-wrapper"), 2);
        IWebElement CardNumberFrame => driver.FindElement(By.CssSelector("#stripe-exp-element iframe"), 2);
        IWebElement CardExpDate => driver.FindElement(By.CssSelector("input[name='exp-date']"), 2);
        IWebElement CardCvcCodeFrame => driver.FindElement(By.CssSelector("#stripe-cvc-element iframe"), 2);
        IWebElement CardCvcCode => driver.FindElement(By.CssSelector("input[name='cvc']"), 2);
        IWebElement Terms => driver.FindElement(By.CssSelector("#terms"), 2);
        IWebElement Placeorder => driver.FindElement(By.CssSelector("place_order"), 2);
        IWebElement ErrorsList => driver.FindElement(By.CssSelector("ul.woocommerce-error"), 2);
        IList<IWebElement> Errors => driver.FindElements(By.CssSelector("ul.woocommerce-error li"), 2);


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
        public void InputsValidationTest()
        {
            driver.Navigate().GoToUrl(baseURL + ProductsURLs[0]);
            AddToCartButton.Click();
            GoToCartButton.Click();
            GoToPayment.Click();

        }
    }
}

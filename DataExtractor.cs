using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ClosedXML.Excel;
using SeleniumExtras.WaitHelpers;

class DataExtractor
{
    public static List<string> ExtractAllProducts()
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--remote-debugging-port=9222");

        IWebDriver driver = new ChromeDriver(options);
        string baseUrl = "https://www.thenx.net";

        List<string> categoryLinks = GetCategoryLinks(driver, baseUrl);
        List<string> allProducts = new List<string>();

        foreach (string categoryUrl in categoryLinks)
        {
            Console.WriteLine($"🔍 Procesăm categoria: {categoryUrl}");
            driver.Navigate().GoToUrl(categoryUrl);
            System.Threading.Thread.Sleep(5000);

            List<string> categoryProducts = ExtractProductsFromCategory(driver);
            allProducts.AddRange(categoryProducts);
        }

        Console.WriteLine($"✅ Total produse extrase de pe site: {allProducts.Count}");
        SaveToExcel(allProducts);
        driver.Quit();
        return allProducts;
    }

    private static List<string> GetCategoryLinks(IWebDriver driver, string baseUrl)
    {
        driver.Navigate().GoToUrl(baseUrl);
        System.Threading.Thread.Sleep(3000);

        List<string> categoryLinks = new List<string>();
        var categoryElements = driver.FindElements(By.XPath("//a[contains(@href, '/catalog/')]"));

        foreach (var element in categoryElements)
        {
            string categoryUrl = element.GetAttribute("href");
            if (!categoryLinks.Contains(categoryUrl) && categoryUrl.StartsWith(baseUrl))
            {
                categoryLinks.Add(categoryUrl);
                Console.WriteLine($"📌 Găsită categorie: {categoryUrl}");
            }
        }
        return categoryLinks;
    }

    private static List<string> ExtractProductsFromCategory(IWebDriver driver)
    {
        List<string> productNames = new List<string>();
        WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));

        bool hasNextPage = true;
        while (hasNextPage)
        {
            Console.WriteLine("🔄 Procesăm pagina...");
            ScrollToBottom(driver);

            try
            {
                var productElements = driver.FindElements(By.XPath("//a[contains(@class, 'product__name')]"));
                foreach (var product in productElements)
                {
                    string name = product.Text.Trim();
                    if (string.IsNullOrEmpty(name))
                    {
                        name = product.GetAttribute("innerText")?.Trim();
                    }
                    if (string.IsNullOrEmpty(name))
                    {
                        name = "N/A";
                    }
                    productNames.Add(name);
                }
            }
            catch (WebDriverException)
            {
                Console.WriteLine("⚠️ Eroare la extragere, reîncercăm...");
                driver.Navigate().Refresh();
                System.Threading.Thread.Sleep(5000);
            }

            try
            {
                var nextPageButton = driver.FindElement(By.XPath("//a[@title='Pagina urmatoare']"));
                if (!nextPageButton.Displayed || !nextPageButton.Enabled) break;

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("arguments[0].scrollIntoView({block: 'center'});", nextPageButton);
                System.Threading.Thread.Sleep(1000);
                nextPageButton.Click();

                System.Threading.Thread.Sleep(3000);
                wait.Until(d => d.FindElements(By.XPath("//a[contains(@class, 'product__name')]")).Count > 0);
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine("⚠️ Nu s-a găsit butonul Next, ieșim din loop.");
                hasNextPage = false;
            }
        }

        return productNames;
    }

    private static void ScrollToBottom(IWebDriver driver)
    {
        IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
        long lastHeight = (long)js.ExecuteScript("return document.body.scrollHeight");

        while (true)
        {
            js.ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            System.Threading.Thread.Sleep(2000);

            long newHeight = (long)js.ExecuteScript("return document.body.scrollHeight");
            if (newHeight == lastHeight) break;
            lastHeight = newHeight;
        }
    }

    private static void SaveToExcel(List<string> productNames)
    {
        string filePath = "Produse_Site_Complet.xlsx";

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine($"🗑 Fișierul {filePath} a fost șters pentru a preveni date vechi.");
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Produse");
            worksheet.Cell(1, 1).Value = "Nume Produs";

            for (int i = 0; i < productNames.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = productNames[i];
            }

            workbook.SaveAs(filePath);
            Console.WriteLine($"✅ Datele au fost salvate în {filePath}");
        }
    }
}

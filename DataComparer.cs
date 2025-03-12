using System;
using System.Collections.Generic;
using System.IO;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using ClosedXML.Excel;
using SeleniumExtras.WaitHelpers;
using System.Linq;

class DataComparer
{
    public static void CompareAndRemoveDuplicates(string sectionUrl)
    {
        Console.WriteLine($"🔍 Procesăm secțiunea: {sectionUrl}");

        // 1️⃣ Preluăm produsele noi din site
        List<string> newProducts = ExtractProductsFromSection(sectionUrl);
        Console.WriteLine($"✅ Produse extrase din secțiune: {newProducts.Count}");

        // 2️⃣ Încărcăm baza de date inițială
        List<string> initialProducts = LoadExistingProducts();
        Console.WriteLine($"📂 Produse totale în catalog: {initialProducts.Count}");

        // 3️⃣ Comparăm listele
        List<string> remainingProducts = initialProducts.Except(newProducts).ToList();
        List<string> removedProducts = initialProducts.Intersect(newProducts).ToList();

        Console.WriteLine($"📉 Produse rămase după ștergere: {remainingProducts.Count}");
        Console.WriteLine($"🗑 Produse eliminate: {removedProducts.Count}");

        // 4️⃣ Salvăm noile fișiere Excel
        SaveToExcel(remainingProducts, "Produse_Rămase.xlsx");
        SaveToExcel(removedProducts, "Produse_Sterse.xlsx");

        Console.WriteLine("🎯 Proces de comparare finalizat!");
    }

    private static List<string> ExtractProductsFromSection(string sectionUrl)
    {
        ChromeOptions options = new ChromeOptions();
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--no-sandbox");

        IWebDriver driver = new ChromeDriver(options);
        List<string> productNames = new List<string>();

        try
        {
            driver.Navigate().GoToUrl(sectionUrl);
            System.Threading.Thread.Sleep(5000);

            bool hasNextPage = true;
            while (hasNextPage)
            {
                Console.WriteLine("🔄 Procesăm pagina...");

                // Extragere nume produse
                var productElements = driver.FindElements(By.XPath("//a[contains(@class, 'product__name')]"));
                foreach (var product in productElements)
                {
                    string name = product.Text.Trim();
                    if (!string.IsNullOrEmpty(name) && !productNames.Contains(name))
                    {
                        productNames.Add(name);
                    }
                }

                // Detectare buton "Pagina următoare"
                try
                {
                    var nextPageButton = driver.FindElement(By.XPath("//a[@title='Pagina urmatoare']"));
                    if (!nextPageButton.Displayed || !nextPageButton.Enabled) break;
                    nextPageButton.Click();
                    System.Threading.Thread.Sleep(3000);
                }
                catch (NoSuchElementException)
                {
                    hasNextPage = false;
                }
            }
        }
        finally
        {
            driver.Quit();
        }

        return productNames;
    }

    private static List<string> LoadExistingProducts()
    {
        string filePath = "Produse_Site_Complet.xlsx";
        List<string> existingProducts = new List<string>();

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"⚠️ Fișierul {filePath} nu există! Se va crea unul nou.");
            return existingProducts;
        }

        using (var workbook = new XLWorkbook(filePath))
        {
            var worksheet = workbook.Worksheet(1);
            foreach (var row in worksheet.RangeUsed().RowsUsed().Skip(1))
            {
                string productName = row.Cell(1).GetValue<string>().Trim();
                if (!string.IsNullOrEmpty(productName))
                {
                    existingProducts.Add(productName);
                }
            }
        }
        return existingProducts;
    }

    private static void SaveToExcel(List<string> products, string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            Console.WriteLine($"🗑 Fișierul {filePath} a fost șters pentru a preveni date vechi.");
        }

        using (var workbook = new XLWorkbook())
        {
            var worksheet = workbook.Worksheets.Add("Produse");
            worksheet.Cell(1, 1).Value = "Nume Produs";

            for (int i = 0; i < products.Count; i++)
            {
                worksheet.Cell(i + 2, 1).Value = products[i];
            }

            workbook.SaveAs(filePath);
            Console.WriteLine($"✅ Datele au fost salvate în {filePath}");
        }
    }
}

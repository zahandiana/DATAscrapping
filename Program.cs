using System;
using System.Collections.Generic;

class Program
{
    static void Main()
    {
        Console.WriteLine("🚀 Selectează operația:");
        Console.WriteLine("1️⃣ Extragere catalog complet");
        Console.WriteLine("2️⃣ Comparare cu bază de date și eliminare duplicate");
        Console.Write("🔹 Alege opțiunea (1 sau 2): ");
        
        string choice = Console.ReadLine();

        if (choice == "1")
        {
            Console.WriteLine("📥 Extragere catalog complet...");
            List<string> extractedProducts = DataExtractor.ExtractAllProducts();
            Console.WriteLine("✅ Catalog extras cu succes!");
        }
        else if (choice == "2")
        {
            Console.Write("🔗 Introdu URL-ul secțiunii: ");
            string sectionUrl = Console.ReadLine();

            Console.WriteLine($"🔍 Comparare produse pentru {sectionUrl}...");
            DataComparer.CompareAndRemoveDuplicates(sectionUrl);
            Console.WriteLine("✅ Comparare finalizată!");
        }
        else
        {
            Console.WriteLine("❌ Opțiune invalidă! Repornește programul.");
        }
    }
}

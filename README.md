# Data Scraping Project

## 📌 Descriere
Acest proiect este o aplicație de **data scraping** dezvoltată în **C#**, care permite extragerea și procesarea unui catalog de produse. Utilizatorii pot alege între două funcționalități principale:
- **Extragere catalog complet**
- **Comparare și eliminare produse duplicate dintr-o bază de date**

## 🛠️ Tehnologii utilizate
- **C#**
- **.NET Framework/Core**
- **Selenium** (OpenQA.Selenium, ChromeDriver) pentru web scraping
- **ClosedXML** pentru manipularea fișierelor Excel
- **HttpClient / Web Scraping Libraries**
- **Procesare fișiere / baze de date**

## 🔧 Funcționalități
1. **Extragere catalog complet**
   - Programul utilizează **Selenium** pentru a naviga pe site și a extrage produse.
   - Datele sunt stocate într-un fișier **Excel** (Produse_Site_Complet.xlsx).

2. **Comparare și eliminare duplicate**
   - Permite compararea produselor extrase cu o bază de date existentă.
   - Elimină duplicatele și generează două fișiere Excel:
     - **Produse_Rămase.xlsx** (produse unice rămase)
     - **Produse_Sterse.xlsx** (produse eliminate)

## 🚀 Instalare și rulare
1. Clonează repository-ul:
   ```sh
   git clone https://github.com/user/data-scraping-project.git
   ```
2. Deschide proiectul în **Visual Studio**.
3. Asigură-te că ai **Google Chrome** și **ChromeDriver** instalate.
4. Compilează și rulează aplicația.
5. Selectează opțiunea dorită din meniul principal:
   - `1` pentru extragere catalog complet
   - `2` pentru comparare și eliminare duplicate

## 📂 Structura codului
- **Program.cs** – Controlul principal al aplicației, gestionarea opțiunilor utilizatorului.
- **DataExtractor.cs** – Modulul care folosește Selenium pentru extragerea produselor.
- **DataComparer.cs** – Modul pentru compararea și eliminarea produselor duplicate.
- **Utilizare Excel (ClosedXML)** – Gestionarea exportului de date în fișiere Excel.

## 🛠️ Îmbunătățiri viitoare
- Adăugarea unui UI pentru interacțiune mai ușoară.
- Integrarea cu baze de date SQL sau NoSQL pentru stocare avansată.
- Suport pentru multiple surse de scraping.

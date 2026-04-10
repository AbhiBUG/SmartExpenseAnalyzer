# 💰 Smart Expense Analyzer

A Windows Forms (.NET 8) desktop application for tracking personal expenses with intelligent spending insights, category analysis, and a pie chart visualization.

---

## 📁 Project Structure

```
SmartExpenseAnalyzer/
│
├── Models/
│   └── Expense.cs              # Data model — Amount, Category, Date, Note
│
├── Services/
│   ├── ExpenseManager.cs       # Loads/saves expenses to JSON; add/remove operations
│   └── Analyzer.cs             # LINQ-based analysis: totals, top category, insights
│
├── UI/
│   └── MainForm.cs             # Main WinForms window — all UI built programmatically
│
├── Data/
│   └── expenses.json           # Persistent JSON data store (auto-created)
│
├── Program.cs                  # Entry point
└── SmartExpenseAnalyzer.csproj # MSBuild project file
```

---

## ✅ Prerequisites

| Tool | Version |
|------|---------|
| Visual Studio | 2022 (Community or higher) |
| .NET SDK | 8.0 or later |
| Workload | ".NET Desktop Development" |

---

## 🚀 How to Run in Visual Studio

1. **Open the project**
   - Launch Visual Studio 2022
   - Click **File → Open → Project/Solution**
   - Navigate to the `SmartExpenseAnalyzer` folder and open `SmartExpenseAnalyzer.csproj`

2. **Restore & Build**
   - Visual Studio will automatically restore NuGet packages (there are none here — no external dependencies)
   - Press **Ctrl + Shift + B** to build the project

3. **Run**
   - Press **F5** (Debug) or **Ctrl + F5** (Run without debugging)
   - The main window will open

> ⚠️ If you see a build error mentioning `System.Windows.Forms.DataVisualization.Charting`, see the note below.

---

## 📦 Chart Control Note

The pie chart uses `System.Windows.Forms.DataVisualization.Charting`, which is **built into the .NET 6/7/8 Windows Desktop SDK** — no extra NuGet package needed. If your build complains, verify that your project targets `net8.0-windows` (already set in the `.csproj`).

---

## 🛠️ How to Run via Command Line

```bash
# From the SmartExpenseAnalyzer folder:
dotnet build
dotnet run
```

---

## 🎯 Features at a Glance

| Feature | Details |
|---------|---------|
| Add Expense | Amount, Category, Date, optional Note |
| Delete Expense | Select row → click Delete button |
| Persistence | Auto-saved to `Data/expenses.json` on each add/delete |
| Total Spending | Displayed live in the summary panel |
| Top Category | Highlighted in the summary panel |
| Insights | Budget warnings, over-40% alerts, general tips |
| Pie Chart | Category-wise spending breakdown |

### Budget Limits
| Category | Limit |
|----------|-------|
| Food | ₹5,000 |
| Shopping | ₹3,000 |

---

## 💡 Design Decisions

- **No external NuGet packages** — uses only built-in .NET 8 libraries
- **No Designer file** — all controls built programmatically in `InitializeComponent()` for clarity
- **Separation of concerns** — UI never calls JSON or LINQ directly; delegates to services
- **Auto-save** — every add/delete immediately writes to disk; no manual save required

---

## 🐛 Troubleshooting

| Problem | Solution |
|---------|----------|
| Chart assembly not found | Ensure `.csproj` has `<UseWindowsForms>true</UseWindowsForms>` and targets `net8.0-windows` |
| `expenses.json` not created | The `Data/` folder is created automatically at startup; check write permissions |
| Form appears blank | Make sure you're running on Windows; WinForms is Windows-only |

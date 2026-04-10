# 💰 Smart Expense Analyzer
A Windows Forms (.NET 8) desktop application for tracking personal expenses with intelligent spending insights, category analysis, and a pie chart visualization.

---

## 📁 Project Structure
```
SmartExpenseAnalyzer/
│
├── Models/
│   └── Expense.cs                    # Data model — Amount, Category, Date, Note
│
├── Services/
│   ├── ExpenseManager.cs             # Add/remove expenses; routes to MongoDB or JSON fallback
│   ├── MongoExpenseRepository.cs     # MongoDB CRUD operations via MongoDB.Driver
│   └── Analyzer.cs                   # LINQ-based analysis: totals, top category, insights
│
├── UI/
│   └── MainForm.cs                   # Main WinForms window — all UI built programmatically
│
├── Data/
│   └── expenses.json                 # Local JSON fallback store (auto-created if MongoDB is unavailable)
│
├── Program.cs                        # Entry point
└── SmartExpenseAnalyzer.csproj       # MSBuild project file
```

---

## ✅ Prerequisites
| Tool | Version |
|------|---------|
| Visual Studio | 2022 (Community or higher) |
| .NET SDK | 8.0 or later |
| Workload | ".NET Desktop Development" |
| MongoDB | 6.0+ (local) **or** MongoDB Atlas (cloud) |

---

## 📦 NuGet Packages
| Package | Purpose |
|---------|---------|
| `MongoDB.Driver` | MongoDB connectivity and CRUD operations |
| `Newtonsoft.Json` | JSON serialization for local fallback storage |

Install via Package Manager Console:
```powershell
Install-Package MongoDB.Driver
Install-Package Newtonsoft.Json
```

---

## 🚀 How to Run in Visual Studio

1. **Open the project**
   - Launch Visual Studio 2022
   - Click **File → Open → Project/Solution**
   - Navigate to the `SmartExpenseAnalyzer` folder and open `SmartExpenseAnalyzer.csproj`

2. **Restore & Build**
   - Visual Studio will automatically restore NuGet packages
   - Press **Ctrl + Shift + B** to build the project

3. **Run**
   - Press **F5** (Debug) or **Ctrl + F5** (Run without debugging)
   - The main window will open

> ⚠️ If you see a build error mentioning `System.Windows.Forms.DataVisualization.Charting`, see the Chart Control Note below.

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

## 🗄️ MongoDB Setup

### Local MongoDB
1. [Download and install MongoDB Community](https://www.mongodb.com/try/download/community)
2. Start the MongoDB service:
   ```bash
   mongod --dbpath "C:\data\db"
   ```
3. The app connects to `mongodb://localhost:27017` by default — no extra configuration needed.

### MongoDB Atlas (Cloud)
1. Create a free cluster at [cloud.mongodb.com](https://cloud.mongodb.com)
2. Copy your connection string (e.g. `mongodb+srv://<user>:<pass>@cluster.mongodb.net/`)
3. Add it to `App.config`:
   ```xml
   <appSettings>
     <add key="MongoConnectionString" value="your-atlas-connection-string" />
   </appSettings>
   ```

### Fallback Behaviour
If MongoDB is **unreachable** on startup, the app automatically falls back to the local `Data/expenses.json` file — no crash, no data loss.

---

## 🎯 Features at a Glance
| Feature | Details |
|---------|---------|
| Add Expense | Amount, Category, Date, optional Note |
| Delete Expense | Select row → click Delete button |
| Persistence | Saved to MongoDB (primary) or `Data/expenses.json` (fallback) |
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
- **MongoDB-first persistence** — expenses are stored in MongoDB with automatic JSON fallback
- **No Designer file** — all controls built programmatically in `MainForm.cs` for clarity
- **Separation of concerns** — UI never calls the database or LINQ directly; delegates to services
- **Auto-save** — every add/delete immediately persists; no manual save required
- **Graceful degradation** — if MongoDB is down, the app runs fully on local JSON

---

## 🐛 Troubleshooting
| Problem | Solution |
|---------|----------|
| Chart assembly not found | Ensure `.csproj` has `<UseWindowsForms>true</UseWindowsForms>` and targets `net8.0-windows` |
| MongoDB connection refused | Make sure `mongod` is running locally, or check your Atlas connection string |
| `expenses.json` not created | The `Data/` folder is created automatically at startup; check write permissions |
| App crashes on startup | Check the Output window — a MongoDB auth error or wrong connection string is the usual cause |
| Form appears blank | Make sure you're running on Windows; WinForms is Windows-only |

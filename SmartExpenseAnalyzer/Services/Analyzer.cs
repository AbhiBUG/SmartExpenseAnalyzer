using SmartExpenseAnalyzer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartExpenseAnalyzer.Services
{
    /// <summary>
    /// Contains all analytical operations on expense data.
    /// Uses LINQ for clean, readable queries.
    /// </summary>
    public class Analyzer
    {
        // ── Budget Thresholds ─────────────────────────────────────────────────
        // Fixed budget limits per category (in currency units).
        private static readonly Dictionary<string, double> BudgetLimits = new Dictionary<string, double>()
        {
            { "Food",     5000 },
            { "Shopping", 3000 }
        };

        // ── Analysis Methods ──────────────────────────────────────────────────

        /// <summary>Returns the sum of all expense amounts.</summary>
        public double GetTotalSpending(IEnumerable<Expense> expenses)
            => expenses.Sum(e => e.Amount);

        /// <summary>
        /// Returns a dictionary mapping each category to its total spend,
        /// ordered descending by amount.
        /// </summary>
        public Dictionary<string, double> GetCategoryTotals(IEnumerable<Expense> expenses)
            => expenses
               .GroupBy(e => e.Category)
               .ToDictionary(
                   g => g.Key,
                   g => g.Sum(e => e.Amount)
               );

        /// <summary>
        /// Returns the name of the category with the highest total spend,
        /// or "N/A" if there are no expenses.
        /// </summary>
        public string GetHighestSpendingCategory(IEnumerable<Expense> expenses)
        {
            var list = expenses.ToList();
            if (!list.Any()) return "N/A";

            return list
                .GroupBy(e => e.Category)
                .OrderByDescending(g => g.Sum(e => e.Amount))
                .First()
                .Key;
        }

        /// <summary>
        /// Generates a list of human-readable insight strings based on the data.
        /// Insights include budget warnings and proportion-based advice.
        /// </summary>
        public List<string> GenerateInsights(IEnumerable<Expense> expenses)
        {
            var insights = new List<string>();
            var expenseList = expenses.ToList();

            if (!expenseList.Any())
            {
                insights.Add("💡 No expenses recorded yet. Start adding your expenses!");
                return insights;
            }

            double total = GetTotalSpending(expenseList);
            var categoryTotals = GetCategoryTotals(expenseList);
            string topCategory = GetHighestSpendingCategory(expenseList);

            // 1. Highest spending category insight
            double topAmount = categoryTotals[topCategory];
            insights.Add($"🏆 Highest spending category: {topCategory} (₹{topAmount:F2})");

            // 2. Budget limit warnings (Food, Shopping)
            foreach (var budget in BudgetLimits)
            {
                string cat = budget.Key;
                double limit = budget.Value;

                if (categoryTotals.TryGetValue(cat, out double spent))
                {
                    if (spent > limit)
                    {
                        double over = spent - limit;
                        insights.Add($"⚠️ {cat} budget exceeded by ₹{over:F2}! (Limit: ₹{limit:F2})");
                    }
                    else
                    {
                        double remaining = limit - spent;
                        insights.Add($"✅ {cat}: ₹{remaining:F2} remaining within budget.");
                    }
                }
            }

            // 3. Category > 40% of total → suggest reduction
            foreach (var entry in categoryTotals)
            {
                double percentage = (entry.Value / total) * 100;
                if (percentage > 40)
                {
                    insights.Add($"📉 {entry.Key} makes up {percentage:F1}% of total spending. Consider reducing it.");
                }
            }

            // 4. General total summary
            insights.Add($"💰 Total spending so far: ₹{total:F2}");

            return insights;
        }
    }
}

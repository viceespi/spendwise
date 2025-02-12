using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using iText;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using Popoupa.Core.DataBaseManipulation;
using Popoupa.Core.DataBaseManipulation.Repositories;
using Popoupa.Core.DomainModels;
using Popoupa.Core.Parsers.Interfaces;

namespace Popoupa.Core.Parsers
{
    public class NubankPDFParser : INubankPDFParser
    {
        private static readonly CultureInfo Brazilian = new CultureInfo("pt-BR");

        private static readonly Dictionary<string, string> months = new Dictionary<string, string>
            {
                {"JAN", "01"},
                {"FEV", "02"},
                {"MAR", "03"},
                {"ABR", "04"},
                {"MAI", "05"},
                {"JUN", "06"},
                {"JUL", "07"},
                {"AGO", "08"},
                {"SET", "09"},
                {"OUT", "10"},
                {"NOV", "11"},
                {"DEZ", "12"}
            };
        public List<Expense> Parse(byte[] fileContents, Guid userId, Category category, Group group)
        {
            var expensesList = new List<Expense>();
            var pdfText = GetTextFromPDF(fileContents);
            string[] pdfLines = pdfText.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            var referenceDate = GetDateOfReference(pdfLines);
            for (int index = 0; index < pdfLines.Length; index++)
            {
                var pdfLine = pdfLines[index];
                var lineNumber = index + 1;
                if (CheckIfIsExpense(pdfLine, lineNumber))
                {
                    var expenseDate = GetExpenseDate(pdfLine, referenceDate, lineNumber);
                    var expenseDescription = GetExpenseDescription(pdfLine, lineNumber);
                    var expenseAmount = GetExpenseAmount(pdfLine, lineNumber);
                    var id = Guid.Empty;
                    var expense = new Expense(id, expenseDescription, expenseAmount, expenseDate, category, group, false, userId);
                    expensesList.Add(expense);
                }
            }
            return expensesList;
        }
        private string GetTextFromPDF(byte[] pdfBytes)
        {
            using (MemoryStream memoryStream = new MemoryStream(pdfBytes))
            {
                using (PdfReader reader = new PdfReader(memoryStream))
                {
                    using (PdfDocument pdfDocument = new PdfDocument(reader))
                    {
                        StringBuilder pdfText = new StringBuilder();
                        for (int page = 3; page <= pdfDocument.GetNumberOfPages(); page++)
                        {
                            string PageText = PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(page));
                            pdfText.AppendLine(PageText);
                        }
                        if (pdfText is null) throw new InvalidBankStatementException(null, "The bankstatement is invalid.");
                        return pdfText.ToString();
                    }
                }
            }
        }

        private bool CheckIfIsExpense(string pdfLine, int lineNumber)
        {
            try
            {
                if (!CheckIfDateIsValid(pdfLine, lineNumber)) return false;
                if (!CheckIfTheValueIsValid(pdfLine, lineNumber)) return false;
                if (pdfLine.Substring(7,7) == "Estorno") return false;
                return true;
            }
            catch (Exception)
            {
                throw new InvalidBankStatementException(lineNumber, $"Invalid bankstatement. The line {lineNumber} is invalid");
            }
        }

        private bool CheckIfTheValueIsValid(string pdfLine, int lineNumber)
        {
            try
            {
                CultureInfo culture = CultureInfo.GetCultureInfo("pt-BR");
                var startingIndex = (pdfLine.LastIndexOf(' ') + 1);
                if (startingIndex == 1) throw new Exception();
                var value = pdfLine.Substring(startingIndex, pdfLine.Length - startingIndex);
                if (decimal.TryParse(value, NumberStyles.Currency, culture, out _))
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                throw new InvalidBankStatementException(lineNumber, $"The bankstatement is Invalid, there is no value in line {lineNumber}");
            }
        }

        private bool CheckIfDateIsValid(string pdfLine, int lineNumber)
        {
            try
            {
                if (!char.IsDigit(pdfLine[0]) || !char.IsDigit(pdfLine[1])) return false;
                if (int.Parse(pdfLine.Substring(0, 2)) < 01 || int.Parse(pdfLine.Substring(0, 2)) > 31) return false;
                if (!months.ContainsKey(pdfLine.Substring(3, 3))) return false;
                return true;

            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidBankStatementException(lineNumber, $"Invalid bankstatement. The line {lineNumber} is invalid");
            }
            catch (ArgumentNullException)
            {
                throw new InvalidBankStatementException(lineNumber, $"Invalid bankstatement. The line {lineNumber} is invalid");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidBankStatementException(lineNumber, $"Invalid bankstatement. The line {lineNumber} is invalid");
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }

        private string[] GetDateOfReference(string[] pdfLines)
        {
            try
            {
                var referenceMonth = pdfLines[1].Substring(10, 3);
                var referenceYear = pdfLines[1].Substring(14, 4);
                string[] referenceDate = { referenceMonth, referenceYear };
                return referenceDate;
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidBankStatementException(null, "Invalid bankstatement. There is no valid reference date");
            }

        }

        private DateTime GetExpenseDate(string pdfLine, string[] referenceDate, int lineNumber)
        {

            try
            {
                var expenseDay = pdfLine.Substring(0, 2);
                var expenseNumberMonth = months[pdfLine.Substring(3, 3)];
                var expenseFullDate = $"{expenseDay}/{expenseNumberMonth}/{referenceDate[1]}";
                var date = DateTime.ParseExact(expenseFullDate, "dd/MM/yyyy", Brazilian);
                if (expenseNumberMonth == "12")
                {
                    var newYear = date.Year - 1;
                    date = new DateTime(newYear, date.Month, date.Day, date.Hour, date.Minute, date.Second);
                }
                if (date <= DateTime.UtcNow) return date;
                throw new InvalidBankStatementException(lineNumber, "The date is invalid");
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new InvalidBankStatementException(lineNumber, "The line is incomplete");
            }
            catch (ArgumentNullException)
            {
                throw new InvalidBankStatementException(lineNumber, "The date is invalid");
            }
            catch (FormatException)
            {
                throw new InvalidBankStatementException(lineNumber, "The date is in a invalid format");
            }
        }

        private string GetExpenseDescription(string pdfLine, int lineNumber)
        {
            try
            {
                const int startIndex = 7;
                var endIndex = pdfLine.LastIndexOf(' ');
                var expenseDescription = pdfLine.Substring(startIndex, endIndex - startIndex);
                if (expenseDescription is null || expenseDescription.Length < 2) throw new Exception();
                return expenseDescription;
            }
            catch (Exception)
            {
                throw new InvalidBankStatementException(lineNumber, $"Invalid bankstatement. There is no valid description in line {lineNumber}");
            }
        }

        private decimal GetExpenseAmount(string pdfLine, int lineNumber)
        {
            try
            {
                var startingIndex = (pdfLine.LastIndexOf(' ') + 1);
                if (startingIndex == 1) throw new Exception();
                var value = pdfLine.Substring(startingIndex, pdfLine.Length - startingIndex);
                if (decimal.TryParse(value, NumberStyles.Currency, Brazilian, out var expenseValue))
                {
                    return expenseValue;
                }
                throw new Exception();
            }
            catch (Exception)
            {
                throw new InvalidBankStatementException(lineNumber, $"Invalid bankstatement. There is no valid value in line {lineNumber}");
            }
        }
    }
}

using System.Text.Json.Serialization;
using Popoupa.Core;
using Popoupa.Core.DomainModels;

namespace Popoupa.API.APIClasses
{
    public class BankStatementMessageObject
    {
        [JsonConstructor]
        public BankStatementMessageObject(IEnumerable<byte> bsContent, string bsFormatType, Category expensesCategory, Group expensesGroup, int monthOfReference) {
            BSFormatType = bsFormatType;
            BSContent = bsContent;
            ExpensesCategory = expensesCategory;
            ExpensesGroup = expensesGroup;
            MonthOfReference = monthOfReference;
        }
        public string BSFormatType { get; set; } = string.Empty;

        public IEnumerable<byte> BSContent { get; set; } = [];

        public Category ExpensesCategory {get; set;}

        public Group ExpensesGroup {get; set;}

        public int MonthOfReference {get; set;}
    }
}

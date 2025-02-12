using System.Text;
using Popoupa.Core.DomainModels;

namespace Popoupa.Core.Parsers.Interfaces
{
    public interface INubankPDFParser
    {
        public List<Expense> Parse(byte[] fileContents, Guid userId, Category category, Group group);
    }
}

using System.Text;
using Popoupa.Core.DomainModels;

namespace Popoupa.Core.Parsers.Interfaces
{
    public interface INubankCSVParser
    {
        public List<Expense> Parse(byte[] fileContents, Encoding fileEncoding, Guid userId, Category category, Group group);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SurrealDb.Net.Models;

namespace Popoupa.Core.DomainModels
{
    public class Category
    {
        public Category(Guid id, string name)
        {
            Id = id;
            Name = name;
        }
        public string Name { get; set; }
        public Guid Id { get; set; }
    }
}

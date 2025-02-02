using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpendWise.Domain.Models.GlobalModels
{
    public class ValidationErrors
    {
        public List<string> Errors { get; } = new List<string>();
        public bool HasError => this.Errors.Count > 0;
    }
}
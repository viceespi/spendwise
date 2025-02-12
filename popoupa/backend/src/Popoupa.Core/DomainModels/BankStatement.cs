using SurrealDb.Net.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Popoupa.Core.DomainModels
{
    public class BankStatement
    {
        public BankStatement(DateTime submissionDate, int referenceMonth, string bankName, Guid ownerId)
        {
            MonthOfReference = referenceMonth;
            BankName = bankName;
            SubmissionDate = submissionDate;
            OwnerId = ownerId;
        }
        public BankStatement(Guid id, DateTime submissionDate, int referenceMonth, string bankName, Guid ownerId)
        {
            Id = id;
            MonthOfReference = referenceMonth;
            BankName = bankName;
            SubmissionDate = submissionDate;
            OwnerId = ownerId;
        }
        public Guid? Id { get; }

        public int MonthOfReference { get; }

        public string BankName { get; }

        public DateTime SubmissionDate { get; }

        public Guid OwnerId { get; }

    }
}

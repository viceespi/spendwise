using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SpendWise.Domain.Models.GlobalModels
{
    public class Result<T>
    {
        public Result(ValidationErrors validationErrors)
        {
            ValidationErrors = validationErrors;
        }
        public Result(T result)
        {
            OperationResult = result;
        }
        public T? OperationResult {get;}
        public ValidationErrors? ValidationErrors { get; }

        public TReturn Match<TReturn>(Func<T, TReturn> sucessFn, Func<ValidationErrors, TReturn> failFn)
        {
            if (OperationResult is not null && ValidationErrors is null)
            {
                return sucessFn(OperationResult);
            }
            else
            {
                return failFn(ValidationErrors!);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Castle.Core.Resource;

namespace Popoupa.Core.DataBaseManipulation.Filters
{
    public class Filter
    {
        public DateTime? StartingDate { get; set; }
        public DateTime? EndingDate { get; set; }
        public string? ExpenseDescription { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? GroupId { get; set; }
        public Guid? ImportId { get; set; }

        public Filter CreateFilterFromQuery(string? query)
        {
            var filterMap = CreateFilterMapFromQueryString(query);
            var filter = CreateFilterFromFilterMap(filterMap);
            return filter;
        }

        private static Dictionary<string, string> CreateFilterMapFromQueryString(string? query)
        {

            Dictionary<string, string> filterMap = new Dictionary<string, string>
            {
                {"StartingDate", string.Empty},
                {"EndingDate", string.Empty},
                {"ExpenseDescription", string.Empty},
                {"CategoryId", string.Empty},
                {"GroupId", string.Empty},
                {"ImportId", string.Empty}
            };
            if (query is null || query == string.Empty) return filterMap;
            var editedQuery = query.Substring(1);
            var filterCriteriasList = editedQuery.Split('&');
            foreach (string linkedPair in filterCriteriasList)
            {
                var splitedPair = linkedPair.Split('=');
                if (splitedPair[1] != "")
                {
                    try
                    {
                        filterMap[splitedPair[0]] = splitedPair[1];
                    }
                    catch
                    {
                        throw new Exception("Invalid query parameters");
                    }
                }
            };
            return filterMap;
        }

        private Filter CreateFilterFromFilterMap(Dictionary<string, string> filterMap)
        {
            var filter = new Filter();
            if (filterMap["StartingDate"] != string.Empty)
            {
                try
                {
                    var decodedDateString = WebUtility.UrlDecode(filterMap["StartingDate"]);
                    DateTime filterDate = DateTime.ParseExact(decodedDateString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    filter.StartingDate = filterDate;
                }
                catch
                {
                    throw new Exception("Invalid starting date format in the query");
                }
            }

            if (filterMap["EndingDate"] != string.Empty)
            {
                try
                {
                    var decodedDateString = WebUtility.UrlDecode(filterMap["EndingDate"]);
                    DateTime filterDate = DateTime.ParseExact(decodedDateString, "yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal);
                    filter.EndingDate = filterDate;
                }
                catch
                {
                    throw new Exception("Invalid ending date format in the query");
                }
            }

            if (filterMap["ExpenseDescription"] != string.Empty)
            {
                filter.ExpenseDescription = $"%{filterMap["ExpenseDescription"]}%";
            }

            if (filterMap["CategoryId"] != string.Empty)
            {
                try
                {
                    var categoryId = Guid.Parse(filterMap["CategoryId"]);
                    filter.CategoryId = categoryId;
                }
                catch
                {
                    throw new Exception("Invalid CategoryId format in the query");
                }
            }

            if (filterMap["GroupId"] != string.Empty)
            {
                try
                {
                    var groupId = Guid.Parse(filterMap["GroupId"]);
                    filter.GroupId = groupId;
                }
                catch
                {
                    throw new Exception("Invalid GroupId format in the query");
                }
            }

            if (filterMap["ImportId"] != string.Empty)
            {
                try
                {
                    var importId = Guid.Parse(filterMap["ImportId"]);
                    filter.ImportId = importId;
                }
                catch
                {
                    throw new Exception("Invalid ImportId format in the query");
                }
            }
            return filter;
        }
    }
}

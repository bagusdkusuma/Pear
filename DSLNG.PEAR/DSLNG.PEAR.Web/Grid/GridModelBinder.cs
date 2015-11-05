using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DSLNG.PEAR.Web.Grid
{
    public class GridModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var request = controllerContext.HttpContext.Request;
            return new GridParams
            {
                Echo = BindDataTablesRequestParam<Int32>(bindingContext, "sEcho"),
                DisplayStart = BindDataTablesRequestParam<Int32>(bindingContext, "iDisplayStart"),
                DisplayLength = BindDataTablesRequestParam<Int32>(bindingContext, "iDisplayLength"),
                ColumnNames = BindDataTablesRequestParam<string>(bindingContext, "sColumns"),
                Columns = BindDataTablesRequestParam<Int32>(bindingContext, "iColumns"),
                Search = BindDataTablesRequestParam<string>(bindingContext, "sSearch"),
                Regex = BindDataTablesRequestParam<bool>(bindingContext, "bRegex"),
                SortingCols = BindDataTablesRequestParam<Int32>(bindingContext, "iSortingCols"),
                DataProp = BindDataTablesRequestParam<string>(controllerContext.HttpContext.Request, "mDataProp_"),
                RegexColumns = BindDataTablesRequestParam<bool>(controllerContext.HttpContext.Request, "bRegex_"),
                Searchable = BindDataTablesRequestParam<bool>(controllerContext.HttpContext.Request, "bSearchable_"),
                Sortable = BindDataTablesRequestParam<bool>(controllerContext.HttpContext.Request, "bSortable_"),
                SortCol = BindDataTablesRequestParam<Int32>(controllerContext.HttpContext.Request, "iSortCol_"),
                SearchColumns = BindDataTablesRequestParam<string>(controllerContext.HttpContext.Request, "sSearch_"),
                SortDir = BindDataTablesRequestParam<string>(controllerContext.HttpContext.Request, "sSortDir_"),
                SortingDictionary = CollectSortingDirectory(controllerContext.HttpContext.Request)
            };

        }

        private static T BindDataTablesRequestParam<T>(ModelBindingContext context, string propertyName)
        {
            if (context.ValueProvider.GetValue(propertyName) != null)
            {
                return (T)context.ValueProvider.GetValue(propertyName).ConvertTo(typeof(T));
            }

            return default(T);
        }

        private static List<T> BindDataTablesRequestParam<T>(HttpRequestBase request, string keyPrefix)
        {
            return (from k in request.Params.AllKeys
                    where k.StartsWith(keyPrefix)
                    orderby k
                    select (T)Convert.ChangeType(request.Params[k], typeof(T))
                   ).ToList();
        }

        private static IDictionary<string, SortOrder> CollectSortingDirectory(HttpRequestBase request)
        {
            var keyPrefix = "sSortDir_";
            var sortColPrefex = "iSortCol_";
            var columnsPrefix = "sColumns";

            IDictionary<string, SortOrder> dict = new Dictionary<string, SortOrder>();
            if (request.Params[columnsPrefix] == null)
            {
                return dict;
            }

            var columnNames = request.Params[columnsPrefix].Split(',');
            foreach (var key in request.Params.AllKeys.Where(x => x.StartsWith(keyPrefix)).ToList())
            {
                var index = int.Parse(key.Substring(keyPrefix.Length, key.Length - keyPrefix.Length));
                var colIndex = int.Parse(request.Params[sortColPrefex + index.ToString()]);
                if (request.Params[key].Equals("asc", StringComparison.InvariantCultureIgnoreCase))
                {
                    dict.Add(columnNames[colIndex], SortOrder.Ascending);
                }
                else
                {
                    dict.Add(columnNames[colIndex], SortOrder.Descending);
                }
            }
            return dict;
        }
    }
}
using Google.Apis.Auth.OAuth2;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Reflection;
using System.Linq;
using Google.Apis.Http;

namespace Data
{
    public class MyData
    {
        public string People { get; set; }
        public int Age { get; set; }
    }
    public class GoogleSheetProvider<T> where T : class
    {
        static string[] Scopes = { SheetsService.Scope.SpreadsheetsReadonly };
        static string ApplicationName = "My Project 72320";

        private static IConfigurableHttpClientInitializer GetSheetCredential()
        {
            using (var stream = new FileStream("client_secret.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleCredential.FromStream(stream).CreateScoped(Scopes);
            }
        }
        private static SheetsService GetSheetsService(IConfigurableHttpClientInitializer userCredential)
        {
            // Create Google Sheets API service.
            return new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = GetSheetCredential(),
                ApplicationName = ApplicationName,
            });
        }

        #region не проверено 
        //public static void FillSpreadSheet(SheetsService service, string spreadsheetId, string[,] data)
        //{
        //    List<Request> requests = new List<Request>();
        //    for (int i = 0; i < data.GetLength(0); i++)
        //    {
        //        List<CellData> values = new List<CellData>();
        //        for (int j = 0; j < data.GetLength(1); j++)
        //        {
        //            values.Add(new CellData
        //            {
        //                UserEnteredValue = new ExtendedValue
        //                {
        //                    StringValue = data[i, j]
        //                }
        //            });
        //        }

        //        requests.Add(new Request
        //        {
        //            UpdateCells = new UpdateCellsRequest
        //            {
        //                Start = new GridCoordinate
        //                {
        //                    SheetId = 0,
        //                    RowIndex = 1,
        //                    ColumnIndex = 0
        //                },
        //                Rows = new List<RowData> { new RowData { Values = values } },
        //                Fields = "userEnteredValue"
        //            }
        //        });
        //    }
        //    BatchUpdateSpreadsheetRequest busr = new BatchUpdateSpreadsheetRequest { Requests = requests };
        //    service.Spreadsheets.BatchUpdate(busr, spreadsheetId).Execute();
        //}
        #endregion не проверено 

        public static List<T> GetValues<T>(SheetsService service, String spreadsheetId, String range) where T : class
        {
            List<T> values = new List<T>();

            SpreadsheetsResource.ValuesResource.GetRequest request = service.Spreadsheets.Values.Get(spreadsheetId, range);
            // Prints the names and majors of students in a sample spreadsheet:
            // https://docs.google.com/spreadsheets/d/1BxiMVs0XRA5nFMdKvBdBZjgmUUqptlbs74OgvE2upms/edit
            ValueRange response = request.Execute();

            #region old
            //IList<IList<Object>> values = response.Values;
            //if (values != null && values.Count > 0)
            //{
            //    Console.WriteLine("Name, Major");
            //    foreach (var row in values)
            //    {
            //        // Print columns A and E, which correspond to indices 0 and 4.
            //        Console.WriteLine("{0}, {1}", row[0], row[1]);
            //    }
            //}
            #endregion old 
            
            MapFields m = null;
            foreach (IList<Object> row in response.Values)
            {
                if (m == null)
                {
                    m = MappedFields(typeof(T), row);
                    continue;
                }
                    
                if (!DataMapper<T>.Read(m, row, out T value))
                    throw new Exception($"not read in {typeof(T)}");
                values.Add(value);
            }

            return values;
        }

        class MapField
        {
            public string FieldName { get; private set; }
            public int FieldIndex { get; private set; }
            public MapField(string fieldName, int fieldIndex)
            {
                FieldName = fieldName;
                FieldIndex = fieldIndex;
            }
        }

        class MapFields
        {
            public List<MapField> Values = new List<MapField>();
        }

        private static MapFields MappedFields(Type type, IList<Object> googeHeadRow)
        {
            MapFields m = new MapFields();
            foreach (var prop in type.GetProperties())
            {
                for (int i = 0; i < googeHeadRow.Count; i++)
                {
                    if (googeHeadRow[i].ToString() == prop.Name)
                    {
                        m.Values.Add(new MapField(prop.Name, i));
                        break;
                    }
                }
            }
            return m;
        }

        class DataMapper<T> where T : class
        {
            public static string GetFieldName(MapFields mapFields, int index)
            {
                foreach(var f in mapFields.Values)
                {
                    if (f.FieldIndex == index)
                    {
                        return f.FieldName;
                    }
                }
                throw new IndexOutOfRangeException($"index {index} is out");
            }
            public static int GetFieldIndex(MapFields mapFields, string fieldName)
            {
                foreach (var f in mapFields.Values)
                {
                    if (f.FieldName == fieldName)
                    {
                        return f.FieldIndex;
                    }
                }
                throw new IndexOutOfRangeException($"field name {fieldName} is out");
            }
            public static bool Read(MapFields map, IList<Object> googeRow, out T value) 
            {
                value = null;
                try
                {
                    value = Activator.CreateInstance<T>();
                    PropertyInfo[] properties = value.GetType().GetProperties();
                    foreach (PropertyInfo propertyInfo in properties)
                    {
                        Object obj = googeRow[GetFieldIndex(map, propertyInfo.Name)];
                        propertyInfo.SetValue((object)value, Convert.ChangeType(obj, propertyInfo.PropertyType), (object[])null);
                    }
                    return true;
                }
                catch
                {
                    return false;
                }                
            }
        }

        public static List<T> Get<T>(string spreadsheetId, string range) where T : class
        {
            return GetValues<T>(GetSheetsService(GetSheetCredential()), spreadsheetId, range);
        }
        private static void UpdatGoogleSheetinBatch(IList<IList<Object>> values, string spreadsheetId, string newRange, SheetsService service)
        {
            SpreadsheetsResource.ValuesResource.AppendRequest request =
               service.Spreadsheets.Values.Append(new ValueRange() { Values = values }, spreadsheetId, newRange);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var response = request.Execute();
        }
        static void CreateEntry(string spreadsheetId, string range, SheetsService service)
        {
            //var range = $"{sheet}!A:F";
            var valueRange = new ValueRange();

            var oblist = new List<object>() { "Hello!", "222" };
            valueRange.Values = new List<IList<object>> { oblist };

            var appendRequest = service.Spreadsheets.Values.Append(valueRange, spreadsheetId, range);
            appendRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.USERENTERED;
            var appendReponse = appendRequest.Execute();
        }
        public static void Set(string spreadsheetId, string range) 
        {
            //FillSpreadSheet(GetSheetsService(GetSheetCredential()), "1kE61PAN5zG_Ygx98PmaRxKZp0L1_NxSelEfOFC2owJs", data);
            CreateEntry(spreadsheetId, range, GetSheetsService(GetSheetCredential()));
        }
    }
}

using MESWebDev.Common.UploadEngine.Attributes;
using MESWebDev.Common.UploadEngine.Helpers;
using System.Data;
using System.Reflection;

namespace MESWebDev.Common.UploadEngine.Services
{
    public class UploadEngineService
    {
        private readonly ExcelReaderService _excelReader;
        private readonly BulkInsertService _bulkInsert;

        public UploadEngineService(ExcelReaderService excelReader, BulkInsertService bulkInsert)
        {
            _excelReader = excelReader;
            _bulkInsert = bulkInsert;
        }

        public async Task<string> UploadFile(IFormFile file, string type)
        {
            var modelType = GetModelType(type);
            var tableAttr = modelType.GetCustomAttribute<UploadTableAttribute>();

            var excelTable = _excelReader.ReadExcel(file);

            var data = MapToModel(excelTable, modelType);

            var dt = DatatableHelper.ToDataTable(modelType, data);

            await _bulkInsert.BulkInsertAsync(tableAttr.TableName, dt);

            return $"{data.Count} rows uploaded";
        }

        private Type GetModelType(string type)
        {
            var models = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.GetCustomAttribute<UploadTableAttribute>() != null)
                .ToDictionary(
                    t => t.Name.Replace("Model", "").ToLower(),
                    t => t
                );

            if (!models.ContainsKey(type.ToLower()))
                throw new Exception("Upload type not supported");

            return models[type.ToLower()];
        }

        private List<object> MapToModel(DataTable table, Type modelType)
        {
            List<object> result = new();

            var props = modelType.GetProperties();

            foreach (DataRow r in table.Rows)
            {
                var obj = Activator.CreateInstance(modelType);

                foreach (var p in props)
                {
                    var attr = p.GetCustomAttribute<ExcelColumnAttributes>();

                    if (attr == null) continue;

                    foreach (var name in attr.Names)
                    {
                        if (!table.Columns.Contains(name)) continue;

                        var val = r[name];

                        if (val != DBNull.Value)
                            p.SetValue(obj, Convert.ChangeType(val, p.PropertyType));

                        break;
                    }
                }

                result.Add(obj);
            }

            return result;
        }

    }
}

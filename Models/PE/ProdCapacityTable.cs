using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;

namespace MESWebDev.Models.PE
{
    public sealed class ProdCapacityColumn
    {
        public ProdCapacityColumn(string label, string key, string group, string type = "text")
        {
            Label = label;
            Key = key;
            Group = group;
            Type = type;
        }

        public string Label { get; }
        public string Key { get; }
        public string Group { get; }
        public string Type { get; }
        public bool IsNumeric => Type == "hc" || Type == "output" ||
                                 Type == "points" || Type == "man-day";
    }

    public sealed class ProdCapacityColumnGroup
    {
        public ProdCapacityColumnGroup(string name, int count)
        {
            Name = name;
            Count = count;
        }

        public string Name { get; }
        public int Count { get; }
    }

    /// <summary>
    /// UI-only column metadata and formatting. Search and formulas live in SQL.
    /// All production formulas (FY, HC/Output, total Man-day) live in SQL SP.
    /// </summary>
    public static class ProdCapacityTable
    {
        public static IReadOnlyList<ProdCapacityColumn> Columns { get; } =
            Array.AsReadOnly(new[]
            {
                new ProdCapacityColumn("Type", "Type", "PRODUCT"),
                new ProdCapacityColumn("Customer 1", "Customer1", "PRODUCT"),
                new ProdCapacityColumn("Customer 2", "Customer2", "PRODUCT"),
                new ProdCapacityColumn("U-No", "UNo", "PRODUCT"),
                new ProdCapacityColumn("B-Model", "BModel", "PRODUCT"),
                new ProdCapacityColumn("PCB No", "PCBNo", "PRODUCT"),
                new ProdCapacityColumn("PCB Type", "PCBType", "PRODUCT"),
                new ProdCapacityColumn("Date", "UpdateDate", "PERIOD", "date"),
                new ProdCapacityColumn("FY", "FY", "PERIOD"),
                new ProdCapacityColumn("VersionStatus", "VersionStatus", "PERIOD"),
                new ProdCapacityColumn("SMT Points", "SMTPointsTotal", "PERIOD", "points"),
                new ProdCapacityColumn("SMT", "MdSMT", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("Insert", "MdInsert", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("SC", "MdSC", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("Assy", "MdAssy", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("MIC", "MdMic", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("Other", "MdOther", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("Total", "MdTotal", "MAN-DAY", "man-day"),
                new ProdCapacityColumn("COB HC", "COBHC", "COB", "hc"),
                new ProdCapacityColumn("COB Output", "COBOutput", "COB", "output"),
                new ProdCapacityColumn("SMT HC", "SMTHC", "SMT", "hc"),
                new ProdCapacityColumn("SMT Output", "SMTOutput", "SMT", "output"),
                new ProdCapacityColumn("Insert HC", "InsertHC", "INSERT", "hc"),
                new ProdCapacityColumn("Insert Output", "InsertOutput", "INSERT", "output"),
                new ProdCapacityColumn("SC HC", "SCHC", "SC", "hc"),
                new ProdCapacityColumn("SC Output", "SCOutput", "SC", "output"),
                new ProdCapacityColumn("Assy HC", "AssyHC", "ASSY", "hc"),
                new ProdCapacityColumn("Assy Output", "AssyOutput", "ASSY", "output"),
                new ProdCapacityColumn("Mic HC", "MicHC", "MIC", "hc"),
                new ProdCapacityColumn("Mic Output", "MicOutput", "MIC", "output"),
                new ProdCapacityColumn("Other HC", "OtherHC", "OTHER", "hc"),
                new ProdCapacityColumn("Other Output", "OtherOutput", "OTHER", "output"),
                new ProdCapacityColumn("Top", "SMTTop", "PCB", "hc"),
                new ProdCapacityColumn("Bottom", "SMTBottom", "PCB", "hc"),
                new ProdCapacityColumn("TTL", "SMTTotal", "PCB", "hc"),
                new ProdCapacityColumn("Insert", "Insert", "LAYOUT", "hc"),
                new ProdCapacityColumn("Pcs / Sheet", "PcsPerSheet", "LAYOUT", "hc")
            });

        public static IReadOnlyList<ProdCapacityColumnGroup> Groups { get; } =
            Array.AsReadOnly(Columns
                .GroupBy(c => c.Group)
                .Select(g => new ProdCapacityColumnGroup(g.Key, g.Count()))
                .ToArray());

        public static string Text(DataRow row, string key)
        {
            var value = Value(row, key);
            return value == null
                ? string.Empty
                : Convert.ToString(value, CultureInfo.InvariantCulture)?.Trim() ?? string.Empty;
        }

        // No calculation here. Formats already calculated values from the stored procedure.
        public static string Display(DataRow row, ProdCapacityColumn column)
        {
            var value = Value(row, column.Key);
            if (value == null)
                return string.Empty;

            if (column.Type == "date")
            {
                if (value is DateTime date)
                    return date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);

                if (DateTime.TryParse(Text(row, column.Key),
                    CultureInfo.InvariantCulture, DateTimeStyles.None, out date))
                    return date.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture);
            }

            if (column.IsNumeric)
            {
                decimal number;
                bool valid;
                if (value is string text)
                {
                    // Use Float (no thousands separator) so Excel-like "4.5" stays 4.5.
                    valid = decimal.TryParse(text, NumberStyles.Float,
                                             CultureInfo.InvariantCulture, out number);
                }
                else
                {
                    try
                    {
                        number = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
                        valid = true;
                    }
                    catch (Exception)
                    {
                        number = 0m;
                        valid = false;
                    }
                }

                if (valid)
                    return number.ToString(
                        column.Type == "man-day" ? "0.000000" : "#,##0.######",
                        CultureInfo.InvariantCulture);
            }

            return Text(row, column.Key);
        }

        private static object Value(DataRow row, string key)
        {
            if (row == null || row.Table == null ||
                !row.Table.Columns.Contains(key) || row.IsNull(key))
                return null;

            return row[key];
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitElementBipChecker.Model
{
    public static class ExcelUtils
    {
        /// <summary>
        /// Save Report To File Csv
        /// </summary>
        /// <param name="dt">datatable</param>
        /// <param name="path">output path of file</param>
        /// <param name="filename">filename want save</param>
        public static void OpenExcel(this DataTable dt, out string path, string filename = "Report.csv")
        {
            string PathDocument = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string namesave;
            if (filename.ToLower().Contains(".csv"))
            {
                namesave = filename;
            }
            else
            {
                namesave = filename + ".csv";
            }
            path = Path.Combine(PathDocument, namesave);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var lines = new List<string>();

            string[] columnNames = dt.Columns
                .Cast<DataColumn>()
                .Select(column => column.ColumnName)
                .ToArray();

            var header = string.Join(",", columnNames.Select(name => $"\"{name}\""));
            lines.Add(header);

            var valueLines = dt.AsEnumerable()
                .Select(row => string.Join(",", row.ItemArray.Select(val => $"\"{val}\"")));

            lines.AddRange(valueLines);

            File.WriteAllLines(path, lines);
        }

        /// <summary>
        /// convert Data Object to data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(this IList<T> data)
        {
            PropertyDescriptorCollection props =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for(int i = 0 ; i < props.Count ; i++)
            {
                PropertyDescriptor prop = props[i];
                table.Columns.Add(prop.Name, prop.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;        
        }
       
        /// <summary>
        /// convert optimize data object to data table
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable2<T>(this IList<T> data)
        {
            PropertyDescriptorCollection properties = 
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }
    }
}

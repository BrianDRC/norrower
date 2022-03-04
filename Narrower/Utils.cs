using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Narrower
{
    public class Utils
    {
        public string FilePath { get; set; }
        public Utils() { }

        public void ConvertFile()
        {
            // Datatable object definition to ordering data
            DataTable table = new DataTable();

            // Reader object definition to read data from file provided
            StreamReader reader = ReadFile(FilePath);

            // Setting the table headers from file
            table = SetHeaders(table, reader.ReadLine());

            // Getting the values from file provided
            table = GetData(table, reader);

            // Closing reader object to free memory
            reader.Close();

            // Create a new .csv file and write the data in the new format
            WriteData(table);
        }

        /** 
         * 
         * Create a reader instance with the provided file loaded into memory
         * 
         * **/
        private StreamReader ReadFile(string path)
        {
            return new StreamReader(path);
        }

        /** 
         * 
         * Define the headers of the datatable to perform the ordering on a complex object
         * A positional value is used to define the first 5 fixed fields as flags according to
         * the file order and a second positional value to get the name of each
         * unique column removing the last char of each one that would be the identifier number
         * unnecessary for the case.
         * 
         * **/
        private DataTable SetHeaders(DataTable table, string headers)
        {
            table.Columns.Clear();
            List<string> headerText = new List<string>();
            headerText = headers.Split(',').ToList();

            for (int i = 0; i < headerText.Count; i++)
            {
                if(i <= 4)
                {
                    table.Columns.Add(headerText[i], typeof(string));
                }
                else
                {
                    i += 3;
                    table.Columns.Add(headerText[i].Remove(headerText[i].Length - 1, 1), typeof(string));
                }
            }
            return table;
        }

        /** 
         * 
         * Gets the data from the source file and manages how it is inserted
         * using a dynamic place value, to know which position is currently being read
         * Thus matching the number of jumps, starting from the position before each column
         *
         * Ex: To obtain the position of Name1 the previous position is used adding the mutator
         *
         * Previous position: 4
         * Mutator: 1
         * Actual position: 5 (4+1)
         *
         * This to perform dynamic loading by calculating the jumps and the current position of the reader.
         * Assuming that if it comes across a first position with an empty name, it jumps to the next one
         * since all that value will be empty.
         *  
         * **/
        private DataTable GetData(DataTable table, StreamReader reader)
        {
            int mutator = 1;
            int beforeNamePos = 4;
            int beforeAddressPos = 8;
            int beforeCityPos = 12;
            int beforeStatePos = 16;
            int beforeZipPos = 20;
            while (!reader.EndOfStream)
            {
                List<string> line = reader.ReadLine().Split(',').ToList();
                
                while(mutator <= 4)
                {
                    if (!String.IsNullOrEmpty(line[beforeNamePos + mutator]))
                    {
                        table.Rows.Add(new Object[] { line[0], line[1], line[2], line[3], line[4], line[beforeNamePos + mutator], line[beforeAddressPos + mutator], line[beforeCityPos + mutator], line[beforeStatePos + mutator], line[beforeZipPos + mutator] });
                    }
                    mutator++;
                }

                if(mutator >= 4)
                {
                    mutator = 1;
                }
                
            }
            return table;
        }

        /**
         * 
         * Writes the data and headers to a new .csv file in memory.
         * Convert the in-memory data table to a string using the (,) character as a separator for each column in the row
         * 
         * The generated file is saved in the /Resources folder in the compiled /bin
         * 
         * **/
        private void WriteData(DataTable table)
        {
            string filename = String.Format("MOCK_DATA_{0}.csv", DateTime.Now.ToString()).Replace('/', '_').Replace(" ", "_").Replace(":", "_");
            filename = "./Resources/" + filename;
            FileStream stream = new FileStream(filename, FileMode.CreateNew);

            StreamWriter writer = new StreamWriter(stream);

            string headers = GetHeadersText(table);

            writer.WriteLine(headers);

            string line = "";
            foreach (DataRow row in table.Rows)
            {
                line = String.Join(",", row.ItemArray);
                writer.WriteLine(line);
                line = "";
            }

            writer.Close();
        }

        /** 
         * 
         * Write the new headers from generated datatable in the first line of new file
         * 
         * **/

        private string GetHeadersText(DataTable table)
        {
            string headers = "";
            foreach (DataColumn column in table.Columns)
            {
                headers += column.ColumnName + ",";
            }

            headers = headers.Remove(headers.Length - 1, 1);

            return headers;
        }
    }
}

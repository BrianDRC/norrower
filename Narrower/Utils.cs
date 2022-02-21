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
            // Definición de la tabla ordenar los datos
            DataTable table = new DataTable();

            // Definición del reader para leer el archivo
            StreamReader reader = ReadFile(FilePath);

            // Setear los headers de la tabla
            table = SetHeaders(table, reader.ReadLine());

            // Obtener los valores del archivo
            table = GetData(table, reader);

            reader.Close();

            // Crea el archivo y escribe la data
            WriteData(table);
        }

        /** 
         * 
         * Crea la instancia del reader con el archivo especificado cargado en memoria.
         * 
         * **/
        private StreamReader ReadFile(string path)
        {
            return new StreamReader(path);
        }

        /** 
         * 
         * Define los headers del datatable para realizar el ordenamiento en un objeto complejo
         * Se usa un valor posicional para definir los primeros 5 campos fijos como flags según 
         * el orden del archivo y un segundo valor posicional para obtener el nombre de cada 
         * columna unica removiedo el ultimo char de cada uno que seria el numero identificador
         * innnecesario para el caso.
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
         * Obtiene los datos del archivo fuente y administra la forma en la que estos se insertan
         * usando un valor posicional dinamico, para saber que posición se está leyendo en el momento 
         * coinciendo así el número de saltos, partiendo de la posición antes de cada columna
         * 
         * Ej: Para obtener la posición de Name1 se usa la posición previa sumando el mutator
         * 
         *                  Posición previa:    4
         *                  Mutator:            1
         *                  Posición real:      5 (4+1)
         *                  
         *  Esto para realizar la carga dinámica calculando los saltos y la posición actual del reader. 
         *  Valiando que si se topa con alguna primera posición de nombre vacía salte a la siguiente
         *  ya que todo ese valor será vacío.
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
         * Escribe la información y los headers en un archivo csv en memoria.
         * Realiza la conversión de Datatable a String con el char (,) como
         * división entre colmna de cada row.
         * 
         * El archivo creado se guarda en la carpeta /Resources en los compilados /bin
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
         * Escribe los headers del Datatable en el archivo en la primera linea.
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

namespace Patient_Transferral_System.Services
{
    public class CsvDataReader
    {

        public List<string[]> GetDataFromCsv()
        {
            List<string[]> rows = new List<string[]>();
            
            string[] lines = File.ReadAllLines("Src/Files/patient_transferral_data_export.csv");
            
            // string currentDirectory = Directory.GetCurrentDirectory();
            //
            // string csvFilePath = Path.Combine(currentDirectory, "Src/Files/patient_transferral_data_export.csv");
            //
            // string[] lines = File.ReadAllLines(csvFilePath);

            foreach (string line in lines)
            {
                string[] fields = line.Split(',');
                rows.Add(fields);
            }

            return rows;
        }
    } 
}

namespace Patient_Transferral_System.Services
{
    public class CsvDataReader
    {

        public async Task<List<string>> GetDataFromCsv()
        {
            string url = "https://marcavans.blob.core.windows.net/solarch/fake_customer_data_export.csv?sv=2023-01-03&st=2024-06-14T10%3A31%3A07Z&se=2032-06-15T10%3A31%3A00Z&sr=b&sp=r&sig=q4Ie3kKpguMakW6sbcKl0KAWutzpMi747O4yIr8lQLI%3D";
            List<string> lines = new List<string>();
            
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    string result = await client.GetStringAsync(url);
                    lines = result.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            return lines;
        }
    } 
}

using System.IO;
namespace desert_auth.Class
{
    public class Log
    {
        public void Create(string text)
        {
            string path = Directory.GetCurrentDirectory() + @"\Logs";
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            Console.WriteLine(text);
            File.AppendAllLines(path + $@"\log {DateTime.Now.ToString("dd-MM-yyyy")}.txt", new string[] { text });
            
        }
       
        
    }
}

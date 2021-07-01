using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace HTML_Stat
{
    class Program
    {
        static void Main(string[] args)
        {
            Parser parser = new Parser();

            string urlAddress = "https://www.simbirsoft.com/";
            //Console.Write("Enter URL: ");
            //string urlAddress = Console.ReadLine();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = CopyAndClose(response.GetResponseStream());
                    
                    StreamReader readStream;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    string saveHtml_filePath = @"page.html";
                    SaveHtml(saveHtml_filePath, readStream);

                    receiveStream.Position = 0;
                    response.Close();
                    readStream.Close();

                    string parseData_filePath = @"unique_words.txt";
                    parser.ParseHtml(parseData_filePath, readStream);

                    ReadFileAndCountWords(parseData_filePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Stream CopyAndClose(Stream inputStream)
        {
            const int readSize = 1024;
            byte[] buffer = new byte[readSize];
            MemoryStream ms = new MemoryStream();

            int count = inputStream.Read(buffer, 0, readSize);
            while (count > 0)
            {
                ms.Write(buffer, 0, count);
                count = inputStream.Read(buffer, 0, readSize);
            }
            ms.Position = 0;
            inputStream.Close();
            return ms;
        }

        private static void SaveHtml(string filePath, StreamReader readStream)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(filePath, false, Encoding.Default);
                string htmlLine = "";
                while ((htmlLine = readStream.ReadLine()) != null)
                {
                    sw.WriteLine(htmlLine);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }        

        private static void ReadFileAndCountWords(string path)
        {
            try
            {
                Dictionary<string, int> words = new Dictionary<string, int>();
                using StreamReader sr = new StreamReader(path, Encoding.Default);
                string line = "";

                while ((line = sr.ReadLine()) != null)
                {
                    line = line.ToLower();
                    words.TryGetValue(line, out int value);
                    words[line] = value + 1;
                }             

                foreach (KeyValuePair<string, int> entry in words)
                {
                    Console.WriteLine(entry.Key + " - " + entry.Value);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
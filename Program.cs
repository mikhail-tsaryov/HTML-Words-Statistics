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
            string urlAddress = "https://www.simbirsoft.com/";
            //Console.Write("Enter URL: ");
            //string urlAddress = Console.ReadLine();

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Stream receiveStream = response.GetResponseStream();
                    StreamReader readStream;
                    if (response.CharacterSet == null)
                    {
                        readStream = new StreamReader(receiveStream);
                    }
                    else
                    {
                        readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                    }

                    //string saveHtml_filePath = @"page.html";
                    //SaveHtml(saveHtml_filePath, readStream);
                    
                    string parseData_filePath = @"unique_words.txt";
                    ParseHtml(parseData_filePath, readStream);
                    response.Close();
                    readStream.Close();

                    ReadFileAndCountWords(parseData_filePath);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
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

            private static void ParseHtml(string filePath, StreamReader readStream)
        {
            try
            {
                using StreamWriter sw = new StreamWriter(filePath, false, Encoding.Default);
                string line;
                bool nameTag_f = false;
                bool textBlock_f = false;
                bool tag_f = false;
                bool noTags_f = false;
                bool separatorsPrint_f = true;
                string tagBuffer = "";
                string prevTag = "";

                while ((line = readStream.ReadLine()) != null)
                {
                    foreach (char c in line)
                    {
                        if (nameTag_f)
                        {
                            if (c != ' ' & c != '>') tagBuffer += c;
                        }

                        if (c == '<')
                        {
                            tag_f = true;
                            nameTag_f = true;
                            textBlock_f = false;
                            tagBuffer = "";
                            prevTag = "";
                        }
                        else if ((c == '>' | c == ' ') & tag_f)
                        {
                            if (c == '>') tag_f = false;
                            nameTag_f = false;
                            prevTag = tagBuffer;

                            if (prevTag == "script" | prevTag == "style")
                            {
                                noTags_f = true;
                                continue;
                            }
                            else if (prevTag == "/script" | prevTag == "/style")
                            {
                                noTags_f = false;
                                continue;
                            }

                            if (c == '>' & !noTags_f)
                            {
                                textBlock_f = true;
                                continue;
                            }
                        }

                        if (textBlock_f)
                        {
                            if (c == '\r' |
                                c == '\n' |
                                c == '\t' |
                                c == ' ' |
                                c == ',' |
                                c == '.' |
                                c == '!' |
                                c == '?' |
                                c == '&' |
                                c == '"' |
                                c == ';' |
                                c == ':' |
                                c == '[' |
                                c == ']' |
                                c == '(' |
                                c == ')')
                            {
                                if (!separatorsPrint_f)
                                {
                                    sw.WriteLine();
                                }

                                separatorsPrint_f = true;
                            }
                            else
                            {
                                sw.Write(c);
                                separatorsPrint_f = false;
                            }
                        }
                    }
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
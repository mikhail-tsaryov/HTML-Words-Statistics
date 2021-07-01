using System;
using System.IO;
using System.Text;

namespace HTML_Stat
{
    class Parser
    {
        public void ParseHtml(string filePath, StreamReader readStream)
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
    }
}

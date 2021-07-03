using System;
using System.Collections.Generic;
using System.IO;

namespace HTML_Stat
{
    class Parser
    {
        private readonly HashSet<char> separators = new HashSet<char>(new char[] { '\r', '\n', '\t', ' ', ',', '.', '!', '?', '&', '"', ';', ':', '[', ']', '(', ')' });

        // Принцип выделения слов - просматриваем построчно и посимвольно дамп html-страницы,
        // определяем появление текстовых блоков и сохраняем.
        // При этом сразу заменяем разделители на сиволы переноса строк.
        // На выходе получаем список слов - каждое слово в отдельной строке.
        public void ParseHtml(StreamWriter sw, StreamReader readStream)
        {
            try
            {
                string line;
                bool nameTag_f = false;     
                bool textBlock_f = false;
                bool tag_f = false;
                bool noTags_f = false;
                bool separatorsPrint_f = true;
                string tagBuffer = "";
                string prevTag = "";

                // Читаем дамп по строкам и посимвольно
                while ((line = readStream.ReadLine()) != null)
                {
                    // Для каждого символа определяем к чему он относится - к тегу, к имени тега или к текстовому блоку
                    foreach (char c in line)
                    {
                        if (nameTag_f) // Сохраняем символы в буфер если обнаружили имя тега
                        {
                            if (c != ' ' & c != '>') tagBuffer += c;
                        }

                        if (c == '<')   // Считаем, что это начало открывающего или закрывающего тега
                        {
                            tag_f = true;
                            nameTag_f = true;
                            textBlock_f = false;
                            tagBuffer = "";
                            prevTag = "";
                        }
                        else if ((c == '>' | c == ' ') & tag_f) 
                        {
                            if (c == '>') tag_f = false;    // Обнаружили конец открывающего или закрывающего тега
                            nameTag_f = false;              // Имя тега закончилось, если встретился пробел при считывании имени тега
                            prevTag = tagBuffer;            // Фиксируем имя тега

                            // Теги script и style пропускаем, там нет полезного текста
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

                            // Конец тега, если это не теги-исключение, то считаем, что начинается текстовый блок
                            if (c == '>' & !noTags_f)
                            {
                                textBlock_f = true;
                                continue;
                            }
                        }

                        if (textBlock_f)
                        {
                            // Внутри текстового блока для каждого разделителя ставил перенос строки, но пропускаем подряд идущие несколько разделителей
                            if (separators.Contains(c))
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

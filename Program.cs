using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace TextGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 0; i < 15; i++)
            {
                //create list of lines
                List<string> lines = new List<string>(LineLister());

                //remove punctuation and upper case chars become lower?
                List<string> cleanLines = CleanText(lines);

                //convert each line into a list of Word class items
                List<Word> WordList = CreateWords(cleanLines);

                //parse together string
                string phrase = StringMaker(WordList);

                Console.WriteLine(phrase);
            }
            Console.ReadKey();
        }

        

        public static List<string> LineLister()
        {
            string line;
            List<string> listOfLines = new List<string>();

            StreamReader file = new StreamReader("../../../tweets.txt");
            while((line = file.ReadLine()) != null)
            {
                listOfLines.Add(line);
            }
            return listOfLines;
        }

        static List<string> CleanText(List<string> lines)
        {
            List<string> cleanLines = new List<string>();
            foreach(string x in lines)
            {
                string y = x.ToLower();
                var sb = new StringBuilder();
                foreach(char c in y)
                {
                    if (!char.IsPunctuation(c) || c == '\'')
                    {
                        sb.Append(c);
                    }
                }
                cleanLines.Add(sb.ToString());
            }
            return cleanLines;
        }

        static List<Word> CreateWords(List<string> lines)
        {
            List<Word> wordList = new List<Word>();
            foreach(string line in lines)
            {
                string[] splitLine = line.Split(' ');
                for (int i = 0; i < splitLine.Length-1; i++)
                {

                    //is first word
                    if (i ==0)
                    {
                        if (!wordList.Exists(x => x.text == splitLine[i]))
                        {
                            wordList.Add(new Word() { text = splitLine[i], isFirst = true, isLast = false });
                            var found = wordList.FindIndex(x => x.text == splitLine[i]);
                            wordList[found].followedby.Add(new Word() { text = splitLine[i + 1], isLast = false, isFirst = false });
                        }
                        
                        else
                        {
                            var found = wordList.FindIndex(x => x.text == splitLine[i]);
                            wordList[found].followedby.Add(new Word() { text = splitLine[i + 1], isLast = false, isFirst = false });
                        }
                    }
                    //is second to last word
                    else if (i == splitLine.Length - 2)
                    {
                        if (!wordList.Exists(x => x.text == splitLine[i]))
                        {
                            wordList.Add(new Word() { text = splitLine[i], isLast = false, isFirst = false });
                            var found = wordList.FindIndex(x => x.text == splitLine[i]);
                            wordList[found].followedby.Add(new Word() { text = splitLine[i + 1], isLast = true, isFirst = false });
                            wordList.Add(new Word() { text = splitLine[i + 1], isLast = true, isFirst = false });
                        }
                        else
                        {
                            var found = wordList.FindIndex(x => x.text == splitLine[i]);
                            wordList[found].followedby.Add(new Word() { text = splitLine[i + 1], isLast = true, isFirst = false });
                            wordList.Add(new Word() { text = splitLine[i + 1], isLast = true, isFirst = false });
                        }
                    }
                    else
                    {
                        if (!wordList.Exists(x => x.text == splitLine[i]))
                        {
                            wordList.Add(new Word() { text = splitLine[i], isFirst = false, isLast = false });
                            var found = wordList.FindIndex(x => x.text == splitLine[i]);
                            wordList[found].followedby.Add(new Word() { text = splitLine[i + 1], isLast = false, isFirst = false });
                        }
                        else
                        {
                            var found = wordList.FindIndex(x => x.text == splitLine[i]);
                            wordList[found].followedby.Add(new Word() { text = splitLine[i+1], isLast = false, isFirst = false });
                        }
                    }
                }
            }
            return wordList;
        }

        private static string StringMaker(List<Word> wordList)
        {
            Random r = new Random();
            Word currentWord = new Word();
            StringBuilder s1 = new StringBuilder();
            StringBuilder s2 = new StringBuilder();
            List<Word> firstWords = wordList.Where(x => x.isFirst == true).ToList();
            Word start = firstWords[r.Next(0, firstWords.Count)];
            for (int i = 0; i < start.text.Length; i++)
            {
                if (i == 0) s1.Append(start.text[i].ToString().ToUpper());
                else s1.Append(start.text[i]);
            }

            s2.Append(s1.ToString() + ' ');
            s1.Clear();
            currentWord = start;

            
                while (!currentWord.isLast)
                {
                try
                {
                    string next = currentWord.followedby[r.Next(0, currentWord.followedby.Count)].text;
                    Word nextWord = wordList.First(x => x.text == next);
                    s2.Append(nextWord.text + ' ');
                    currentWord = nextWord;
                }
                catch
                {
                    break;
                }
                }

                return s2.ToString().Trim() + '.';
            
            

            }
    }

    public class Word
    {
        public string text;
        public List<Word> followedby = new List<Word>();
        public bool isFirst;
        public bool isLast;
    }
}

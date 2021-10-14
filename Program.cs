/*
 * Important: This program does not take in the arguments from the Main(string args[]), instead the program uses 
 *            an infinite loop with each iteration taking in a new set of arguments. To run the program simply 
 *            call the dotnet run command from the command line. For more help us the -help command when the 
 *            program is runing. The program may be closed at any time using the -kill command.
 *            
 * Operating System : Windows 10
 * IDE : Visual Studio 2019
 * Author: Conor Banville, SN 18383803
 * 
 * 
 * This program will take in a file and convert it to another file type. When converting, this propgram will first convert the input
 * file to a 2D array, which in turn will then be converted to the requested file type.
 * 
 * All input files must be placed inside the files folder contained within this project, the coresponding output file can be found 
 * in the output folder.
 * 
 * Note from student;
 *      I don't belive I adopted an object orientated approach. If I was to re-do this assignment I would structure it as to have each 
 *      file type be handled in its own class. For example, CSV would have an associated class that would include methods for converting
 *      a csv file to an array and a method for converting the array back to csv. This way I would have a modular solution that could 
 *      easily be expanded upon. For instance if I wanted to handle LaTeX tables I would simply add a new class to handle it. Unfortunately 
 *      I ran out of time before I could convert my solution.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;

namespace Tabconv
{
    class Program
    {
        /*
         * First the main method prints a welcome message and informs the user of the of how to access the help screen.
         * Then a line is read in from the user, this line is the broken up into and action and arguments and stored in strings.
         *      example: -v -i <file1.input> -o <file2.output>
         *      action = "-v", arguments = "-i <file1.input> -o <file2.output>", please note the spaces around the '<' and '>'.
         * After this has been done the two new strings are passed to the Parse(string, string) method to be further broked down
         * and acted upon.
         * 
         * The Main method will then wait for an integer to be returned from the Parse method, this int will be either a 0 or a 1.
         * 0 indicates the line has been parsed and acted upon and the program should now wait for another command, if a 1 is 
         * returned this indicated the user wishes to to quit the program and the loop will be broken, closing the program.
         * 
         */
        static void Main(string[] args)
        {
            Console.WriteLine("\n<-------------------------  Tabconv  ------------------------->\n");
            Console.WriteLine("Welcome to Tabconv! A simple file conversion program.\nTo get started type \"-h\" for a list of commands.");
            Console.WriteLine("---------------------------------------------------------------\n");

            while (true)
            {
                string input = Console.ReadLine();
                int ParseValue;

                if (input.Contains(' '))
                {
                    string action = input.Substring(0, input.IndexOf(' '));
                    string arg = input.Substring(input.IndexOf(' ') + 1, input.Length - (input.IndexOf(' ') + 1));
                    ParseValue = Parse(action, arg);
                }
                else
                {
                    ParseValue = Parse(input, "");
                }

                if (ParseValue == 0)
                {
                    Console.WriteLine("Program Closing ...");
                    break;
                }
            }

            Console.ReadKey();
        }

        /*
         * This method will take in an action and possibly arguments. The action will then be used in a switch to decide what should be done.
         * eg. print help screen or convert a file ect.
         * 
         * When verbose has been selected a little more work is needed. The remnaining arguments will need to be broken down, and an input file 
         * location and a output file name/type will need to be extracted. This is done using Substring methods.
         * When these have been extracted they will be passed to the Convert method to be converted into the apropriate files.
         */
        static int Parse(string action, string arg)
        {
            switch (action)
            {
                case "-h":
                    PrintHelp();
                    return 1;

                case "-help":
                    PrintHelp();
                    return 1;

                case "-l":
                    PrintList();
                    return 1;

                case "-list-formats":
                    PrintList();
                    return 1;

                case "-i":
                    PrintInfo();
                    return 1;

                case "-info":
                    PrintInfo();
                    return 1;

                case "-k":
                    return 0;

                case "-kill":
                    return 0;

                case "-v":
                    try
                    {
                        string inputFile = "";
                        string outputFile = "";
                        for (int i = 0; i < arg.Length; i++)
                        {
                            //If the input file path was given first
                            if (arg[i] == '-' && arg[i + 1] == 'i')
                            {
                                //Extract the input file path
                                inputFile = arg.Substring(arg.IndexOf('<') + 1, arg.IndexOf('>') - (arg.IndexOf('<') + 1));
                                //Trim the remaining argument
                                arg = arg.Substring(arg.IndexOf("-o"), arg.Length - (arg.IndexOf("-o")));
                                //Extract the output file path
                                outputFile = arg.Substring(arg.IndexOf('<') + 1, arg.IndexOf('>') - (arg.IndexOf('<') + 1));
                                //Pass the files paths onto the converter
                                Convert(inputFile, outputFile);
                            }

                            //If the input file path was given first
                            else if (arg[i] == '-' && arg[i + 1] == 'o')
                            {
                                //Extract the output file path
                                outputFile = arg.Substring(arg.IndexOf('<') + 1, arg.IndexOf('>') - (arg.IndexOf('<') + 1));
                                //Trim the remaining argument
                                arg = arg.Substring(arg.IndexOf("-i"), arg.Length - (arg.IndexOf("-i")));
                                //Extract the input file path
                                inputFile = arg.Substring(arg.IndexOf('<') + 1, arg.IndexOf('>') - (arg.IndexOf('<') + 1));
                                //Pass the files paths onto the converter
                                Convert(inputFile, outputFile);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Sorry! Error parsing: {arg}");
                    }

                    return 1;

                case "-verbose":
                    try
                    {
                        string inputFile = "";
                        string outputFile = "";
                        for (int i = 0; i < arg.Length; i++)
                        {
                            if (arg[i] == '-' && arg[i + 1] == 'i')
                            {
                                inputFile = arg.Substring((arg.IndexOf('<') + 1), (arg.IndexOf('>') - (arg.IndexOf('<') + 1)));
                                arg = arg.Substring(arg.IndexOf('o'), arg.Length - arg.IndexOf('o'));
                                outputFile = arg.Substring((arg.IndexOf('<') + 1), (arg.IndexOf('>') - (arg.IndexOf('<') + 1)));
                                Convert(inputFile, outputFile);
                            }

                            if (arg[i] == '-' && arg[i + 1] == 'o')
                            {
                                outputFile = arg.Substring((arg.IndexOf('<') + 1), (arg.IndexOf('>') - (arg.IndexOf('<') + 1)));
                                arg = arg.Substring(arg.IndexOf('i'), arg.Length - arg.IndexOf('i'));
                                inputFile = arg.Substring((arg.IndexOf('<') + 1), (arg.IndexOf('>') - (arg.IndexOf('<') + 1)));
                                Convert(inputFile, outputFile);
                            }
                        }
                    }
                    catch
                    {
                        Console.WriteLine($"Sorry! Error parsing {arg}");
                    }

                    return 1;

                default:
                    Console.WriteLine("Sorry I did'nt understand your input!");
                    return 1;
            }
        }

        /*
         * The convert method consists of two switches. The first will decide what type of file we are trying to convert and call the apropriate 
         * method, the method that is called will then return the contents of the file inside a 2D string array. The second will decide what type 
         * of file we will convert to and also call the apropriate method, passing it the 2D array.
         */
        static void Convert(string inputFile, string outputFile)
        {
            //string[,] arrayForm = null;

            try
            {
                string inputExt = GetExtension(inputFile);
                string outputExt = GetExtension(outputFile);

                if (inputExt == null)
                {
                    Console.WriteLine($"File extension not specified for input file: {inputFile}");
                    return;
                }
                if (outputExt == null)
                {
                    Console.WriteLine($"File extension not specified for output file: {outputFile}");
                    return;
                }

                string[,] ArrayForm = null;

                switch (inputExt)
                {
                    case ".csv":
                        ArrayForm = CSVToArray(inputFile);
                        //PrintArray(ArrayForm);
                        break;

                    case ".md":
                        ArrayForm = MDToArray(inputFile);
                        //PrintArray(ArrayForm);
                        break;

                    case ".json":
                        ArrayForm = JSONToArray(inputFile);
                        //PrintArray(ArrayForm);
                        break;

                    case ".html":
                        ArrayForm = HTMLToArray(inputFile);
                        //PrintArray(ArrayForm);
                        break;

                    default:
                        Console.WriteLine($"Sorry {inputExt} is not supported!");
                        break;
                }

                outputFile = outputFile.Substring(0, outputFile.IndexOf('.'));

                switch (outputExt)
                {
                    case ".csv":
                        ArrayToCSV(ArrayForm, outputFile);
                        break;

                    case ".md":
                        ArrayToMD(ArrayForm, outputFile);
                        break;

                    case ".json":
                        ArrayToJSON(ArrayForm, outputFile);
                        break;

                    case ".html":
                        ArrayToHTML(ArrayForm, outputFile);
                        break;

                    default:
                        Console.WriteLine($"Sorry {outputExt} is not supported!");
                        break;
                }
            }
            catch //(Exception e)
            {
                //Console.WriteLine(e);
                Console.WriteLine("Error: Could'nt find specified file!");
            }
        }

        
        
        /*
         * This method will take in a 2D string array and convert it to html. When finished it will then save this html in a file specified 
         * by the fileName argument passed.
         */
        static void ArrayToHTML(string[,] ArrayForm, string FileName)
        {
            string html = "<table>\n\t<tr>\n";

            for (int i = 0; i < ArrayForm.GetLength(1); i++)
            {
                html += $"\t\t<th>{ArrayForm[0, i]}</th>\n";
            }

            html += "\t</tr>";

            for (int i = 1; i < ArrayForm.GetLength(0); i++)
            {
                html += "\n\t<tr>\n";

                for (int j = 0; j < ArrayForm.GetLength(1); j++)
                {
                    html += $"\t\t<td>{ArrayForm[i, j]}</td>\n";
                }

                html += "\t</tr>";
            }

            html += "\n</table>";

            File.WriteAllText($".\\output\\{FileName}.html", html);
            Console.WriteLine($"File has been saved at \"./output/{FileName}.html\"\n");

        }
        
        /*
         * This method will take in a 2D string array and convert is to Json. When finished it will then save this html in a file specified 
         * by the fileName argument passed in.
         */
        static void ArrayToJSON(string[,] ArrayForm, string FileName)
        {
            string[] headers = new string[ArrayForm.GetLength(1)];

            for (int i = 0; i < headers.Length; i++)
            {
                headers[i] = ArrayForm[0, i].Replace("\n", "").Replace("\t", "");
            }

            string json = "[\n\t{\n";

            for (int i = 1; i < ArrayForm.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayForm.GetLength(1); j++)
                {
                    if (isNumber(ArrayForm[i, j]))
                    {
                        json += $"\t\t\"{headers[j]}\":{ArrayForm[i, j].Replace("\n", "").Replace("\t", "")},\n";
                    }
                    else
                    {
                        json += $"\t\t\"{headers[j]}\":\"{ArrayForm[i, j].Replace("\n", "").Replace("\t", "")}\",\n";
                    }
                }
                json += "\n\t},\n";
            }

            json = json.Substring(0, json.Length - 1);
            json += "\n]";

            File.WriteAllText($".\\output\\{FileName}.json", json);
            Console.WriteLine($"File has been saved at \"./output/{FileName}.json\"\n");
        }
        /*
        * This method will take in a 2D string array and convert is to MD. When finished it will then save this html in a file specified 
        * by the fileName argument passed in.
        */
        static void ArrayToMD(string[,] ArrayForm, string FileName)
        {
            string MD = "";

            //Add the headers
            for (int i = 0; i < ArrayForm.GetLength(1); i++)
            {
                MD += $"|{ArrayForm[0, i]}";
            }

            MD += "|\n";

            //Adding the row of dashes, '-'
            int count = 0;

            for (int i = 0; i < ArrayForm.GetLength(1); i++)
            {
                count = ArrayForm[0, i].Length;
                MD += "|";
                for (int j = 0; j < count; j++)
                {
                    MD += "-";
                }
            }

            MD += "|\n";

            //Add the rest of the data
            for (int i = 1; i < ArrayForm.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayForm.GetLength(1); j++)
                {
                    MD += $"|{ArrayForm[i, j]}";
                }

                MD += "|\n";
            }

            File.WriteAllText($".\\output\\{FileName}.md", MD);
            Console.WriteLine($"File has been saved at \"./output/{FileName}.md\"\n");
        }
        
        /*
        * This method will take in a 2D string array and convert is to CSV. When finished it will then save this html in a file specified 
        * by the fileName argument passed in.
        */
        static void ArrayToCSV(string[,] ArrayForm, string FileName)
        {
            string csv = "";

            for (int i = 0; i < ArrayForm.GetLength(0); i++)
            {
                for (int j = 0; j < ArrayForm.GetLength(1); j++)
                {
                    csv += $"\"{ArrayForm[i, j]}\",";
                }

                csv = csv.Substring(0, csv.Length - 1);
                csv += "\n";
            }

            File.WriteAllText($".\\output\\{FileName}.csv", csv);
            Console.WriteLine($"File has been saved at \"./output/{FileName}.csv\"\n");
        }
       
        /*
         * This method will take in a html file location and then read in the file converting it to an array.
         * This array is then returned
         */
        static string[,] HTMLToArray(string HTMLFile)
        {
            string file = File.ReadAllText($".\\files\\{HTMLFile}");
            string[] splitFile = file.Split("</tr>");
            string[] splitFileTrim = new string[splitFile.Length - 1];

            for (int i = 0; i < splitFileTrim.Length; i++)
            {
                splitFileTrim[i] = splitFile[i];
            }

            //Extract headers from splitFileTemp[0]
            string[] temp = splitFileTrim[0].Split("<th");
            string[,] Array = new string[splitFileTrim.Length, temp.Length - 1];

            for (int i = 1; i < temp.Length; i++)
            {
                Array[0, i - 1] = temp[i].Substring(temp[i].IndexOf('>') + 1, temp[i].IndexOf('<') - (temp[i].IndexOf('>') + 1));
            }

            /*
             * Headers are now stored in Array
             * Need to extrac the data now
             *
             * 1. split each elm in splitFileTrim
             * 2. split temp and pull data out
             * 3. store in array
             */

            for (int i = 1; i < splitFileTrim.Length; i++)
            {
                temp = splitFileTrim[i].Split("<td");
                for (int j = 1; j < temp.Length; j++)
                {
                    Array[i, j - 1] = temp[j].Substring(temp[j].IndexOf(">") + 1, temp[j].IndexOf("</td>") - (temp[j].IndexOf(">") + 1));
                }
            }

            return Array;
        }

        /*
         * This method will take in a json file location and then read in the file converting it to an array.
         * This array is then returned
         */
        static string[,] JSONToArray(string JSONFile)
        {
            string data, header;
            string rawJSON = File.ReadAllText($".\\files\\{JSONFile}");
            rawJSON = rawJSON.Replace("[", "");
            rawJSON = rawJSON.Replace("]", "");
            string[] temp = rawJSON.Split("},");
            string[] json = new string[temp.Length - 1];

            for (int i = 0; i < json.Length; i++)
            {
                json[i] = temp[i];
            }

            int cols = 0;
            foreach (char c in json[0])
            {
                if (c == ':')
                {
                    cols++;
                }
            }
            string[,] Array = new string[json.Length + 1, cols];

            //Extract the headers
            json[0] = json[0].Replace("{", "");
            string[] s = json[0].Split(',');

            for (int i = 0; i < s.Length; i++)
            {
                header = s[i].Substring(0, s[i].IndexOf(":")).Replace("\"", "").Replace("\t","");
                data = s[i].Substring(s[i].IndexOf(":"), s[i].Length - s[i].IndexOf(":")).Replace("\"", "").Replace(":", "").Replace("\t","");
                Array[0, i] = header;
                Array[1, i] = data;
            }

            for (int i = 1; i < json.Length; i++)
            {
                s = json[i].Replace("{", "").Split(',');

                for (int j = 0; j < s.Length; j++)
                {
                    data = s[j].Substring(s[j].IndexOf(":"), s[j].Length - s[j].IndexOf(":")).Replace("\"", "").Replace(":", "");
                    Array[i + 1, j] = data;
                }
            }

            return Array;
        }

        /*
         * This method will take in a md file location and then read in the file converting it to an array.
         * This array is then returned
         */
        static string[,] MDToArray(string MDFile)
        {
            string[] tempFile = File.ReadAllLines($".\\files\\{MDFile}");
            string[] file = new string[tempFile.Length - 1];

            //Removes the 2nd row from the MD file as this hold no useful data
            file[0] = tempFile[0];
            for (int i = 2; i < tempFile.Length; i++)
            {
                file[i - 1] = tempFile[i];
            }

            //foreach (string elm in file)
            //{
            //    Console.WriteLine(elm);
            //}

            //Find the dimensions for our Array
            int rows = file.Length;
            int cols = -1;
            for (int i = 0; i < file[0].Length; i++)
            {
                if (file[0][i] == '|')
                {
                    cols++;
                }
            }
            //Set the dimensions of the new Array
            string[,] Array = new string[rows, cols];

            int count = 0;  //Track how many elements that have been added to the nth row in array
            string temp = ""; //Store each element temporarily in a string

            //Extract data from the MD file and store in an Array
            for (int i = 0; i < file.Length; i++)
            {
                count = 0;
                for (int j = 0; j < file[i].Length; j++)
                {
                    if (file[i][j] != '|')
                    {
                        temp += file[i][j];
                    }
                    else
                    {
                        if (temp != "")
                        {
                            Array[i, count] = temp;
                            temp = "";
                            count++;
                        }
                    }
                }
            }

            return Array;
        }

        /*
         * This method will take in a csv file location and then read in the file converting it to an array.
         * This array is then returned
         */
        static string[,] CSVToArray(string CSVFile)
        {
            string[] file = File.ReadAllLines($".\\files\\{CSVFile}");
            int rows = file.Count();
            int cols = 1;

            for (int i = 0; i < file[0].Length; i++)
            {
                if (file[0][i] == ',')
                {
                    cols++;
                }
            }

            string[,] Array = new string[rows, cols];

            string temp = "";   //temp string to store each elm
            //for each row in file[]
            for (int i = 0; i < file.Length; i++)
            {
                int count = 0;  //Track how many elements we have added
                //for each character in file[i]
                for (int j = 0; j < file[i].Length; j++)
                {
                    //Don't store the quotation mark
                    if (file[i][j] != '"' && file[i][j] != ',')
                    {
                        //Add each character to temp
                        temp += file[i][j];
                    }

                    ////If we have read the whole element
                    if (file[i][j] == ',' || j == file[i].Length - 1)
                    {
                        Array[i, count] = temp;    //Store the element in the array
                        count++;
                        temp = "";
                    }
                }
            }

            return Array;
        }

        
        //This method takes in a files path location and returns the files extension, eg. ".json"
        static string GetExtension(string path)
        {
            for (int i = path.Length - 1; i >= 0; i--)
            {
                if (path[i] == '.')
                {
                    return path.Substring(i, (path.Length - i));
                }
            }

            return null;
        }

        /*
         * This method takes in a 2D string array and prints the contents to the console indicating each elements loaction
         * within the array. This is for debuging purposes only and is never called in final production.
         */
        static void PrintArray(string[,] ArrayForm)
        {
            int rows = ArrayForm.GetLength(0);
            int cols = ArrayForm.GetLength(1);

            Console.Write("[");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{ArrayForm[i, j]},");
                }
                Console.WriteLine();
            }
            Console.Write("]");
        }

        //This method takes in a string and returns true if that string can be converted to a number.
        static bool isNumber(string s)
        {
            foreach (char c in s)
            {
                if (c < '0' || c > '9') return false;
            }

            return true;
        }

        //Prints the help screen
        static void PrintHelp()
        {
            Console.WriteLine("\n-v, -verbose\t\t\tVerbose mode (debugging output to STDOUT)");
            Console.WriteLine("-o <file>, -output <file>\tOutput file specified by <file>");
            Console.WriteLine("-l, -list-formats\t\tList formats");
            Console.WriteLine("-h, -help\t\t\tShow usage message");
            Console.WriteLine("-i, -info\t\t\tShow version information");
            Console.WriteLine("-k, -kill\t\t\tClose the program");
            Console.WriteLine("\nsample input:\t\t-v -i <my_cool_json.json> -o <converted_to_html.html>");
        }

        //Print the version info
        static void PrintInfo()
        {
            Console.WriteLine("\nVersion [0.0.1]\nAuthor: Conor Banville");
        }

        //Print the list of accepted file types
        static void PrintList()
        {
            Console.WriteLine("\nThese are the supported files types:\n");
            Console.WriteLine("HTML[.html]\tHyper Text Markup Language");
            Console.WriteLine("MD[.md]\t\tMark Down");
            Console.WriteLine("CSV[.csv]\tComma Separated Values");
            Console.WriteLine("JSON[.json]\tJavaScrpt Object Notation");
            Console.WriteLine("\nComing soon:\tLaTeX Table \x1");
        }
    }
}
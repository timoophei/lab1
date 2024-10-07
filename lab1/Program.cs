using System;
using System.Collections.Generic;
using System.IO;

namespace GeneticsProject
{
    public struct GeneticData
    {
        public string name; 
        public string organism;
        public string formula; 
    }

    class Program
    {
        static List<GeneticData> data = new ();
        
        static string GetFormula(string proteinName)
        {
            foreach (GeneticData item in data)
            {
                if (item.name.Equals(proteinName)) 
                    return item.formula;
            }
            return null;
        }

        static void ReadGeneticData(string filename)
        {
            using StreamReader reader = new StreamReader(filename);
            
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] fragments = line.Split('\t');

                GeneticData protein;
                protein.name = fragments[0];
                protein.organism = fragments[1];
                protein.formula = fragments[2];
                
                data.Add(protein);
            }
        }

        static void ReadHandleCommands(string inputFilename, string outputFilename)
        {
            using StreamReader reader = new StreamReader(inputFilename);
            using StreamWriter writer = new StreamWriter(outputFilename);

            writer.WriteLine("Tsimafei Heiba");
            writer.WriteLine("Genetic Searching");
            writer.WriteLine("--------------------------------------------------------------------------");

            //int counter = 0;
            for (var i=0; !reader.EndOfStream; i++)
            {
                string line = reader.ReadLine();
                //counter++;
                string[] command = line.Split('\t');
                
                writer.WriteLine($"{i.ToString("D3")}   {command[0]}   {string.Join("   ", command[1..])}");

                switch (command[0])
                {
                    case "search":
                        int index = Search(command[1]);
                        
                        writer.WriteLine("organism\t\t\tprotein");
                        
                        if (index != -1)
                        {
                            writer.WriteLine($"{data[index].organism}\t\t{data[index].name}");
                        }
                        else
                        {
                            writer.WriteLine("NOT FOUND");
                        }
                        
                        break;

                    case "diff":
                        int diffCount = Diff(command[1], command[2]);
                        writer.WriteLine("amino-acids difference: ");
                        if (diffCount != -1)
                        {
                            writer.WriteLine(diffCount.ToString());
                        }
                        else
                        {
                            writer.WriteLine("MISSING");
                        }
                        break;

                    case "mode":
                        string mostCommonAminoAcid = Mode(command[1]);
                        writer.WriteLine("amino-acid occurs: ");
                        if (mostCommonAminoAcid != null)
                        {
                            writer.WriteLine(mostCommonAminoAcid);
                        }
                        else
                        {
                            writer.WriteLine("MISSING");
                        }
                        break;

                    default:
                        writer.WriteLine("Unknown command");
                        break;
                }

                writer.WriteLine("--------------------------------------------------------------------------");
            }
        }

        static string Decoding(string formula)
        {
            string decoded = String.Empty;
            
            for (int i = 0; i < formula.Length; i++)
            {
                if (char.IsDigit(formula[i]))
                {
                    char letter = formula[i + 1];
                    int conversion = formula[i] - '0';
                    
                    for (int j = 0; j < conversion - 1; j++)
                    {
                        decoded += letter;
                    }
                }
                else decoded += formula[i];
            }
            return decoded;
        }

        static int Search(string amino_acid)
        {
            string decoded = Decoding(amino_acid);
            
            for (int i = 0; i < data.Count; i++)
            {
                if (data[i].formula.Contains(decoded)) return i;
            }
            
            return -1;
        }

        static int Diff(string protein1, string protein2)
        {
            string formula1 = Decoding(GetFormula(protein1));
            string formula2 = Decoding(GetFormula(protein2));

            if (formula1 == null || formula2 == null)
                return -1; 

            int minLength = Math.Min(formula1.Length, formula2.Length);
            int counter = 0;

            for (int i = 0; i < minLength; i++)
            {
                if (formula1[i] != formula2[i])
                {
                    counter++;
                }
            }

            counter += Math.Abs(formula1.Length - formula2.Length);

            return counter;
        }

        static string Mode(string proteinName)
        {
            string formula = GetFormula(proteinName);

            if (formula == null)
                return null;

            Dictionary<char, int> frequencyMap = new ();

            foreach (char aminoAcid in formula)
            {
                if (frequencyMap.ContainsKey(aminoAcid))
                {
                    frequencyMap[aminoAcid]++;
                }
                else
                {
                    frequencyMap[aminoAcid] = 1;
                }
            }

            char mostFrequentAminoAcid = '\0';
            int maxCount = 0;

            foreach (var kvp in frequencyMap)
            {
                if (kvp.Value > maxCount || (kvp.Value == maxCount && kvp.Key < mostFrequentAminoAcid))
                {
                    mostFrequentAminoAcid = kvp.Key;
                    maxCount = kvp.Value;
                }
            }

            return $"{mostFrequentAminoAcid}\t\t{maxCount}";
        }

        static void Main(string[] args)
        {
            int fileCount = 3;

            for (int i = 0; i < fileCount; i++)
            {
                ReadGeneticData($"sequences.{i}.txt");
                ReadHandleCommands($"commands.{i}.txt", $"genedata.{i}.txt");
            }

        }
    }
}
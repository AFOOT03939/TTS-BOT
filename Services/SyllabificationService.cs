using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace TTS_BOT.Services
{
    public class SyllabificationService
    {
        public List<string> textReciever(string text)
        {
            //var text = Console.ReadLine();

            string[] splitText;
            List<string> dirtySyllables = new List<string>();
            List<string> syllables = new List<string>();

            //separa las palabras
            splitText = splitWord(text);

            //hace la silabificación
            foreach (var split in splitText)
            {
                dirtySyllables.Add(syllabificate(split));
            }

            //limpia las [s]
            foreach (var dirtySyllab in dirtySyllables)
            {
                syllables.AddRange(dirtySyllab.Split("[s]", StringSplitOptions.RemoveEmptyEntries));
            }

            //Console.ForegroundColor = ConsoleColor.Red;

            //foreach (var sy in syllables)
            //{
            //    Console.WriteLine("Silaba\n" + sy);
            //}

            //Console.ForegroundColor = ConsoleColor.White;

            //_audioPlayerService.audioPlay(syllables);

            return syllables;
        }

        HashSet<char> vocals = new HashSet<char>
        {
            'a', 'e', 'i', 'o', 'u',
            'á', 'é', 'í', 'ó', 'ú',
            'ü'
        };

        HashSet<char> consonants = new HashSet<char>
        {
            'b', 'c', 'd', 'f', 'g', 'h',
            'j', 'k', 'l', 'm', 'n', 'ñ',
            'p', 'q', 'r', 's', 't', 'v',
            'w', 'x', 'y', 'z'
        };

        HashSet<string> consonantClusters = new HashSet<string>
        {
            "bl", "br", "cl", "cr",
            "dr", "fl", "fr", "gl",
            "gr", "pl", "pr", "tr",
            "tl"
        };

        HashSet<string> weakStrongDiphthongs = new HashSet<string>
        {
            "ia", "ie", "io",
            "ua", "ue", "uo"
        };

        HashSet<string> strongWeakDiphthongs = new HashSet<string>
        {
            "ai", "ei", "oi",
            "au", "eu", "ou"
        };

        HashSet<string> weakWeakDiphthongs = new HashSet<string>
        {
            "iu", "ui"
        };

        HashSet<string> strongStrongHiatus = new HashSet<string>
        {
            "aa", "ae", "ao",
            "ea", "ee", "eo",
            "oa", "oe", "oo",
            "aá", "aé", "aó",
            "eá", "eé", "eó",
            "oá", "oé", "oó",
            "áa", "áe", "áo",
            "éa", "ée", "éo",
            "óa", "óe", "óo"
        };

        HashSet<string> weakStrongHiatus = new HashSet<string>
        {
            "ía", "íe", "ío",
            "úa", "úe", "úo"
        };

        HashSet<string> strongWeakHiatus = new HashSet<string>
        {
            "aí", "eí", "oí",
            "aú", "eú", "oú"
        };

        HashSet<string> weakWeakHiatus = new HashSet<string>
        {
            "ii", "uu"
        };

        public string syllabificate(string text)
        {
            string syllables = "";

            List<int> syllablePositions = new List<int>();

            for (int i = 0; i < text.Length; i++)
            {
                //identifica el tipo de hiato y agrega el separador
                if ((i + 1 < text.Length) && (strongStrongHiatus.Contains($"{text[i]}{text[i + 1]}") || weakStrongHiatus.Contains($"{text[i]}{text[i + 1]}") || strongWeakHiatus.Contains($"{text[i]}{text[i + 1]}") || weakWeakHiatus.Contains($"{text[i]}{text[i + 1]}")))
                {
                    syllablePositions.Add(i);
                    syllablePositions.Add(i + 1);
                }

                //identifica el tipo de diptongo y agrega el separador
                if ((i + 1 < text.Length) && (weakStrongDiphthongs.Contains($"{text[i]}{text[i + 1]}") || strongWeakDiphthongs.Contains($"{text[i]}{text[i + 1]}") || weakWeakDiphthongs.Contains($"{text[i]}{text[i + 1]}")))
                {
                    if (!consonants.Contains(text[text.Length - 1]) && !(i == text.Length - 1))
                    {
                        syllablePositions.Add(i + 1);

                    }
                }

                //identifica todas las combinaciones silábicas en la separación de consonante-vocal

                //primero identifica una vocal
                if (vocals.Contains(text[i]))
                {
                    //recorre la palabra a partir de la siguiente letra
                    for (int j = i + 1; j < text.Length; j++)
                    {
                        //encuentra la siguiente vocal para crear un rango que abarca todas las consonantes de en medio
                        if (vocals.Contains(text[j]))
                        {
                            //una sílaba puede empezar de 2 maneras, con una consonante o en un grupo de 2 consonantes
                            //estos grupos ya están predeterminados, son una regla y están arriiba en el código
                            //por ejemplo: trans-por donde po es la separación consonante-vocal
                            //o por ejemplo: a-tra donde tr es el grupo de 2 consonantes que pueden empezar una sílaba
                            if ((j + 2 < text.Length) && (consonantClusters.Contains($"{text[j - 2]}{text[j - 1]}")))
                            {
                                syllablePositions.Add(j - 3);
                            }
                            else if (consonants.Contains(text[j - 1]))
                            {

                                if (text[j - 1] == 'r' && text[j - 2] == 'r')
                                {
                                    syllablePositions.Add(j - 3);

                                }
                                else if (text[j - 1] == 'h' && text[j - 2] == 's')
                                {
                                    syllablePositions.Add(j - 3);
                                }
                                else if (text[j - 1] == 'l' && text[j - 2] == 'l')
                                {
                                    syllablePositions.Add(j - 3);
                                }
                                else
                                {
                                    syllablePositions.Add(j - 2);
                                }
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < text.Length; i++)
            {
                syllables += text[i];

                if (syllablePositions.Contains(i))
                {
                    syllables += "[s]";
                }

            }

            return syllables;
        }

        public string[] splitWord(string text)
        {
            return Regex.Split(text, @"( )")
                        .Select(x => x == " " ? "¤" : x)
                        .ToArray();
        }

    }
}

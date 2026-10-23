using System;
using System.Collections.Generic;
using System.Text;

namespace TTS_BOT.Services
{
    public class SyllabificationService
    {
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
            string currentWord = "";
            string currentVocalWord = "";
            string currentConsonantWord = "";

            List<int> syllablePositionsConsonants = new List<int>();
            List<int> syllablePositionsVocals = new List<int>();
            List<int> syllablePositionsDiphthongs = new List<int>();
            List<int> syllablePositionsHiatus = new List<int>();
            List<int> syllablePositionsConsonantsClusters = new List<int>();

            //este for recorre todo el string de la palabra sabiendo que text[i] es un character
            for (int i = 0; i < text.Length; i++)
            {
                currentWord += text[i];

                if ((i + 1 < text.Length) && (strongStrongHiatus.Contains($"{text[i]}{text[i + 1]}") || weakStrongHiatus.Contains($"{text[i]}{text[i + 1]}") || strongWeakHiatus.Contains($"{text[i]}{text[i + 1]}") || weakWeakHiatus.Contains($"{text[i]}{text[i + 1]}")))
                {
                    syllablePositionsHiatus.Add(i);
                    syllablePositionsHiatus.Add(i + 1);
                }

                //agrega el diptongo al principio de la palabra: eu-ro-pa donde eu es el diptongo
                if ((i < 2) && (weakStrongDiphthongs.Contains($"{text[i]}{text[i + 1]}") || strongWeakDiphthongs.Contains($"{text[i]}{text[i + 1]}") || weakWeakDiphthongs.Contains($"{text[i]}{text[i + 1]}")))
                {
                    syllablePositionsDiphthongs.Add(i + 1);
                }

                //esta condición evalúa cuando la vocal empieza sola luego de dos consonantes específicas
                //el ejemplo sería a-bra-zar
                if ((i + 2 < text.Length) && consonantClusters.Contains($"{text[i]}{text[i + 1]}"))
                {
                    syllablePositionsConsonantsClusters.Add(i - 1);

                    //comprobación de diptongos
                    if ((i + 3 < text.Length) && (weakStrongDiphthongs.Contains($"{text[i + 2]}{text[i + 3]}") || strongWeakDiphthongs.Contains($"{text[i + 2]}{text[i + 3]}") || weakWeakDiphthongs.Contains($"{text[i + 2]}{text[i + 3]}")))
                    {
                        syllablePositionsDiphthongs.Add(i + 3);
                    }
                    else
                    {
                        syllablePositionsConsonantsClusters.Add(i + 2);
                    }

                    //syllablePositionsConsonants.Add(i+3);
                }

                //consonantes entre vocales bim-bo donde i-m-b-o
                if ((i + 1 < text.Length) && consonants.Contains(text[i]) && consonants.Contains(text[i + 1]) && !consonantClusters.Contains($"{text[i]}{text[i + 1]}"))
                {
                    if (vocals.Contains(text[i - 1]) && vocals.Contains(text[i + 2]))
                    {
                        if (text[i] == 'r' && text[i + 1] == 'r')
                        {
                            syllablePositionsConsonants.Add(i - 1);

                        } else if(text[i] == 's' && text[i + 1] == 'h')
                        {
                            syllablePositionsConsonants.Add(i + 1);
                        }
                        else
                        {
                            syllablePositionsConsonants.Add(i);
                        }
                    }
                }

                //vocales entre consonantes ca - sa
                //if (consonants.Contains(text[i - 1]) && vocals.Contains(text[i]))
                if ((i + 2 < text.Length) && (vocals.Contains(text[i]) && consonants.Contains(text[i + 1]) && vocals.Contains(text[i + 2])))
                {
                    //comprobación de diptongos
                    if ((i + 1 < text.Length) && (weakStrongDiphthongs.Contains($"{text[i]}{text[i + 1]}") || strongWeakDiphthongs.Contains($"{text[i]}{text[i + 1]}") || weakWeakDiphthongs.Contains($"{text[i]}{text[i + 1]}")))
                    {
                        syllablePositionsDiphthongs.Add(i + 1);
                    }
                    else
                    {
                        syllablePositionsVocals.Add(i);
                    }

                    //esto es un rollback por si encuentra un consonantCluster
                    //ejemplo: plato, donde "la" pasaría la regla, pero "pla" es la regla real
                    if ((i >= 2) && consonantClusters.Contains($"{text[i - 2]}{text[i - 1]}"))
                    {
                        syllablePositionsVocals.RemoveAt(syllablePositionsVocals.Count - 1);
                    }
                }

                //esto aplica para si la ultima letra de la palabra es consonante
                //se mete esa consonante a la ultima sílaba
                if (consonants.Contains(text[text.Length - 1]) && (i == text.Length - 1))
                {
                    if (syllablePositionsVocals.Count != 0)
                    {
                        syllablePositionsVocals.RemoveAt(syllablePositionsVocals.Count - 1);
                        syllablePositionsVocals.Add(i - 3);
                    }

                    if (syllablePositionsDiphthongs.Count != 0)
                    {
                        syllablePositionsDiphthongs.RemoveAt(syllablePositionsDiphthongs.Count - 1);
                        syllablePositionsDiphthongs.Add(i - 4);
                    }

                    if (syllablePositionsHiatus.Count != 0)
                    {
                        syllablePositionsHiatus.RemoveAt(syllablePositionsHiatus.Count - 1);
                    }
                    
                }
            }

            for (int i = 0; i < text.Length; i++)
            {
                syllables += text[i];

                if (syllablePositionsHiatus.Contains(i))
                {
                    syllables += "[s]";
                }

                if (syllablePositionsDiphthongs.Contains(i))
                {
                    syllables += "[s]";
                }

                if (syllablePositionsConsonantsClusters.Contains(i))
                {
                    syllables += "[s]";
                }

                if (syllablePositionsConsonants.Contains(i))
                {
                    syllables += "[s]";
                }

                if (syllablePositionsVocals.Contains(i))
                {
                    syllables += "[s]";
                }

            }

            string correctedSeparatedSyllables = removeExtraSeparator(syllables);

            return correctedSeparatedSyllables;
        }

        public string removeExtraSeparator(string text)
        {
            string separator = "[s]";

            for (int i = 0; i <= text.Length; i++)
            {
                if ((i + 5 < text.Length) && (separator.Contains($"{text[i]}{text[i + 1]}{text[i + 2]}") && separator.Contains($"{text[i + 3]}{text[i + 4]}{text[i + 5]}")))
                {
                    text = text.Remove(i + 3, 3);
                }
            }

            return text;
        }
    }
}

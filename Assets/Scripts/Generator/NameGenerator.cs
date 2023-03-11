using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public static class NameGenerator
{
    // Galaxy
    public static string GenerateGalaxyName()
    {
        int seedValue = GalaxyGenerator.Instance.Seed;
        int attemptCount = 0;
        int attemptMax = (galaxyAdjectiveArray.Length * galaxyNounArray.Length) / 2;
        string galaxyName = null;

        do
        {
            System.Random rand = new System.Random(seedValue);

            // Adjective
            int adjectiveSeed = rand.Next(0, galaxyAdjectiveArray.Length);

            // Noun
            int nounSeed = rand.Next(0, galaxyNounArray.Length + 1);
            string nounWord = null;

            if (nounSeed == galaxyNounArray.Length)
            {
                if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Spiral)
                {
                    if (GalaxyGenerator.Instance.Seed % 2 == 0) // Even
                    {
                        nounWord = "Whirlpool";
                    }
                    else // Odd
                    {
                        nounWord = "Gyre";
                    }
                }
                else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ellipitical)
                {
                    if (GalaxyGenerator.Instance.Seed % 2 == 0) // Even
                    {
                        nounWord = "Cluster";
                    }
                    else // Odd
                    {
                        nounWord = "Group";
                    }
                }
                else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
                {
                    if (GalaxyGenerator.Instance.Seed % 2 == 0) // Even
                    {
                        nounWord = "Ring";
                    }
                    else // Odd
                    {
                        nounWord = "Halo";
                    }
                }
            }
            else
            {
                nounWord = galaxyNounArray[nounSeed];
            }

            galaxyName = "The " + galaxyAdjectiveArray[adjectiveSeed] + " " + nounWord;

            seedValue++;
            attemptCount++;

            if (attemptCount > attemptMax)
            {
                Debug.Log("NameGenerator Error");
                break;
            }

        } while (IsGalaxyNameExistInSave(galaxyName));
        
        return galaxyName;
    }
    private static bool IsGalaxyNameExistInSave(string galaxyName)
    {
        List<string> saveFolderPathList = FileHelper.GetListOfFolderPaths(FileGalaxy.MotherSaveFolderPath, false);

        foreach (string saveFolderPath in saveFolderPathList)
        {
            if (Path.GetFileName(saveFolderPath) != FileGalaxy.CurrentSaveFolderName)
            {
                // Get List of files in Directory
                string[] saveFilePathArray = Directory.GetFiles(saveFolderPath);
                List<string> saveFilePathList = saveFilePathArray.ToList();

                if (saveFilePathList.Count > 0)
                {
                    if (galaxyName == FileGalaxy.ReadGalaxy(Path.GetFileName(saveFolderPath), Path.GetFileName(saveFilePathList[0])).Name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // Celestial
    public static string GenerateCelestialName(System.Random rand)
    {
        string name = null;

        int letterAmount = rand.Next(3, 9);

        int isVowelSeed = rand.Next(0, 2);
        bool isVowel = false;

        if (isVowelSeed == 1)
        {
            isVowel = true;
        }

        int loopCount = 0;

        while (loopCount < letterAmount)
        {
            if (isVowel)
            {
                string vowel = GetVowel(rand);

                name = name + vowel;
                isVowel = false;
            }
            else
            {
                string consonant = GetConsonant(rand);

                name = name + consonant;
                isVowel = true;
            }

            loopCount++;
        }

        name = CapitalizeName(name);

        if (IsProfanity(name))
        {
            name = GenerateCelestialName(rand);
        }

        return name;
    }

    // Letters
    private static string GetVowel(System.Random rand)
    {
        string[] vowelArray =
        {
            "a",
            "e",
            "i",
            "o",
            "u",
            "y",
        };

        int vowelSeed = rand.Next(0, 5);
        string vowel = vowelArray[vowelSeed];

        return vowel;
    }
    private static string GetConsonant(System.Random rand)
    {
        string[] consonantArray =
        {
            "b",
            "c",
            "d",
            "f",
            "g",
            "h",
            "j",
            "k",
            "l",
            "m",
            "n",
            "p",
            "q",
            "r",
            "s",
            "t",
            "v",
            "w",
            "x",
            "z",
        };

        int consonantSeed = rand.Next(0, 20);
        string consonant = consonantArray[consonantSeed];

        return consonant;
    }

    // Utility
    private static bool IsProfanity(string aName)
    {
        bool isProfanity = false;
        string name = aName.ToLower();

        foreach (string profanity in profanityArray)
        {
            if (name.Contains(profanity))
            {
                isProfanity = true;
            }
        }

        return isProfanity;
    }
    private static string CapitalizeName(string name)
    {
        char[] letters = name.ToCharArray();
        letters[0] = char.ToUpper(letters[0]);

        return new string(letters);
    }

    // Arrays
    private static string[] galaxyNounArray =
    {
        "Abyss",
        "Body",
        "Chasm",
        "Cosmos",
        "Creation",
        "Expanse",
        "Eye",
        "Galaxy",
        "Night",
        "Ocean",
        "One",
        "Plain",
        "Point",
        "Reality",
        "Region",
        "Rift",
        "Sea",
        "Skies",
        "Space",
        "Spirit",
        "Spot",
        "Stars",
        "Stretch",
        "Universe",
        "Void",
        "Way",
    };
    private static string[] galaxyAdjectiveArray =
    {
        "Abiding",
        "Aching",
        "Aging",
        "Ancient",
        "Beloved",
        "Bent",
        "Black",
        "Blind",
        "Boundless",
        "Breathing",
        "Bright",
        "Callous",
        "Calm",
        "Cavernous",
        "Ceaseless",
        "Changing",
        "Cloudy",
        "Cold",
        "Colorful",
        "Colossal",
        "Conscious",
        "Constant",
        "Dark",
        "Deep",
        "Definite",
        "Delicate",
        "Dense",
        "Dim",
        "Distant",
        "Enchanted",
        "Endurable",
        "Ethereal",
        "Exotic",
        "Faded",
        "Faint",
        "Fair",
        "Falling",
        "Fatherly",
        "Feeble",
        "Fertile",
        "First",
        "Fluid",
        "Frail",
        "Frigid",
        "Gaping",
        "Great",
        "Growing",
        "Halting",
        "Heavy",
        "Hidden",
        "Hollow",
        "Imperfect",
        "Infinite",
        "Invincible",
        "Jaded",
        "Aging",
        "Knowing",
        "Lasting",
        "Liquid",
        "Living",
        "Lonely",
        "Longing",
        "Lost",
        "Melodic",
        "Milky",
        "Motherly",
        "Murky",
        "Mysterious",
        "Narrow",
        "Observant",
        "Odd",
        "Old",
        "Pale",
        "Past",
        "Perfect",
        "Perpetual",
        "Present",
        "Quiet",
        "Radiant",
        "Raw",
        "Serene",
        "Shady",
        "Shallow",
        "Silent",
        "Silky",
        "Sparkling",
        "Stark",
        "Superior",
        "Thundering",
        "Treasured",
        "Trusting",
        "Vast",
        "Vibrant",
        "Vivid",
        "Wandering",
        "Wild",
        "Young",
    };
    private static string[] profanityArray =
    {
        "ass",
        "bitch",
        "cunt",
        "cum",
        "cock",
        "crap",
        "dick",
        "damn",
        "dyke",
        "fuck",
        "hell",
        "jesus",
        "kike",
        "nigga",
        "nigger",
        "piss",
        "prick",
        "penis",
        "pussy",
        "shit",
        "slut",
        "sex",
        "twat",
        "vagina",
        "wanker",
    };
}

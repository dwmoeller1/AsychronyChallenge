using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AsychronyChallenge
{
    class Program
    {
        static void Main(string[] args)
        {
            string stringToEncode = Console.ReadLine();

            string res = encode(stringToEncode);
        }

        public enum CharacterType
        {
            Vowel,
            Consonant,
            Y,
            Space,
            Digit,
            Other
        }

        public static string vowels = "aeiou";
        static string encode(string stringToEncode)
        {
            string output = string.Empty;
            stringToEncode = stringToEncode.ToLower();
            for (int i = 0; i< stringToEncode.Length; i++)
            {
                Encoder encoder = new Encoder(stringToEncode[i]);
                output += encoder.Encode();
                if (i == stringToEncode.Length - 1)
                    encoder.ProcessCurrentNumber();
            }

            return output;
        }

        class Encoder
        {
            private char charToEncode;
            private static string currentNumber = string.Empty;
            private string output = string.Empty;
            private CharacterType characterType;

            public Encoder(char charToEncode)
            {
                this.charToEncode = charToEncode;
                this.characterType = GetCharacterType();
            }

            public string Encode()
            {
                if (characterType != CharacterType.Digit || isLastCharacter)
                {
                    output += ProcessCurrentNumber();
                    output += ApplyCharacterRule();
                }
                else
                    currentNumber += ApplyCharacterRule();
                
                return output;
            }

            private CharacterType GetCharacterType()
            {
                CharacterTyper typer = new CharacterTyper(charToEncode);
                return typer.GetCharacterType();
            }

            public string ProcessCurrentNumber()
            {
                NumberStringEncodingRules encoderRules = new NumberStringEncodingRules();
                var rule = encoderRules.GetRule();
                string result = rule(currentNumber);
                currentNumber = string.Empty;
                return result;
            }

            private string ApplyCharacterRule()
            {
                CharacterTypeEncodingRules encoderRules = new CharacterTypeEncodingRules(characterType);
                var rule = encoderRules.GetRule();
                return rule(charToEncode);
            }
        }

        public class CharacterTyper
        {
            private char character;

            public CharacterTyper(char character)
            {
                this.character = character;
            }

            public CharacterType GetCharacterType()
            {
                //TODO: Come up with a more elegant way to do this - as is it will be more difficult than necessary to change if a new type was added
                //at the least, could probably use reflection for this but can't recall the specifics for calling methods using reflection
                if (IsDigit())
                    return CharacterType.Digit;
                else if (IsVowel())
                    return CharacterType.Vowel;
                else if (IsConsonant())
                    return CharacterType.Consonant;
                else if (character.Equals('y'))
                    return CharacterType.Y;
                else if (character.Equals(' '))
                    return CharacterType.Space;
                else
                    return CharacterType.Other;
            }

            private bool IsDigit()
            {
                return int.TryParse(character.ToString(), out int result);
            }

            private bool IsVowel()
            {
                return vowels.Contains(character);
            }

            private bool IsConsonant()
            {
                return !IsVowel() && character != 'y' && character <= 122 && character >= 97;
            }


        }

        class CharacterTypeEncodingRules
        {
            private CharacterType characterType;

            public CharacterTypeEncodingRules(CharacterType characterType)
            {
                this.characterType = characterType;
            }

            public Func<char, string> GetRule()
            {
                switch (characterType)
                {
                    case CharacterType.Vowel:
                        return VowelEncodingRule();
                    case CharacterType.Digit:
                        return DigitEncodingRules();
                    case CharacterType.Space:
                        return SpaceEncodingRule();
                    case CharacterType.Y:
                        return YEncodingRule();
                    case CharacterType.Consonant:
                        return ConsonantEncodingRule();
                    case CharacterType.Other:
                        return DefaultEncodingRule();
                    default:
                        return DefaultEncodingRule();
                }
            }

            private Func<char, string> DefaultEncodingRule()
            {
                return c => c.ToString();
            }

            private Func<char, string> ConsonantEncodingRule()
            {
                return c => ((char)(c - 1)).ToString();
            }

            private Func<char, string> YEncodingRule()
            {
                return c => " ";
            }

            private Func<char, string> SpaceEncodingRule()
            {
                return c => "y";
            }

            public Func<char, string> VowelEncodingRule()
            {
                return c => (vowels.IndexOf(c) + 1).ToString();
            }

            public Func<char, string> DigitEncodingRules()
            {
                return c => c.ToString();
            }
        }

        class NumberStringEncodingRules
        {
            public Func<string, string> GetRule()
            {
                return x => string.Join("", x.Reverse());
            }
        }
    }
}


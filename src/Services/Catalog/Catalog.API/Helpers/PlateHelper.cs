using System.Text;

namespace Catalog.API.Helpers;

public static class PlateHelper
{
 
    private static Dictionary<char, string> NumToLetterDict2 = new Dictionary<char, string>()
    {
        {'0', "O" },
        {'1', "IL" },
        {'2', "RZ" },
        {'3', "BE" },
        {'4', "A" },
        {'5', "S" },
        {'6', "BG" },
        {'7', "T" },
        {'8', "B" },
        {'9', "G" }
    };



    /// <summary>
    /// return only nos from reg
    /// </summary>
    /// <param name="registration"></param>
    /// <returns></returns>
    public static int GetDigits(string registration)
    {
        return int.TryParse(new string(registration.Where(c => char.IsDigit(c)).ToArray()), out var digits) ? digits : 0;
    }

    
    /// <summary>
    /// return first 3 letters, but potentially do num-to-letter conversion
    /// </summary>
    /// <param name="registration"></param>
    /// <returns></returns>
    public static string GetLetters(string registration, int thisMany = 3)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in registration.Take(thisMany))
        {
            sb.Append(GetTranslation(c));
        }

        return sb.ToString();
    }

    /// <summary>
    /// translate input 'num' to corresponding letter if translation exists
    /// (if multiple, return the first)
    /// otherwise return original char
    /// e.g. '3' -> 'E' , 'C' -> 'C' 
    /// </summary>
    /// <param name="c"></param>
    /// <returns></returns>
    public static char GetTranslation(char c)
    {
        return NumToLetterDict2.TryGetValue(c, out var newLetters) ? newLetters.First() : c;
    }


    private static List<char> GetDigitsFromLetter(char c)
    {
        var digits = NumToLetterDict2.Where(item => item.Value.Contains(char.ToUpper(c))).Select(i => i.Key).ToList();
        return digits;
    }

    /// <summary>
    /// return a list of possible plates 
    /// e.g. GSMITH -> GSM17H, 65M17H etc..
    /// </summary>
    /// <remarks>
    /// recursive method 
    /// initialise list with "reg".
    /// iterate thro' each char in "reg", create a new plate with that char substituted.
    /// upon each iteration, when creating new plate, it'll do substitutions on all previous new plates created too.
    /// </remarks>
    /// <param name="reg"></param>
    /// <returns></returns>
    public static List<string> GetCombinations(string reg, int pos = 0, List<string> results = null)
    {
        if (pos == 0)                                   //initial set up, clear list and add the reg 
            results = new List<string> { reg };

        if (pos >= reg.Length)                          //end of the line => return results
            return results;

        //below is where new combinations are created by substituting the character at pos in reg.

        var combinations = new List<string>();          //new list for new combos in this iteration

        var charAtPos = reg[pos];                       //get reg char at [pos]
        var digits = GetDigitsFromLetter(charAtPos);    //get alternative digits for this char

        foreach (var d in digits)                       //for every alternative digit (if any)
        {
            foreach (var r in results)                  //loop thro' each reg in results so far
            {
                var newReg = r.Replace(charAtPos, d);   //get new plate by making substitution of the given char with the replacement digit
                combinations.Add(newReg);               //add new plate to list (combinations)
            }
        }

        results.AddRange(combinations);                 //add these combos to overall results

        return GetCombinations(reg, pos + 1, results);  //make recursive call, moving on to substituting next char in reg
    }

}

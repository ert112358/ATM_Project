using System.Text.RegularExpressions;

namespace ATM_BackEnd;

public class Password
{
    /*
     * Source: https://regex101.com/library/0bH043
     *
     * This function returns true if all the following criteria are met:
     * password must contain 1 number (0-9)
     * password must contain 1 uppercase letters
     * password must contain 1 lowercase letters
     * password must contain 1 non-alpha numeric number
     * password is 8-16 characters with no space
     */

    private static string regex = "^(?=.*\\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^\\w\\d\\s:])([^\\s]){8,16}$";
    
    public static bool IsPasswordStrong(string password)
    {
        return Regex.IsMatch(password, regex);
    }
}
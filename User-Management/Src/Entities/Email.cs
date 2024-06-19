using System.Text.RegularExpressions;

namespace User_Management.Model;

public class Email
{
    public static void Validate(string email)
    {
        if (Regex.IsMatch(email, "^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$") == false)
        {
            throw new ArgumentException("Invalid email");
        }
    }
}

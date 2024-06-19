using System.ComponentModel.DataAnnotations;

namespace User_Management.Model;

public class RegisterUser: User
{
   public string name { get; set; }

   public int age { get; set; }

   public string bsn { get; set; }

   public string sex { get; set; }

}

using System.ComponentModel.DataAnnotations;

namespace User_Management.Model;

public class RegisterUser: User
{
   public string Name { get; set; }

   public int Age { get; set; }

   public string Bsn { get; set; }

   public string Sex { get; set; }

}

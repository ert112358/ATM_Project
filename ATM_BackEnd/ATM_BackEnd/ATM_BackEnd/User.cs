using System.ComponentModel.DataAnnotations;

namespace ATM_BackEnd;

public class User
{
    [Key]
    public string Name { get; set; }
    public string Digest { get; set; }
    public string Token { get; set; }
    public decimal Balance { get; set; }
}
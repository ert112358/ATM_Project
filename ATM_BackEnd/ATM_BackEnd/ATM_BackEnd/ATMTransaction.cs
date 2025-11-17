using System.ComponentModel.DataAnnotations;

namespace ATM_BackEnd;

public class ATMTransaction
{
    [Key]
    public string User { get; set; }
    public ATMTransactionType Type { get; set; }
    public int Amount { get; set; }
}
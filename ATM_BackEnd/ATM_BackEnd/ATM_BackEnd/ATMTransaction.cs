using System.ComponentModel.DataAnnotations;

namespace ATM_BackEnd;

public class ATMTransaction
{
    [Key] 
    public Guid Id { get; set; } = Guid.NewGuid();
    public ATMTransactionType Type { get; set; }
    public int Amount { get; set; }
    public string UserName { get; set; }
}
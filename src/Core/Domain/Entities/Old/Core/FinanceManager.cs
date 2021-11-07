using System;

public class FinanceManager // : Entity(Type Entity)
{
    // Needs to be linked to the branch or dealer depending if the dealer has a branch or not one or not
    // How to link this still to be determined
    public bool Active { get; set; }
    public string Email { get; set; }
    public string Name { get; set; } // Inherite from person
    public string Account { get; set; }
}

namespace backend.Domain;

public class User
{
    public int id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
    public int rolid { get; set; }

    // Navigatie-eigenschap voor Role (optioneel)
    public Role rol { get; set; }
    
}
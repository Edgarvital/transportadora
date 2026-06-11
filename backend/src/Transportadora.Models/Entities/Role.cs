using Transportadora.Models.Enums;

namespace Transportadora.Models.Entities;

public class Role
{
    public int Id { get; set; }
    public RoleTipo Nome { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
}

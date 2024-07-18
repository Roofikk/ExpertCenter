using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class VarCharColumn : ColumnValueBase
{
    [Column(TypeName = "varchar(50)")]
    public string Value { get; set; } = null!;
}

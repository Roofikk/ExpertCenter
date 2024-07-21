using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class ColumnType
{
    [Column(TypeName = "varchar(24)")]
    public string ColumnTypeId { get; set; } = null!;
    [Column(TypeName = "varchar(50)")]
    public string DisplayName { get; set; } = null!;

    public virtual ICollection<Column> Columns { get; set; } = [];
}

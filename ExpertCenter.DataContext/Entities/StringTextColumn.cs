﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ExpertCenter.DataContext.Entities;

public class StringTextColumn : ColumnValueBase
{
    [Column(TypeName = "nvarchar(max)")]
    public string Value { get; set; } = null!;
}

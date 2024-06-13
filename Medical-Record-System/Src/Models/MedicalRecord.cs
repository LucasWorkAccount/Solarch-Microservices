using System;
using System.Collections.Generic;

namespace Medical_Record_System;

public partial class MedicalRecord
{
    public int Id { get; set; }

    public Guid Uuid { get; set; }

    public string Name { get; set; } = null!;

    public int Age { get; set; }

    public string Sex { get; set; } = null!;

    public string Bsn { get; set; } = null!;

    public string Record { get; set; } = null!;
}

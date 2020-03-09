﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CourseGenerator.Models.Info.Entities
{
    public class MaterialType
    {
        public int Id { get; set; }

        public string Note { get; set; }

        public int? ParentId { get; set; }
        public MaterialType Parent { get; set; }
        public ICollection<MaterialType> MaterialTypes { get; set; }

        public ICollection<Material> Materials { get; set; }
    }
}

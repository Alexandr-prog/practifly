﻿using CourseGenerator.Api.Infrastructure.SwaggerFilters.Examples;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseGenerator.Api.Models
{
    /// <summary>
    /// ViewModel, що представляє мову в меню вибору мови
    /// </summary>
    [SwaggerSchemaFilter(typeof(LanguageSelectSchemaFilter))]
    public class LanguageSelectModel
    {
        /// <summary>
        /// Код мови.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Назва мовою оригіналу.
        /// </summary>
        public string Name { get; set; }
    }
}

﻿using CourseGenerator.Api.Infrastructure.SwaggerFilters.Examples;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        [DefaultValue("ua")]
        public string Code { get; set; }

        /// <summary>
        /// Назва мовою оригіналу.
        /// </summary>
        public string Name { get; set; }
    }
}

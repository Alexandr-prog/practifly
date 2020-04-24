﻿using CourseGenerator.Api.Infrastructure.SwaggerFilters.Examples;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CourseGenerator.Api.Models
{
    /// <summary>
    /// ViewModel, що представляє курс в меню вибору курсу
    /// </summary>
    [SwaggerSchemaFilter(typeof(CourseSelectSchemaFilter))]
    public class CourseSelectModel
    {
        /// <summary>
        /// Код курсу.
        /// </summary>
        [Required]
        public int Id { get; set; }

        /// <summary>
        /// Локалізована назва курсу.
        /// </summary>
        public string Name { get; set; }
    }
}

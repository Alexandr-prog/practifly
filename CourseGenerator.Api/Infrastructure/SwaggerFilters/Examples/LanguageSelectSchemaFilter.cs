﻿using CourseGenerator.BLL.DTO;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CourseGenerator.Api.Infrastructure.SwaggerFilters.Examples
{
    /// <summary>
    /// Додає приклад для <see cref="LanguageSelectDTO"/>
    /// </summary>
    public class LanguageSelectSchemaFilter : ISchemaFilter
    {
        /// <inheritdoc/>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            schema.Example = new OpenApiObject
            {
                { "Code", new OpenApiString("ENG") },
                { "Name", new OpenApiString("Англійська") }
            };
        }
    }
}

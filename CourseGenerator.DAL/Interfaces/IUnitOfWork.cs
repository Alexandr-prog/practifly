﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using CourseGenerator.Models.Entities.Identity;
using CourseGenerator.DAL.Repositories;
using CourseGenerator.Models.Entities.CourseAccess;
using CourseGenerator.Models.Entities.Info;

namespace CourseGenerator.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        ApplicationUserManager UserManager { get; set; }
        RoleManager<Role> RoleManager { get; set; }
        IGenericEFRepository<Language> LanguageRepository { get; set; }
        IThemeRepository ThemeRepository { get; set; }
        ICourseRepository CourseRepository { get; set; }
        IGenericEFRepository<UserCourse> UserCourseRepository { get; set; }

        Task SaveAsync();
    }
}

﻿using System.ComponentModel.DataAnnotations;

namespace LessonMigration.ViewModels.Account
{
    public class LoginVM
    {
        [Required,MaxLength(150)]
        public string UsernameorEmail { get; set; }
        [Required,DataType(DataType.Password)]
        public string Password { get; set; }

    }
}

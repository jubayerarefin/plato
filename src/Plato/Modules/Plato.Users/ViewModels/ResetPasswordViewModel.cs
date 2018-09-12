﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Plato.Users.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordConfirmation { get; set; }

        public bool IsValidResetToken { get; set; }

        public string ResetToken { get; set; }
    }
}

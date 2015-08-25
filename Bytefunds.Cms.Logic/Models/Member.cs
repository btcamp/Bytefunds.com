using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Umbraco.Web.Models;

namespace Bytefunds.Cms.Logic.Models
{
    public class MemberViewModel
    {
        public int  languageId { get; set; }
        [RegularExpression(@"[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?", ErrorMessage = "Please enter the correct email address.")]
        [Required(ErrorMessage = "Please enter the email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please specify a password.")]
        [StringLength(60, MinimumLength = 6, ErrorMessage = "Your password must be at least 6 characters.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please specify a password.")]
        [Compare("Password", ErrorMessage = "Passwords did not match. Please try again.")]
        public string ConfirmPassword { get; set; }
    }
}
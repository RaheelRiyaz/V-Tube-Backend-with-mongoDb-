using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstApp.Application.Validators
{
    public class VideoValidation (params string[] extensions) : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if(value is IFormFile file)
            {
                return extensions.Contains(file.ContentType.ToLower());
            }
            return false;
        }
    }
}

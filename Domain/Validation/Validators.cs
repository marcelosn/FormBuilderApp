﻿using Domain.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain
{
    public delegate ValidationError Validator(string fieldKEy, JToken serializedValue);
    public class Validators
    {
        public static Validator Empty => (k, v) => null;
        public static Validator RequiredText => (k, v)
            => string.IsNullOrWhiteSpace(v?.Value<string>()) ? new ValidationError(k, "is empty") : null;
        public static Validator MinLength(int? minlength)
            => minlength.HasValue
                ? (k, v) => (v?.Value<string>()?.Length < minlength) ? new ValidationError(k, "too short") : null
                : Empty;
        public static Validator MaxLength(int? maxlength)
            => maxlength.HasValue
                ? (k, v) => v?.Value<string>()?.Length > maxlength ? new ValidationError(k, "too long") : null
                : Empty;
        public static Validator Combine(params Validator[] validators) => (k, v)
            => validators.Select(x => x(k, v)).FirstOrDefault(err => err != null);
    }
}
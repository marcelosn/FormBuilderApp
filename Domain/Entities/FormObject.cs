﻿using Domain.Exceptions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Domain.Entities
{
    public class FormObject : BaseEntity
    {
        private Dictionary<string, object> values = new Dictionary<string, object>();
        public Guid FormDefinitionId { get; }
        public IReadOnlyDictionary<string, object> Values =>
            new ReadOnlyDictionary<string, object>(values);
        public FormObject(FormDefinition metadata, JObject values)
        {
            if (metadata == null)
            {
                throw new ArgumentNullException(nameof(metadata));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            this.FormDefinitionId = metadata.Id;
            foreach (var fieldDefition in metadata.FieldDefinitions)
            {
                var specified=values.TryGetValue(fieldDefition.FieldKey, out var value);
                var validationError = fieldDefition.Validate(value);
                if (validationError != null)
                {
                    throw new ValidationException(validationError);
                }
                            
                this.values.Add(fieldDefition.FieldKey, value);
            }
            metadata.IncreaseObjectCount();
        }
    }
}

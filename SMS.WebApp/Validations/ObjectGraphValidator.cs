using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace SMS.WebApp.Validations
{
    public class ObjectGraphValidator : ComponentBase
    {
        [CascadingParameter] private EditContext CurrentEditContext { get; set; } = default!;
        private ValidationMessageStore _messages;

        protected override void OnInitialized()
        {
            if (CurrentEditContext == null)
                throw new InvalidOperationException($"{nameof(ObjectGraphValidator)} requires a cascading " +
                    $"parameter of type {nameof(EditContext)}.");

            _messages = new ValidationMessageStore(CurrentEditContext);

            CurrentEditContext.OnValidationRequested += (s, e) => ValidateModel(CurrentEditContext.Model, _messages);
            CurrentEditContext.OnFieldChanged += (s, e) => ValidateModel(CurrentEditContext.Model, _messages);
        }

        private void ValidateModel(object model, ValidationMessageStore messages)
        {
            messages.Clear();
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model);

            // Validate the root object
            Validator.TryValidateObject(model, context, results, true);

            foreach (var result in results)
            {
                foreach (var memberName in result.MemberNames)
                {
                    var fieldIdentifier = new FieldIdentifier(model, memberName);
                    messages.Add(fieldIdentifier, result.ErrorMessage);
                }
            }

            // 🔑 Recursively validate nested properties
            foreach (var property in model.GetType().GetProperties())
            {
                var value = property.GetValue(model);
                if (value != null && !property.PropertyType.IsPrimitive && property.PropertyType != typeof(string))
                {
                    var nestedResults = new List<ValidationResult>();
                    var nestedContext = new ValidationContext(value);
                    Validator.TryValidateObject(value, nestedContext, nestedResults, true);

                    foreach (var result in nestedResults)
                    {
                        foreach (var memberName in result.MemberNames)
                        {
                            var fieldIdentifier = new FieldIdentifier(value, memberName);
                            messages.Add(fieldIdentifier, result.ErrorMessage);
                        }
                    }
                }
            }
        }
    }
}
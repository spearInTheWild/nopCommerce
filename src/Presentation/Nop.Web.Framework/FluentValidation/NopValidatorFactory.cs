﻿using System;
using FluentValidation;
using FluentValidation.Attributes;
using Nop.Core.Infrastructure;
using Nop.Services.Events;
using Nop.Web.Framework.Events;

namespace Nop.Web.Framework.FluentValidation
{
    /// <summary>
    /// Represents custom validator factory that looks for the attribute instance on the specified type in order to provide the validator instance.
    /// </summary>
    public class NopValidatorFactory : AttributedValidatorFactory
    {
        /// <summary>
        /// Gets a validator for the appropriate type
        /// </summary>
        /// <param name="type">Type</param>
        /// <returns>Created IValidator instance; null if a validator cannot be created</returns>
        public override IValidator GetValidator(Type type)
        {
            if (type == null)
                return null;

            //get a custom attribute applied to a member of a type
            var validatorAttribute = (ValidatorAttribute)Attribute.GetCustomAttribute(type, typeof(ValidatorAttribute));
            if (validatorAttribute == null || validatorAttribute.ValidatorType == null)
                return null;

            //try to create instance of the validator
            var instance = EngineContext.Current.ResolveUnregistered(validatorAttribute.ValidatorType);

            if (instance != null)
            {
                try
                {
                    var eventPublisher = EngineContext.Current.Resolve<IEventPublisher>();

                    eventPublisher.ValidatorPrepare(type, instance as IValidator);
                }
                catch
                {
                    // ignored
                }
            }

            return instance as IValidator;
        }
    }
}
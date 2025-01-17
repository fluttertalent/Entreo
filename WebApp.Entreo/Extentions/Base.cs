﻿using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace WebApp.Entreo.Extentions
{
    static class Guard
    {
        public static T AgainstNull<T>([NotNull] T? argument, [CallerArgumentExpression("argument")] string? paramName = null) where T : class
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }

            return argument;
        }
    }

}

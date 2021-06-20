﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Utils
{
    public sealed class CommandsConnectionString
    {
        public string Value { get; }
        public CommandsConnectionString(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public sealed class QueriesConnectionString
    {
        public string Value { get; }

        public QueriesConnectionString(string value)
        {
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}

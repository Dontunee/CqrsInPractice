using System;
using System.Collections.Generic;
using System.Text;

namespace Logic.Attributes
{
    [AttributeUsage( AttributeTargets.Class,Inherited = false, AllowMultiple = true)]
    public sealed class AuditLogRetryAttribute : Attribute
    {

        public AuditLogRetryAttribute()
        {

        }
    }
}

using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    public class UsingContextAttribute : OnMethodBoundaryAspect
    {
        public override void OnEntry(MethodExecutionArgs args)
        {
        }

        public override void OnExit(MethodExecutionArgs args)
        {
        }

        public override void OnException(MethodExecutionArgs args)
        {
        }
    }
}

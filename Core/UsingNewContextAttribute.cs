using MethodBoundaryAspect.Fody.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core
{
    [Obsolete]
    public class UsingNewContextAttribute : OnMethodBoundaryAspect
    {
        private Guid _mainContext;
        private Guid _currentContext;
        public override void OnEntry(MethodExecutionArgs args)
        {
            //if (!(args.Instance is IBaseRepository))
            //{
            //    throw new Exception("Incorrect context using");
            //}
            //_mainContext = (args.Instance as IBaseRepository).CurrentContext;
            //var newContext = new BillingContext();
            //_currentContext = Guid.NewGuid();
            //(args.Instance as IBaseRepository).CurrentContext = _currentContext;
            //(args.Instance as IBaseRepository).Contexts.Add(_currentContext, newContext);
        }

        public override void OnExit(MethodExecutionArgs args)
        {
            //Remove(args);
        }

        public override void OnException(MethodExecutionArgs args)
        {
            //Remove(args);
        }

        private void Remove(MethodExecutionArgs args)
        {
            //if (!(args.Instance is IBaseRepository))
            //{
            //    throw new Exception("Incorrect context using");
            //}
            //if ((args.Instance as IBaseRepository).Contexts.ContainsKey(_currentContext))
            //{
            //    (args.Instance as IBaseRepository).Contexts[_currentContext].Dispose();
            //    (args.Instance as IBaseRepository).Contexts.Remove(_currentContext);
            //    (args.Instance as IBaseRepository).CurrentContext = _mainContext;
            //}
        }

    }
}

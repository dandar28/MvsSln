using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.r_eg.MvsSln.Core
{
    public class ThrowResult
    {
        public bool bShouldReturn = true;
        public object ResultData = null;
    }

    public interface IExceptionThrower
    {
        bool ShouldThrowErrorsInStrictMode { get; set; }
        ThrowResult Throw(Exception exception, object contextData = null);
    }

    public abstract class AbstractExceptionThrower : IExceptionThrower
    {
        public bool ShouldThrowErrorsInStrictMode { get; set; }
        public ThrowResult Throw(Exception exception, object contextData = null)
        {
            if (ShouldThrowErrorsInStrictMode)
            {
                throw exception;
            }

            var FoundHandler = FindThrowHandler(exception);
            if (FoundHandler != null)
            {
                return FoundHandler(contextData);
            }

            return new ThrowResult();
        }

        public void RegisterThrowHandler(System.Func<Exception, bool> filterMatchCondition, System.Func<object, ThrowResult> handleExceptionThroughData)
        {
            ThrowHandlers.Add(filterMatchCondition, handleExceptionThroughData);
        }

        protected System.Func<object, ThrowResult> FindThrowHandler(Exception exception)
        {
            foreach (var Handler in ThrowHandlers)
            {
                if (Handler.Key(exception))
                {
                    return Handler.Value;
                }
            }
            return null;
        }

        protected IDictionary<System.Func<Exception, bool>, System.Func<object, ThrowResult>> ThrowHandlers = new Dictionary<System.Func<Exception, bool>, System.Func<object, ThrowResult>>();
    }

    public class ExceptionThrower : AbstractExceptionThrower
    {
        // ...
    }
}

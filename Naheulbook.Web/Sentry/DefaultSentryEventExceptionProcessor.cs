using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Sentry;

namespace Naheulbook.Web.Sentry
{
    public static class DefaultSentryEventExceptionProcessor
    {
        public static SentryEvent BeforeSend(SentryEvent sentryEvent)
        {
            if (sentryEvent.Exception != null)
            {
                FlattenException(sentryEvent.Exception, sentryEvent, "__exceptionData");
            }

            return sentryEvent;
        }

        private static void FlattenException(Exception exception, SentryEvent sentryEvent, string key)
        {
            var properties = exception.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propertyInfo in properties.Where(x => x.DeclaringType != typeof(Exception)))
            {
                try
                {
                    var value = JsonConvert.SerializeObject(propertyInfo.GetValue(exception, null));
                    sentryEvent.SetExtra(key + "." + propertyInfo.Name, value);
                }
                catch (Exception ex)
                {
                    sentryEvent.SetExtra(key + "." + propertyInfo.Name, "ERROR accessing value: " + ex.Message);
                }
            }

            if (exception.InnerException != null)
            {
                FlattenException(exception.InnerException, sentryEvent, key + ".innerException");
            }
        }
    }
}
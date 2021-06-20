using Logic.Attributes;
using Logic.Decorators;
using Logic.Students;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Api.Utils
{
    public static class HandlerRegistration
    {

        public static void AddHandlers(this IServiceCollection services)
        {
            
            List<Type> handlerTypes = typeof(ICommand).Assembly.GetTypes()
                                        .Where(x => x.GetInterfaces().Any(y => IsHandlerInterface(y)))
                                        .Where(x => x.Name.EndsWith("Handler"))
                                        .ToList();

            foreach (Type type in handlerTypes)
            {
                AddHandler(services, type);
            }
        }

        private static void AddHandler(IServiceCollection services, Type type)
        {
            //retrieves all attributes attached to the typpe
            object[] attributes = type.GetCustomAttributes(false);

            //Converts them to decorator type then gets the types and creates a list 
            List<Type> pipeline = attributes
                    .Select(x => ToDecorator(x))
                    .Concat(new[] { type })
                    .Reverse()
                    .ToList();

            //Gets the interface the handler implements
            Type interfaceType = type.GetInterfaces().Single(y => IsHandlerInterface(y));

            //build a factory out of all the decorators and handlers gotten
            Func<IServiceProvider, object> factory = BuildPipeline(pipeline, interfaceType);

            //Register in service collection
            services.AddTransient(interfaceType, factory);

        }

        private static Func<IServiceProvider, object> BuildPipeline(List<Type> pipeline, Type interfaceType)
        {
            //Gets constructors of each of the pipeline types
            List<ConstructorInfo> ctors = pipeline
                                            .Select(x =>
                                            {
                                                Type type = x.IsGenericType ? x.MakeGenericType(interfaceType.GenericTypeArguments) : x;
                                                return type.GetConstructors().Single();
                                            })
                                            .ToList();

            //Creates and return the delegate
            Func<IServiceProvider, object> func  = provider => 
            {
                object current = null;

                foreach (ConstructorInfo ctor in ctors)
                {
                    //Get the list of parameters they require
                    List<ParameterInfo> paramaterInfos = ctor.GetParameters().ToList();
                    //resolve them 
                    object[] parameters = GetParameters(paramaterInfos, current, provider);

                    //invoke the constructor  and create an instance in the pipeline 
                    current = ctor.Invoke(parameters);
                }

                return current;
            };
            return func;
        }

        private static object[] GetParameters(List<ParameterInfo> paramaterInfos, object current, IServiceProvider provider)
        {
            var result = new object[paramaterInfos.Count];

            for(int i = 0; i < paramaterInfos.Count; i++)
            {
                result[i] = GetParameter(paramaterInfos[i], current, provider);
            }

            return result;
        }

        private static object GetParameter(ParameterInfo parameterInfo, object current, IServiceProvider provider)
        {
            //Get the type of the parameter
            Type parameterType = parameterInfo.ParameterType;

            //If its an handler interface, use the object in the pipeline instantiated
            if (IsHandlerInterface(parameterType))
                return current;


            //use d.i container to resolve it 
            object service = provider.GetService(parameterType);
            if (service != null)
                return service;

            throw new ArgumentException($"Type {parameterType} not found");

        }

        private static Type ToDecorator(object attribute)
        {
            Type type = attribute.GetType();

            if (type == typeof(DatabaseRetryAttribute))
                return typeof(DatabaseRetryDecorator<>);

            if (type == typeof(AuditLogRetryAttribute))
                return typeof(AuditLoggingDecorator<>);

            //other attributes go here

            throw new ArgumentException(attribute.ToString());
        }

        private static bool IsHandlerInterface(Type type)
        {
            if (!type.IsGenericType)
                return false;

            Type typeDefinition = type.GetGenericTypeDefinition();

            return typeDefinition == typeof(ICommandHandler<>) || typeDefinition == typeof(IQueryHandler<,>);
        }
    }
}

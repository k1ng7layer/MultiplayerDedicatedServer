using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Builder;

namespace MultiplayerDedicatedServer.Utils
{
    public static class RegistrationSubstitute
    {
        private static readonly Dictionary<Type, Type> SubstituteType = new();
        private static readonly Dictionary<Type, object> SubstituteData = new();
        
        private static bool GetSubstituteData<TContract>(out TContract substitute) where TContract : class
        {
            substitute = default(TContract);
            if (!SubstituteData.ContainsKey(typeof(TContract)))
                return false;
            substitute = (TContract)SubstituteData[typeof(TContract)];
            return true;
        }

        public static IRegistrationBuilder<TConcrete, IConcreteActivatorData, SingleRegistrationStyle> 
            BindFromSubstitute<TConcrete, TContract>(this ContainerBuilder containerBuilder) where TConcrete : notnull where TContract : class
        {
            var hasSubstitute = GetSubstituteData<TContract>(out var substitute);
            
            var registrationBuilder =
                hasSubstitute
                    ? (IRegistrationBuilder<TConcrete, IConcreteActivatorData, SingleRegistrationStyle>)containerBuilder.RegisterInstance(substitute)
                    : containerBuilder.RegisterType<TConcrete>().As<IComparable>();
            
            return registrationBuilder;
        }
    }
}
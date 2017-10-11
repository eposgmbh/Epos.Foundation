using System;
using System.Reflection;

namespace Epos.Utilities.Composition
{
    public abstract class ComponentCreationStrategy
    {
        internal ComponentRegistration ComponentRegistration { get; set; }

        internal object GetInstance() {
            Delegate theFactoryMethod = ComponentRegistration.FactoryMethod;
            if (theFactoryMethod != null) {
                return GetInstance(theFactoryMethod);
            } else {
                ConstructorInfo theConstructurInfo = ComponentRegistration.ConstructurInfo;
                return GetInstance(
                    theConstructurInfo, GetParameterValues(theConstructurInfo, ComponentRegistration.Container)
                );
            }
        }

        protected abstract object GetInstance(ConstructorInfo constructorInfo, object[] parameterValues);

        protected abstract object GetInstance(Delegate factoryMethod);

        #region Hilfsmethoden

        private object[] GetParameterValues(ConstructorInfo constructurInfo, Container container) {
            ParameterInfo[] theParameterInfos = constructurInfo.GetParameters();
            
            object[] theParameterValues = new object[theParameterInfos.Length];
            for (int theIndex = 0; theIndex < theParameterValues.Length; theIndex++) {
                theParameterValues[theIndex] = GetParameterValue(container, theParameterInfos[theIndex]);
            }
            return theParameterValues;
        }

        private object GetParameterValue(Container container, ParameterInfo parameterInfo) {
            Type theParameterType = parameterInfo.ParameterType;

            object theParameterValue;
            if (!ComponentRegistration.Parameters.TryGetValue(parameterInfo.Name, out theParameterValue)) {
                theParameterValue = container.Resolve(theParameterType);
            } else {
                TestParameterValue(parameterInfo, theParameterValue);
            }

            return theParameterValue;
        }

        private static void TestParameterValue(ParameterInfo parameterInfo, object parameterValue) {
            Type theParameterType = parameterInfo.ParameterType;

            if (parameterValue != null) {
                if (parameterValue.GetType() != theParameterType) {
                    throw new InvalidOperationException(
                        $"Parameter '{parameterInfo.Name}' must be of type {theParameterType.Dump()} (is {parameterValue.GetType().Dump()})."
                        );
                }
            } else {
                if (theParameterType.IsValueType) {
                    throw new InvalidOperationException(
                        $"Parameter '{parameterInfo.Name}' is a value type and therefore cannot be null."
                        );
                }
            }
        }

        #endregion
    }
}
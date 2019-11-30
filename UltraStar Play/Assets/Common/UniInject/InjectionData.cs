
using System.Reflection;

namespace UniInject
{

    public class InjectionData
    {
        // The object that needs injection. The member belongs to this object.
        public object TargetObject { get; private set; }

        // The member of the target object that needs injection.
        public MemberInfo MemberInfo { get; private set; }

        // A method can have multiple parameters and all of them have to be injected.
        // Thus, there can be multiple injectionKeys for a member.
        public object[] InjectionKeys { get; private set; }

        public SearchMethods searchMethod;

        public bool isOptional;

        public InjectionData(object targetObject, MemberInfo memberInfo, object injectionKey, SearchMethods strategy, bool isOptional)
            : this(targetObject, memberInfo, new object[] { injectionKey }, strategy, isOptional)
        {
        }

        public InjectionData(object targetObject, MemberInfo memberInfo, object[] injectionKeys, SearchMethods strategy, bool isOptional)
        {
            this.TargetObject = targetObject;
            this.MemberInfo = memberInfo;
            this.InjectionKeys = injectionKeys;
            this.searchMethod = strategy;
            this.isOptional = isOptional;
        }
    }

}
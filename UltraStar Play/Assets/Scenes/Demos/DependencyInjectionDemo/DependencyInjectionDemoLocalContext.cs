using Zenject;

public class DependencyInjectionDemoLocalContext : MonoInstaller
{
    public string injectedName;

    public override void InstallBindings()
    {
        // Bindings that are local to child objects.
        // It works, because the Context for dependency injection is searched by Zenject
        // in the object hierarchy and thus will encounter this Installer, but not its sibling.
        Container.BindInstance(injectedName).WithId("theInjectedNameId");
    }
}

using UnityEngine;

public class LaunchInitializationController : MonoBehaviour
{
    private void Awake()
    {
        InstallBindings();
    }

    private void InstallBindings()
    {
        ServiceLocator.CreateAndRegisterService<IGeminiRequestService, GeminiRequestService>();
        ServiceLocator.CreateAndRegisterService<IPromptBuilderService, PromptBuilderService>();
        ServiceLocator.CreateAndRegisterService<INumberSetGenerator, NumberSetGenerator>();
    }
}

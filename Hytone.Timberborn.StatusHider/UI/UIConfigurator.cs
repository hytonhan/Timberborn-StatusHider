using Bindito.Core;
using System;
using System.Collections.Generic;
using System.Text;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace Hytone.Timberborn.StatusHider.UI
{
    [Configurator(SceneEntrypoint.All)]
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<StatusHiderMenu>().AsSingleton();
        }
    }
}

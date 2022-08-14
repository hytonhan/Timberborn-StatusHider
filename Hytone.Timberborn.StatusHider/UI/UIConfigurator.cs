using Bindito.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hytone.Timberborn.StatusHider.UI
{
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<StatusHiderMenu>().AsSingleton();
        }
    }
}

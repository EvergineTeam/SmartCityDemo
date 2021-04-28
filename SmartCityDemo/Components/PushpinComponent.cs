using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;

namespace SmartCityDemo.Components
{
    public class PushpinComponent : Component
    {
        [BindComponent]
        public Transform3D Transform;

        [BindComponent(source: BindComponentSource.Parents)]
        public BuildingComponent BuildingComponent;
    }
}

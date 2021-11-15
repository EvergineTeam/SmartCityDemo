using System;
using System.Collections.Generic;
using System.Text;
using Evergine.Framework;
using Evergine.Framework.Graphics;

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

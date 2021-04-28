using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Components.WorkActions;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace SmartCityDemo.Components
{
    public class BuildingComponent : Component
    {
        [BindComponent]
        public Transform3D Transform;

        [BindComponent(source:BindComponentSource.Children)]
        public PushpinComponent PushpinComponent;

        [BindComponent(source: BindComponentSource.Children)]
        public ColapseComponent ColapseComponent;

        public void Select()
        {
            this.ColapseComponent.Show();
        }

        public void Unselect()
        {
            this.ColapseComponent.Hide();
        }
    }
}

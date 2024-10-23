using Evergine.Framework;
using Evergine.Framework.Graphics;

namespace SmartCityDemo.Components
{
    public class BuildingComponent : Component
    {
        [BindComponent]
        public Transform3D Transform;

        [BindComponent(source:BindComponentSource.Children)]
        public PushpinComponent PushpinComponent;

        [BindComponent(source: BindComponentSource.Children)]
        public CollapseComponent CollapseComponent;

        public void Select()
        {
            this.CollapseComponent.Show();
        }

        public void Unselect()
        {
            this.CollapseComponent.Hide();
        }
    }
}

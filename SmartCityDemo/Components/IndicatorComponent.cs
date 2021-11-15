using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Mathematics;

namespace SmartCityDemo.Components
{
    public class IndicatorComponent : Component
    {
        [BindComponent]
        private Transform3D transform;

        private Transform3D targetTransform;

        public Vector2 Center;

        public float Distance = 1.5f;

        public string PointerTag;

        protected override bool OnAttached()
        {
            if (!Application.Current.IsEditor)
            {
                if (!string.IsNullOrEmpty(this.PointerTag))
                {
                    this.targetTransform = this.Managers.EntityManager.FindAllByTag(this.PointerTag).FirstOrDefault()?.FindComponent<Transform3D>();

                    if (this.targetTransform != null)
                    {
                        this.targetTransform.PositionChanged += OnPointerChanged;

                        this.RefreshVisible();
                    }
                }
            }

            return base.OnAttached();
        }

        private void OnPointerChanged(object sender, EventArgs e)
        {
            this.RefreshVisible();
        }

        private void RefreshVisible()
        {
            var pos2D = new Vector2(this.targetTransform.Position.X, this.targetTransform.Position.Z);
            var vector = pos2D - this.Center;
            var distance = vector.Length();

            this.Owner.IsEnabled = distance > this.Distance;

            if (this.Owner.IsEnabled)
            {
                var lookAt = this.targetTransform.Position;
                lookAt.Y = this.transform.Position.Y;

                this.transform.LookAt(lookAt);
                ////this.transform.LocalRotation = Vector3.UnitY * angle;
            }
        }
    }
}

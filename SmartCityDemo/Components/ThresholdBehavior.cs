using System;
using System.Collections.Generic;
using System.Text;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Mathematics;

namespace SmartCityDemo.Components
{
    public class ThresholdBehavior : Behavior
    {
        [BindComponent]
        private Transform3D transform;

        public Vector2 Center;

        public float Distance = 1.4f;

        public float SmoothTime = 0.5f;

        private Vector3 scaleVelocity;

        private Vector3 initScale;

        private Vector3 targetScale;

        private bool isAnimating;

        protected override bool OnAttached()
        {
            if (!Application.Current.IsEditor)
            {
                this.initScale = this.transform.LocalScale;
                this.transform.PositionChanged += this.OnPositionChanged;
                this.RefreshScale();
                this.transform.LocalScale = this.targetScale;
            }
            return base.OnAttached();
        }

        protected override void OnDetach()
        {
            base.OnDetach();

            this.transform.PositionChanged -= this.OnPositionChanged;
        }

        private void OnPositionChanged(object sender, EventArgs e)
        {
            this.RefreshScale();
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.transform.LocalScale = Vector3.SmoothDamp(this.transform.LocalScale, this.targetScale, ref this.scaleVelocity, this.SmoothTime, (float)gameTime.TotalSeconds);
        }

        private void RefreshScale()
        {
            var pos2D = new Vector2(this.transform.Position.X, this.transform.Position.Z);
            var distance = Vector2.Distance(pos2D, this.Center);
            this.targetScale = (distance < this.Distance) ? this.initScale : Vector3.Zero;
        }
    }
}

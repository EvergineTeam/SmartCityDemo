using Evergine.Components.WorkActions;
using Evergine.Framework;
using Evergine.Framework.Graphics;
using Evergine.Framework.Services;
using Evergine.Mathematics;
using System;

namespace SmartCityDemo.Components
{
    public class CollapseComponent : Component
    {
        [BindComponent]
        public Transform3D Transform;

        public float AnimationTime = 0.5f;

        private Vector3 initScale;
        private Vector3 collapsedScale;
        private IWorkAction currentAction;

        protected override void OnActivated()
        {
            base.OnActivated();
            this.initScale = this.Transform.LocalScale;
            
            this.collapsedScale = this.initScale;
            this.collapsedScale.Y = 0;

            if (!Application.Current.IsEditor)
            {
                this.Transform.LocalScale = this.collapsedScale; 
            }
        }

        public void Show()
        {
            this.currentAction?.Cancel();
            this.currentAction = this.Owner.Scene.CreateWorkAction(new ScaleTo3DWorkAction(this.Owner, this.initScale, TimeSpan.FromSeconds(this.AnimationTime), EaseFunction.CubicInOutEase));
            this.currentAction.Run();
        }

        public void Hide()
        {
            this.currentAction?.Cancel();
            this.currentAction = this.Owner.Scene.CreateWorkAction(new ScaleTo3DWorkAction(this.Owner, this.collapsedScale, TimeSpan.FromSeconds(this.AnimationTime), EaseFunction.CubicInOutEase));
            this.currentAction.Run();
        }
    }
}

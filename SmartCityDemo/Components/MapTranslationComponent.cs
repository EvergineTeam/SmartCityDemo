using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using WaveEngine.Common.Input.Mouse;
using WaveEngine.Common.Input.Pointer;
using WaveEngine.Framework;
using WaveEngine.Framework.Graphics;
using WaveEngine.Framework.Physics3D;
using WaveEngine.Mathematics;

namespace SmartCityDemo.Components
{
    public class MapTranslationComponent : Behavior
    {
        [BindComponent(source: BindComponentSource.ChildrenSkipOwner)]
        private Transform3D translationPivot = null;

        [BindComponent(source: BindComponentSource.Scene)]
        private Camera3D camera = null;

        private MouseDispatcher mouseDispatcher;

        private bool translationInProgress;
        private Vector3 initPosition;
        private Vector3 initTouchPosition;
        private Vector3 targetPosition;
        private Vector3 velocity;

        public Vector3 ClampMaxPosition { get; set; }
        public Vector3 ClampMinPosition { get; set; }

        public float Smooth { get; set; } = 0.1f;
        
        public float PushpinSmooth { get; set; } = 0.3f;

        private float currentSmooth;
        private BuildingComponent currentSelectedBuilding;

        protected override void OnActivated()
        {
            base.OnActivated();

            this.mouseDispatcher = this.camera.Display?.MouseDispatcher;
            if (this.mouseDispatcher != null)
            {
                this.RegisterMouseEvents();
            }

            this.targetPosition = this.translationPivot.Position;
        }

        protected override void Update(TimeSpan gameTime)
        {
            this.translationPivot.Position = Vector3.SmoothDamp(this.translationPivot.Position, this.targetPosition, ref this.velocity, this.currentSmooth, (float)gameTime.TotalSeconds);
        }

        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            if (this.mouseDispatcher != null)
            {
                this.UnregisterMouseEvents();
            }
        }

        private void BeginTranslation(Vector2 screenPos)
        {
            this.camera.CalculateRay(ref screenPos, out var ray);

            var hitResult = this.Managers.PhysicManager3D.RayCast(ref ray, 10, CollisionCategory3D.All);
            if (hitResult.Succeeded)
            {
                this.initPosition = this.translationPivot.Position;
                var body = hitResult.PhysicBody.BodyComponent;
                var pushpin = body.Owner.FindComponent<PushpinComponent>();

                if (pushpin != null)
                {
                    this.SelectBuilding(pushpin.BuildingComponent);
                    this.currentSmooth = this.PushpinSmooth;
                }
                else
                {
                    this.UnselectBuilding();
                    this.translationInProgress = true;
                    this.currentSmooth = this.Smooth;
                    this.initTouchPosition = ray.IntersectionYPlane(0);
                }
            }
        }

        private void UpdateTranslation(Vector2 screenPos)
        {
            if (this.translationInProgress)
            {
                this.camera.CalculateRay(ref screenPos, out var ray);
                var currentTouchPosition = ray.IntersectionYPlane(0);                

                // final position
                var finalPosition = this.initPosition + (currentTouchPosition - this.initTouchPosition);

                this.targetPosition = Vector3.Clamp(finalPosition, ClampMinPosition, ClampMaxPosition);
            }
        }

        private void EndTranslation()
        {
            this.translationInProgress = false;
        }

        public void SelectBuilding(BuildingComponent building)
        {
            this.currentSelectedBuilding = building;
            this.currentSelectedBuilding.Select();

            var newPosition = this.translationPivot.Position - building.Transform.Position;
            this.JumpToPosition(newPosition);
        }

        private void UnselectBuilding()
        {
            this.currentSelectedBuilding?.Unselect();
            this.currentSelectedBuilding = null;
        }

        public void JumpToPosition(Vector3 newPosition)
        {
            this.targetPosition = newPosition;
            this.translationInProgress = false;
        }

        private void RegisterMouseEvents()
        {
            this.mouseDispatcher.MouseButtonDown += this.MouseDispatcher_MouseButtonDown;
            this.mouseDispatcher.MouseMove += this.MouseDispatcher_MouseMove;
            this.mouseDispatcher.MouseButtonUp += this.MouseDispatcher_MouseButtonUp;
            this.mouseDispatcher.MouseLeave += this.MouseDispatcher_MouseLeave; ;
        }

        private void UnregisterMouseEvents()
        {
            this.mouseDispatcher.MouseButtonDown -= this.MouseDispatcher_MouseButtonDown;
            this.mouseDispatcher.MouseMove -= this.MouseDispatcher_MouseMove;
            this.mouseDispatcher.MouseButtonUp -= this.MouseDispatcher_MouseButtonUp;
            this.mouseDispatcher.MouseLeave -= this.MouseDispatcher_MouseLeave; ;
        }

        private void MouseDispatcher_MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                this.BeginTranslation(e.Position.ToVector2()); 
            }
        }

        private void MouseDispatcher_MouseMove(object sender, MouseEventArgs e)
        {
            this.UpdateTranslation(e.Position.ToVector2());
        }

        private void MouseDispatcher_MouseButtonUp(object sender, MouseButtonEventArgs e)
        {
            this.EndTranslation();
        }

        private void MouseDispatcher_MouseLeave(object sender, MouseEventArgs e)
        {
            this.EndTranslation();
        }
    }
}

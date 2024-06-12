using Evergine.Framework;

namespace SmartCityDemo
{
    public class MyScene : Scene
    {
		public override void RegisterManagers()
        {
        	base.RegisterManagers();
        	this.Managers.AddManager(new Evergine.Bullet.BulletPhysicManager3D());
        }

        protected override void CreateScene()
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Algo.Utils
{
    public abstract class ResourceHolder
    {


        private readonly List<Resource> list = new List<Resource>(2);
        private readonly List<Resource> subList = new List<Resource>(2);

        public abstract string GetName();

        public void HoldResource(Resource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            this.list.Add(resource);
        }

        public void HoldSubResource(Resource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            this.subList.Add(resource);
        }

        public void RemoveResource(Resource resource)
        {
            if (resource == null) throw new ArgumentNullException(nameof(resource));
            this.list.Remove(resource);
        }

        public void CloseAllSubResources()
        {
            _CloseAllResources(this.subList);
        }

        public void CloseAllResources()
        {
            _CloseAllResources(this.list);
        }

        private void _CloseAllResources(List<Resource> resources)
        {
            foreach (var resource in resources)
            {
               
                if (!resource.IsClosed())
                {
                    resource.Close();
                    var message = $"Resource {resource.GetType()} in {GetName()} not closed normally.";
           

                    if (resource.GetReferCount() > 0)
                    {
                        resource.RealClose();
                        message = $"Resource {resource.GetType()} in {GetName()} not closed normally, closing it automatically!";
                    
                    }
                }
            }
            resources.Clear();
        }
    }
}


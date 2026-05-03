using TCTOS.Impls.Incus.DTOs;

namespace TCTOS.Util;

public static class Extensions
{
    public static TData WaitAndGet<TData>(this Task<TData> task)
    {
        task.Wait();
        return task.Result;
    }

    extension(Instance instance)
    {
        public InstancesPost ToInstancesPost()
        {
            return new InstancesPost
            {
                Description = instance.Description,
                Type = instance.Type,
                Name = instance.Name,
                Devices = instance.Devices,
                Stateful = instance.Stateful,
                Architecture = instance.Architecture,
                Config = instance.Config,
                Ephemeral = instance.Ephemeral,
                Profiles = instance.Profiles
            };
        }

        public InstancePut ToInstancePut()
        {
            return new InstancePut
            {
                Description = instance.Description,
                Devices = instance.Devices,
                Stateful = instance.Stateful,
                Architecture = instance.Architecture,
                Config = instance.Config,
                Ephemeral = instance.Ephemeral,
                Profiles = instance.Profiles
            };
        }
    }
}
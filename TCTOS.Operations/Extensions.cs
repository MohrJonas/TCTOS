using TCTOS.Abstractions.Incus.DTOs;

namespace TCTOS.Util;

public static class Extensions
{
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
﻿using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlatoCore.Models.Shell;
using PlatoCore.Stores.Abstractions.Shell;

namespace PlatoCore.Shell
{

    public class ShellDescriptorManager : IShellDescriptorStore
    {

        private readonly IEnumerable<ShellModule> _shellFeatures;
        private IShellDescriptor _shellDescriptor;

        public ShellDescriptorManager(IEnumerable<ShellModule> shellFeatures)
        {
            _shellFeatures = shellFeatures;
        }

        public Task<IShellDescriptor> GetAsync()
        {
            if (_shellDescriptor == null)
            {
                _shellDescriptor = new ShellDescriptor
                {
                    Modules = _shellFeatures.ToList()
                };
            }

            return Task.FromResult(_shellDescriptor);
        }

        public Task<IShellDescriptor> SaveAsync(IShellDescriptor model)
        {
            return Task.FromResult(default(IShellDescriptor));
        }

        public Task<bool> DeleteAsync()
        {
            return Task.FromResult(default(bool));
        }

    }

}

﻿using System.Collections.Generic;
using PlatoCore.Assets.Abstractions;

namespace Plato.Categories.Assets
{
    public class AssetProvider : IAssetProvider
    {
        
        public IEnumerable<AssetEnvironment> GetAssetEnvironments()
        {

            return new List<AssetEnvironment>
            {

                // Development
                new AssetEnvironment(TargetEnvironment.Development, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.categories/content/js/categories.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Staging
                new AssetEnvironment(TargetEnvironment.Staging, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.categories/content/js/categories.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                }),

                // Production
                new AssetEnvironment(TargetEnvironment.Production, new List<Asset>()
                {
                    new Asset()
                    {
                        Url = "/plato.categories/content/js/categories.min.js",
                        Type = AssetType.IncludeJavaScript,
                        Section = AssetSection.Footer
                    }
                })

            };

        }

    }

}
